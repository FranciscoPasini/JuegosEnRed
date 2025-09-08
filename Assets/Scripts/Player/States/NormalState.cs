using UnityEngine;

public class NormalState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.HasBomb = false;
        player.SetColor(Color.white);
    }

    public void Update(PlayerController player) { }

    public void Exit(PlayerController player) { }
}