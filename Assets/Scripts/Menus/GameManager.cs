using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Esperar un poquito para asegurarse de que todos los jugadores estén instanciados
            Invoke(nameof(AssignRandomBomb), 10f);
        }
    }

    private void AssignRandomBomb()
    {
        if (PhotonNetwork.PlayerList.Length == 0) return;

        // Elegir un jugador al azar
        Player randomPlayer = PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.PlayerList.Length)];

        // Mandar RPC a todos diciendo quién tiene la bomba
        photonView.RPC("RPC_GiveBomb", RpcTarget.AllBuffered, randomPlayer.ActorNumber);
    }

    [PunRPC]
    private void RPC_GiveBomb(int actorNumber)
    {
        // Buscar todos los PlayerController en la escena
        PlayerStateController[] players = FindObjectsOfType<PlayerStateController>();

        foreach (var player in players)
        {
            if (player.photonView.Owner.ActorNumber == actorNumber)
            {
                player.ChangeState(new BombState());
            }
            else
            {
                player.ChangeState(new NormalState());
            }
        }
    }
}
