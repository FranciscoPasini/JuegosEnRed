using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    private readonly List<PlayerStateController> players = new List<PlayerStateController>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Debug.Log("GameManager Awake");
    }

    private void Start()
    {
        Debug.Log($"GameManager Start - IsMasterClient: {PhotonNetwork.IsMasterClient}");

        // Solo el MasterClient decide la asignación inicial
        if (PhotonNetwork.IsMasterClient)
        {
            Invoke(nameof(AssignBombAfterDelay), 10f);
            Debug.Log("MasterClient scheduled AssignBombAfterDelay(10s)");
        }
    }

    public void RegisterPlayer(PlayerStateController player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
            Debug.Log($"Registered player: {player.name} (actor:{(player.pv.Owner != null ? player.pv.Owner.ActorNumber : -1)}) total:{players.Count}");
        }
    }

    public void UnregisterPlayer(PlayerStateController player)
    {
        if (players.Remove(player))
        {
            Debug.Log($"Unregistered player: {player.name}. remaining:{players.Count}");
        }
    }

    public void AssignBombAfterDelay()
    {
        Debug.Log("AssignBombAfterDelay called.");
        if (players.Count == 0)
        {
            Debug.LogWarning("No players registered in GameManager.players. Falling back to PhotonNetwork.PlayerList.");
            if (PhotonNetwork.PlayerList.Length > 0)
            {
                Player random = PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.PlayerList.Length)];
                photonView.RPC("RPC_AssignBomb", RpcTarget.AllBuffered, random.ActorNumber);
            }
            return;
        }

        int idx = Random.Range(0, players.Count);
        PlayerStateController chosen = players[idx];
        int actorNumber = chosen.pv.Owner != null ? chosen.pv.Owner.ActorNumber : -1;
        Debug.Log($"Chosen local player: {chosen.name}, actor: {actorNumber}");
        photonView.RPC("RPC_AssignBomb", RpcTarget.AllBuffered, actorNumber);
    }

    [PunRPC]
    private void RPC_AssignBomb(int actorNumber)
    {
        Debug.Log($"RPC_AssignBomb received on client. actorNumber={actorNumber}");
        // Recorremos los PlayerStateController que están en esta escena/local client
        var localPlayers = FindObjectsOfType<PlayerStateController>();
        foreach (var p in localPlayers)
        {
            int ownerActor = (p.pv.Owner != null) ? p.pv.Owner.ActorNumber : -1;
            Debug.Log($" - checking {p.name} ownerActor={ownerActor}");
            if (ownerActor == actorNumber)
                p.ChangeState(new BombState());
            else
                p.ChangeState(new NormalState());
        }
    }
}
