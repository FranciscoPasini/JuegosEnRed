using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStarter : MonoBehaviourPunCallbacks
{
    public static GameStarter Instance;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;

    public Transform playerListContent;
    public GameObject playerListItemPrefab;

    public GameObject startPanel;
    [SerializeField] private GameObject playButton;
    private bool hasSpawned = false;

    GameManager gameManager;
    public void Awake()
    {
        RefreshPlayerList();
    }

    public void Update()
    {
        RefreshPlayerList();
    }

    private void OnEnable()
    {
        if (startPanel != null)
            startPanel.SetActive(true);

        if (playButton != null)
            playButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void StartMatch()
    {
        if (PhotonNetwork.IsMasterClient) // solo el host puede iniciar
        {
            photonView.RPC("RPC_StartMatchForAll", RpcTarget.AllBuffered);
            startPanel.SetActive(false);
            playButton.SetActive(false);
        }  
    }

    public void RefreshPlayerList()
    {
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            OnPlayerEnteredRoom(p);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");
    }

    private void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    [PunRPC]
    private void RPC_StartMatchForAll()
    {
        if (hasSpawned) return; // evitar duplicados

        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        player.GetComponent<PhotonView>().RPC("RPC_SetNickname", RpcTarget.AllBuffered, PlayerPrefs.GetString("playerNickname"));

        ClosePanel(startPanel);

        hasSpawned = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.BeginMatch();
        }
    }
}
