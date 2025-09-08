using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int speed = 5;
    [SerializeField] private TextMeshPro nickNameUI;
    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            // Llamo solo en el mío para setear mi nombre
            photonView.RPC("RPC_SetNickname", RpcTarget.AllBuffered, PlayerPrefs.GetString("playerNickname"));
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector2 movement = new Vector2(horizontal, vertical);
            transform.Translate(movement.normalized * speed * Time.deltaTime);
        }
    }

    [PunRPC]
    public void RPC_SetNickname(string name)
    {
        Debug.Log("Set nickname: " + name);
        nickNameUI.text = name;
    }
}
