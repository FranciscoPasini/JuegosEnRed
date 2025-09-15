using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStarter : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;

    public Transform playerListContent;
    public GameObject playerListItemPrefab;

    public GameObject startPanel;
    [SerializeField] private Button playButton;
    private bool hasSpawned = false;

    public void Awake()
    {
        RefreshPlayerList();
    }

    public void StartMatch()
    {
        if (PhotonNetwork.IsMasterClient) // solo el host puede iniciar
        {
            photonView.RPC("RPC_StartMatchForAll", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void RPC_StartMatchForAll()
    {
        if (hasSpawned) return; // evitar duplicados

        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        player.GetComponent<PhotonView>().RPC("RPC_SetNickname", RpcTarget.AllBuffered, PlayerPrefs.GetString("playerNickname"));

        ClosePanel(startPanel);

        hasSpawned = true;
        playButton.interactable = false;
    }

    private void RefreshPlayerList()
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
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Saliste de la sala");
        SceneManager.LoadScene("MainMenu");
    }

    private void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }
}
