using Photon.Pun;
using UnityEngine;

public class EliminatedState : IPlayerState
{
    GameStarter gameStarter;
    PhotonView photonView;

    public void Enter(PlayerStateController player)
    {
        Debug.Log("Enter Eliminated State");
        gameStarter = GameStarter.Instance;

        if (gameStarter != null && gameStarter.DiePanel != null)
        {
            gameStarter.DiePanel.SetActive(true);
        }
        player.gameObject.SetActive(false);
    }

    public void Update(PlayerStateController player) { }

    public void Exit(PlayerStateController player) { }
}