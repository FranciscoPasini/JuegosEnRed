using UnityEngine;

public class BombState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.HasBomb = true;
        player.SetColor(Color.red); // visual para indicar bomba
    }

    public void Update(PlayerController player)
    {
        // podría chequear colisiones o countdown
    }

    public void Exit(PlayerController player)
    {
        player.HasBomb = false;
    }
}