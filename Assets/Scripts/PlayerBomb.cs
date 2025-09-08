using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerBombPhoton : MonoBehaviourPun
{
    private BombManagerPhoton bombManager;

    void Start()
    {
        bombManager = FindObjectOfType<BombManagerPhoton>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (bombManager == null) return;

        if (bombManager.playerWithBomb == gameObject)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                bombManager.TryPassBomb(collision.gameObject);
            }
        }
    }
}

