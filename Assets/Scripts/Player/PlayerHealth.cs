using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerHealth : MonoBehaviourPun
{
    public bool IsAlive { get; private set; } = true;

    void Start()
    {
        // Guardar estado inicial en CustomProperties
        SetAlive(true);
    }

    public void Die()
    {
        if (!IsAlive) return;

        Debug.Log("Jugador " + photonView.Owner.NickName + " murió.");
        SetAlive(false);

        photonView.RPC(nameof(RPC_OnDie), RpcTarget.All);
    }

    [PunRPC]
    void RPC_OnDie()
    {
        IsAlive = false;
        gameObject.SetActive(false); // simple, podés hacer animaciones o respawn
    }

    void SetAlive(bool value)
    {
        IsAlive = value;
        var props = new Hashtable();
        props["IsAlive"] = value;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
}

