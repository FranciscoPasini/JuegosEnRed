using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;

    public Transform playerListContent;
    public GameObject playerListItemPrefab;

    public GameObject startPanel;

    public void Awake()
    {
        RefreshPlayerList();
    }

    public void StartMatch()
    {
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name,spawnPoint.position,spawnPoint.rotation);
        player.GetComponent<PhotonView>().RPC("RPC_SetNickname",RpcTarget.AllBuffered,PlayerPrefs.GetString("playerNickname"));

        ClosePanel(startPanel);
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
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer); //creamos, el prefabPlayerItem, lista de nuestros player, agregamos nuestro PlayerListitem.cs y llamamos a nuestro setUp para agregar a las listas
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