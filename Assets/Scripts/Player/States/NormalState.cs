using UnityEngine;

public class NormalState : IPlayerState
{
    public void Enter(PlayerStateController player)
    {
        player.SetColor(Color.white); // jugador normal
    }

    public void Exit(PlayerStateController player) { }
}