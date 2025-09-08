using UnityEngine;

public class NormalState : IPlayerState
{
    public void Enter(PlayerStateController player)
    {
        player.HasBomb = false;
        player.SetColor(Color.white);
    }

    public void Update(PlayerStateController player) { }

    public void Exit(PlayerStateController player) { }
}