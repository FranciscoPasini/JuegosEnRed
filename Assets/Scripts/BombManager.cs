using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;

public class BombManagerPhoton : MonoBehaviourPunCallbacks
{
    [Header("Timer")]
    public float countdownTime = 10f; // duración en segundos
    private double endTime; // tiempo global en que termina la ronda

    [Header("UI")]
    public TMP_Text timerText;

    [Header("Bomba")]
    public GameObject playerWithBomb;
    public Color bombColor = Color.red;
    public Color normalColor = Color.white;

    private List<GameObject> players = new List<GameObject>();

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // al empezar, sincronizar lista de jugadores
            FindAllPlayers();

            // asignar bomba inicial aleatoria
            int randomIndex = Random.Range(0, players.Count);
            GameObject randomPlayer = players[randomIndex];
            photonView.RPC("SetBombOwner", RpcTarget.All, randomPlayer.GetComponent<PhotonView>().ViewID);

            // iniciar timer global
            StartNewRound();
        }
    }

    void Update()
    {
        if (endTime > 0)
        {
            double timeLeft = endTime - PhotonNetwork.Time;

            if (timeLeft > 0)
            {
                if (timerText != null)
                    timerText.text = Mathf.CeilToInt((float)timeLeft).ToString();
            }
            else
            {
                if (timerText != null)
                    timerText.text = "0";

                if (PhotonNetwork.IsMasterClient)
                {
                    // Master decide la explosión
                    photonView.RPC("Explode", RpcTarget.All, playerWithBomb.GetComponent<PhotonView>().ViewID);
                }

                endTime = 0; // detener timer
            }
        }
    }

    void StartNewRound()
    {
        endTime = PhotonNetwork.Time + countdownTime;
        photonView.RPC("SyncTimer", RpcTarget.All, endTime);
    }

    void FindAllPlayers()
    {
        players.Clear();
        foreach (var view in FindObjectsOfType<PhotonView>())
        {
            if (view.CompareTag("Player"))
                players.Add(view.gameObject);
        }
    }

    // 🔹 Sincronizar Timer
    [PunRPC]
    void SyncTimer(double networkEndTime)
    {
        endTime = networkEndTime;
    }

    // 🔹 Pasar la bomba a otro jugador
    public void TryPassBomb(GameObject newOwner)
    {
        if (playerWithBomb == newOwner) return;
        photonView.RPC("SetBombOwner", RpcTarget.All, newOwner.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void SetBombOwner(int viewID)
    {
        GameObject newOwner = PhotonView.Find(viewID).gameObject;
        if (playerWithBomb != null)
        {
            var sr = playerWithBomb.GetComponent<SpriteRenderer>();
            if (sr) sr.color = normalColor;
        }

        playerWithBomb = newOwner;

        var srNew = playerWithBomb.GetComponent<SpriteRenderer>();
        if (srNew) srNew.color = bombColor;
    }

    // 🔹 Explosión
    [PunRPC]
    void Explode(int victimViewID)
    {
        GameObject victim = PhotonView.Find(victimViewID).gameObject;
        Debug.Log("💥 Explota: " + victim.name);

        players.Remove(victim);
        Destroy(victim);

        if (PhotonNetwork.IsMasterClient)
        {
            if (players.Count > 0)
            {
                int randomIndex = Random.Range(0, players.Count);
                GameObject newOwner = players[randomIndex];
                photonView.RPC("SetBombOwner", RpcTarget.All, newOwner.GetComponent<PhotonView>().ViewID);

                StartNewRound();
            }
            else
            {
                Debug.Log("🏁 Fin del juego - no quedan jugadores.");
            }
        }
    }
}

