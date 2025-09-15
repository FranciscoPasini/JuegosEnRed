using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private Transform playerListContent;
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private GameObject startPanel;

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
            AddPlayerToList(p);
        }
    }

    private void AddPlayerToList(Player player)
    {
        GameObject obj = Instantiate(playerListItemPrefab, playerListContent);
        obj.GetComponent<PlayerListItem>().SetUp(player);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Entraste a la sala: " + PhotonNetwork.CurrentRoom.Name);
        RefreshPlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " entró al server");
        AddPlayerToList(newPlayer);
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