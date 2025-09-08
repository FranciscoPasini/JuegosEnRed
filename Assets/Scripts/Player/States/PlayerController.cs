using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool HasBomb { get; set; }
    private IPlayerState currentState;

    public void ChangeState(IPlayerState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

    private void Update()
    {
        currentState?.Update(this);
    }

    public void SetColor(Color c)
    {
        GetComponent<SpriteRenderer>().color = c;
    }
}