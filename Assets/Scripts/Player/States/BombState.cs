using UnityEngine;

public class BombState : IPlayerState
{
    public void Enter(PlayerStateController player)
    {
        player.SetColor(Color.red); // jugador con bomba
    }

    public void Exit(PlayerStateController player) { }
}