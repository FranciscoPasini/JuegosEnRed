using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombState : IPlayerState
{
    public void Enter(PlayerStateController player)
    {
        Debug.Log($"[{player.name}] Enter BombState");
        player.SetColor(Color.red);
    }

    public void Exit(PlayerStateController player)
    {
        Debug.Log($"[{player.name}] Exit BombState");
    }
}
