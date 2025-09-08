using UnityEngine;

public class BombState : IPlayerState
{
    public void Enter(PlayerStateController player)
    {
        // Cambiamos el color del SpriteRenderer a rojo
        player.GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void Exit(PlayerStateController player) { }
}
