using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class PlayerStateController : MonoBehaviourPun
{
    private IPlayerState currentState;

    public PhotonView PhotonView => photonView;

    private void Start()
    {
        // Todos arrancan como normales
        ChangeState(new NormalState());
    }

    public void ChangeState(IPlayerState newState)
    {
        if (currentState != null)
            currentState.Exit(this);

        currentState = newState;

        if (currentState != null)
            currentState.Enter(this);
    }
}