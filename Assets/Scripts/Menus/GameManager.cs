using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviourPun
{
    private List<PlayerStateController> players = new List<PlayerStateController>();

    void Start()
    {
        // Buscar jugadores al inicio
        players.AddRange(FindObjectsOfType<PlayerStateController>());

        if (PhotonNetwork.IsMasterClient)
            Invoke(nameof(AssignBombToRandomPlayer), 10f); // después de 10 segundos
    }

    void AssignBombToRandomPlayer()
    {
        if (players.Count == 0) return;

        int randomIndex = Random.Range(0, players.Count);
        photonView.RPC("RPC_AssignBomb", RpcTarget.All, players[randomIndex].photonView.ViewID);
    }

    [PunRPC]
    void RPC_AssignBomb(int playerViewId)
    {
        var player = PhotonView.Find(playerViewId).GetComponent<PlayerStateController>();

        if (player != null)
        {
            player.ChangeState(new BombState());
        }
    }
}

