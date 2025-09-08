using UnityEngine;

public class NormalState : IPlayerState
{
    public void Enter(PlayerStateController player)
    {
        // Cambiamos el color del SpriteRenderer a blanco
        player.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void Exit(PlayerStateController player) { }
}