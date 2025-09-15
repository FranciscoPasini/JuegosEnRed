using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBombVisual : MonoBehaviour
{
    public GameObject bombIcon; // objeto encima del jugador (sprite o prefab)

    public void SetBombActive(bool active)
    {
        if (bombIcon != null)
            bombIcon.SetActive(active);
    }
}

