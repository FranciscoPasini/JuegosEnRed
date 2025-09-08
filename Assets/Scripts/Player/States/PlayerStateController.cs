using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class PlayerStateController : MonoBehaviourPun
{
    private IPlayerState currentState;

    public void ChangeState(IPlayerState newState)
    {
        if (currentState != null)
            currentState.Exit(this);

        currentState = newState;
        currentState.Enter(this);
    }

    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }
}