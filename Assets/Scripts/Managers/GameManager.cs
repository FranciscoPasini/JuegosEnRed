using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections; // Necesario para Corrutinas

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    private readonly List<PlayerStateController> players = new List<PlayerStateController>();

    [Header("UI References")]
    [SerializeField] private TMP_Text TimerText;
    // [IMPORTANTE] Arrastra tu objeto que tiene el script LeaderboardUI aquí en el Inspector
    [SerializeField] private LeaderboardUI leaderboardUI;

    [Header("Game Settings")]
    [SerializeField] private int pointsPerWin = 100;
    [SerializeField] private string leaderboardKey = "global_highscore"; // La Key de tu dashboard

    private float currentTime = 0f;
    private bool isCounting = false;
    private bool bombActive = false;
    private int currentBombOwner = -1;

    private double bombStartTime;
    private float bombDuration;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Aseguramos que la leaderboard empiece oculta
        if (leaderboardUI != null) leaderboardUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isCounting && bombActive)
        {
            double elapsed = PhotonNetwork.Time - bombStartTime;
            currentTime = bombDuration - (float)elapsed;

            if (PhotonNetwork.IsMasterClient && currentTime <= 0f)
            {
                currentTime = 0f;
                bombActive = false;
                photonView.RPC(nameof(RPC_ExplodeCurrentBombOwner), RpcTarget.AllBuffered, currentBombOwner);
            }

            if (TimerText != null)
                TimerText.text = Mathf.CeilToInt(Mathf.Max(currentTime, 0f)).ToString();
        }
    }

    public void RegisterPlayer(PlayerStateController player)
    {
        if (!players.Contains(player))
            players.Add(player);
    }

    public void UnregisterPlayer(PlayerStateController player)
    {
        players.Remove(player);
        CheckWinner();
    }

    public void BeginMatch()
    {
        if (PhotonNetwork.IsMasterClient)
            Invoke(nameof(AssignBombAfterDelay), 5f);
    }

    public void AssignBombAfterDelay()
    {
        if (players.Count == 0) return;

        int idx = Random.Range(0, players.Count);
        PlayerStateController chosen = players[idx];
        int actorNumber = chosen.pv.Owner != null ? chosen.pv.Owner.ActorNumber : -1;

        StartBombTimer(10f, actorNumber);
    }

    public void StartBombTimer(float duration, int actorNumber)
    {
        bombDuration = duration;
        bombStartTime = PhotonNetwork.Time;
        isCounting = true;
        bombActive = true;
        currentBombOwner = actorNumber;

        photonView.RPC(nameof(RPC_StartBombTimerSync), RpcTarget.AllBuffered, bombStartTime, bombDuration, actorNumber);
    }

    [PunRPC]
    private void RPC_StartBombTimerSync(double startTime, float duration, int actorNumber)
    {
        bombStartTime = startTime;
        bombDuration = duration;
        isCounting = true;
        bombActive = true;
        currentBombOwner = actorNumber;

        RPC_AssignBomb(actorNumber);
    }

    public void StopBombTimer()
    {
        isCounting = false;
        bombActive = false;
        currentTime = 0f;

        if (TimerText != null)
            TimerText.text = "";
    }

    [PunRPC]
    private void RPC_AssignBomb(int actorNumber)
    {
        foreach (var p in FindObjectsOfType<PlayerStateController>())
        {
            int ownerActor = (p.pv.Owner != null) ? p.pv.Owner.ActorNumber : -1;
            if (ownerActor == actorNumber)
                p.ChangeState(new BombState());
            else
                p.ChangeState(new NormalState());
        }
    }

    [PunRPC]
    private void RPC_ExplodeCurrentBombOwner(int actorNumber)
    {
        foreach (var p in FindObjectsOfType<PlayerStateController>())
        {
            if (p.pv.Owner != null && p.pv.Owner.ActorNumber == actorNumber)
                p.RPC_Die();
        }

        StopBombTimer();

        // Solo continuamos si hay mas de 1 jugador, si no CheckWinner se encarga
        if (players.Count > 1 && PhotonNetwork.IsMasterClient)
            Invoke(nameof(AssignBombAfterDelay), 3f);
    }

    public void NotifyBombPass(int newOwnerActor)
    {
        currentBombOwner = newOwnerActor;
        photonView.RPC(nameof(RPC_AssignBomb), RpcTarget.AllBuffered, newOwnerActor);
    }

    // --- LÓGICA DE GANADOR Y LEADERBOARD ---

    // En GameManager.cs

    public void CheckWinner()
    {
        int alive = 0;
        PlayerStateController winner = null;

        foreach (var p in players)
        {
            if (p.gameObject.activeSelf)
            {
                alive++;
                winner = p;
            }
        }

        if (alive == 1 && winner != null)
        {
            Debug.Log("?? Ganador detectado: " + winner.name);

            // 1. Activar el UI visualmente (pero quizás esperar para el Refresh)
            if (leaderboardUI != null)
            {
                leaderboardUI.gameObject.SetActive(true);
                // NO hacemos Refresh() aquí todavía para evitar mostrar datos viejos
                leaderboardUI.tableText.text = "Calculando puntajes...";
            }

            // 2. Si YO soy el ganador, subo mi puntaje
            if (winner.pv.IsMine)
            {
                Debug.Log("Soy el ganador local, iniciando suma de puntaje...");
                LeaderboardService.AddScore(pointsPerWin, leaderboardKey, (success) =>
                {
                    if (success)
                    {
                        Debug.Log("Puntaje actualizado. Avisando a todos para actualizar tabla.");
                        // Avisamos a todos (incluyéndome) que actualicen la tabla AHORA
                        photonView.RPC("RPC_UpdateLeaderboardUI", RpcTarget.All);
                    }
                });
            }
            else
            {
                // Si no soy el ganador, espero a que el ganador termine de subir su score
                // O podemos esperar el RPC del ganador.
            }

            // 3. El Master Client coordina el reinicio
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(WaitAndRestartMatch());
            }
        }
    }

    // AGREGAR ESTE NUEVO RPC en GameManager.cs
    [PunRPC]
    public void RPC_UpdateLeaderboardUI()
    {
        if (leaderboardUI != null && leaderboardUI.gameObject.activeSelf)
        {
            leaderboardUI.Refresh();
        }
    }

    private IEnumerator WaitAndRestartMatch()
    {
        // Esperamos 10 segundos para ver la tabla
        yield return new WaitForSeconds(10f);
        photonView.RPC("RPC_RestartMatch", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_RestartMatch()
    {
        PhotonNetwork.LoadLevel("Levels");
    }
}


