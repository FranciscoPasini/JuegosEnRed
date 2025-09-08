using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    public bool HasBomb { get; set; }
    private IPlayerState currentState;
    public PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void ChangeState(IPlayerState newState)
    {
        if (currentState != null)
            currentState.Exit(this);

        currentState = newState;

        if (currentState != null)
            currentState.Enter(this);

        Debug.Log($"[{photonView.Owner.NickName}] cambió a estado {currentState.GetType().Name}");
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