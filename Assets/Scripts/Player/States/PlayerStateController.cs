using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class PlayerStateController : MonoBehaviourPun
{
    private IPlayerState currentState;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Todos empiezan normales
        SetState(new NormalState());
    }

    public void SetState(IPlayerState newState)
    {
        if (currentState != null)
            currentState.Exit(this);

        currentState = newState;

        if (currentState != null)
            currentState.Enter(this);
    }
}