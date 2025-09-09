using UnityEngine;

public class NormalState : IPlayerState
{
    public void Enter(PlayerStateController player)
    {
        Debug.Log($"[{player.name}] Enter NormalState");
        player.SetColor(Color.white);
    }

    public void Exit(PlayerStateController player) { }
}
