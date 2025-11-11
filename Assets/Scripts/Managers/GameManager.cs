using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    private readonly List<PlayerStateController> players = new List<PlayerStateController>();

    [SerializeField] private TMP_Text TimerText;

    private float currentTime = 0f;
    private bool isCounting = false;
    private bool bombActive = false;
    private int currentBombOwner = -1;

    // ?? Variables de sincronización del tiempo
    private double bombStartTime;
    private float bombDuration;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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
    }

    public void BeginMatch()
    {
        if (PhotonNetwork.IsMasterClient)
            Invoke(nameof(AssignBombAfterDelay), 5f);
    }

    // ?? Ahora es pública
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

        if (PhotonNetwork.IsMasterClient)
            Invoke(nameof(AssignBombAfterDelay), 3f);
    }

    public void NotifyBombPass(int newOwnerActor)
    {
        currentBombOwner = newOwnerActor;
        photonView.RPC(nameof(RPC_AssignBomb), RpcTarget.AllBuffered, newOwnerActor);
    }

    public void CheckWinner()
    {
        int alive = 0;
        PlayerStateController last = null;

        foreach (var p in players)
        {
            if (p.gameObject.activeSelf)
            {
                alive++;
                last = p;
            }
        }

        if (alive == 1 && last != null)
        {
            Debug.Log("?? Ganador: " + last.name);
            if (PhotonNetwork.IsMasterClient)
                photonView.RPC("RPC_RestartMatch", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void RPC_RestartMatch()
    {
        PhotonNetwork.LoadLevel("Levels");
    }
}


