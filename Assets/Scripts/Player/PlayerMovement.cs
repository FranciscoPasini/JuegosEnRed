using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int speed = 5;
    [SerializeField] private TextMeshPro nickNameUI;
    private PhotonView photonView;

    private GameStarter gameStarter;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            // Se busca el gameStarter
            gameStarter = FindObjectOfType<GameStarter>();
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

            if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            {
                if (gameStarter != null && gameStarter.startPanel != null)
                {
                    bool isActive = gameStarter.startPanel.activeSelf;
                    gameStarter.startPanel.SetActive(!isActive);
                }
            }
        }
    }

    [PunRPC]
    public void RPC_SetNickname(string name)
    {
        Debug.Log("Set nickname: " + name);
        nickNameUI.text = name;
    }
}
