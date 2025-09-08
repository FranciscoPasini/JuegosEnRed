using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(AssignBombWithDelay());
    }

    private IEnumerator AssignBombWithDelay()
    {
        yield return new WaitForSeconds(10f); // esperamos 10 segundos

        if (PhotonNetwork.PlayerList.Length == 0) yield break;

        Player randomPlayer = PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.PlayerList.Length)];
        photonView.RPC("RPC_AssignBomb", RpcTarget.AllBuffered, randomPlayer.ActorNumber);
    }

    [PunRPC]
    private void RPC_AssignBomb(int actorNumber)
    {
        // Se asegura de encontrar todos los PlayerStateController existentes
        PlayerStateController[] players = FindObjectsOfType<PlayerStateController>();

        foreach (var player in players)
        {
            if (player.photonView.Owner.ActorNumber == actorNumber)
                player.SetState(new BombState());
            else
                player.SetState(new NormalState());
        }
    }
}
