using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Solo el MasterClient controla la asignación inicial
            Invoke(nameof(AssignBombAfterDelay), 10f);
        }
    }

    private void AssignBombAfterDelay()
    {
        if (PhotonNetwork.PlayerList.Length == 0) return;

        // Elegir jugador aleatorio
        Player randomPlayer = PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.PlayerList.Length)];

        // Llamar RPC a todos los clientes para cambiar estado
        photonView.RPC("RPC_AssignBomb", RpcTarget.AllBuffered, randomPlayer.ActorNumber);
    }

    [PunRPC]
    private void RPC_AssignBomb(int actorNumber)
    {
        // Cambiamos el estado de todos los jugadores
        PlayerStateController[] players = FindObjectsOfType<PlayerStateController>();

        foreach (var player in players)
        {
            if (player.PhotonView.Owner.ActorNumber == actorNumber)
                player.ChangeState(new BombState());
            else
                player.ChangeState(new NormalState());
        }

        Debug.Log($"Jugador {actorNumber} recibió la bomba.");
    }
}
