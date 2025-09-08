using UnityEngine;

public class BombState : IPlayerState
{
    public void Enter(PlayerStateController player)
    {
        player.HasBomb = true;
        player.GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void Update(PlayerStateController player)
    {
        // podría chequear colisiones o countdown
    }

    public void Exit(PlayerStateController player)
    {
        player.HasBomb = false;
        player.GetComponent<SpriteRenderer>().color = Color.white;
    }
}