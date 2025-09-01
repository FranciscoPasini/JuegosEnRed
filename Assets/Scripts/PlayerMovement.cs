using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int speed = 5;
    private PhotonView photonView;
    public PhotonView PhotonView => photonView ?? GetComponent<PhotonView>();

    public void Start()
    {
       photonView = GetComponent<PhotonView>();
    }

    public void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if (PhotonView.IsMine)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector2 movement = new Vector2(horizontal, vertical);
            transform.Translate(translation: movement.normalized * speed * Time.deltaTime);
        }
    }
}
