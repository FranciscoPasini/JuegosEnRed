using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameStarter : MonoBehaviourPunCallbacks
{
    public static GameStarter Instance;
    public GameObject disconnectpanel;
    private MainMenuManager mainMenuManager;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints; // 🔹 Array con 12 spawn points
    [SerializeField] private GameObject diePanel;
    public GameObject DiePanel => diePanel;
    public Transform playerListContent;
    public GameObject playerListItemPrefab;
    public GameObject startPanel;
    [SerializeField] private GameObject playButton;
    private bool hasSpawned = false;

    private static List<int> usedSpawnIndexes = new List<int>(); // 🔹 Para no repetir spawns

    private void Awake()
    {
        Instance = this;
        RefreshPlayerList();
    }

    private void Update()
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
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_StartMatchForAll", RpcTarget.AllBuffered);
            startPanel.SetActive(false);
            playButton.SetActive(false);
        }
    }

    public void RefreshPlayerList()
    {
        foreach (Transform child in playerListContent)
            Destroy(child.gameObject);

        foreach (Player p in PhotonNetwork.PlayerList)
            OnPlayerEnteredRoom(p);
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
    public void DisconnectionBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public override void OnDisconnected(DisconnectCause cause) // accion ni bien desconecta
    {
        Debug.Log("Desconectado de Photon: " + cause);
        mainMenuManager.OpenPanel(disconnectpanel);
    }
    [PunRPC]
    private void RPC_StartMatchForAll()
    {
        if (hasSpawned) return;

        // 🔹 Elegimos un spawn libre de forma aleatoria
        int spawnIndex = GetUniqueRandomSpawnIndex();
        Transform spawn = spawnPoints[spawnIndex];

        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawn.position, spawn.rotation);
        player.GetComponent<PhotonView>().RPC("RPC_SetNickname", RpcTarget.AllBuffered, PlayerPrefs.GetString("playerNickname"));

        ClosePanel(startPanel);
        hasSpawned = true;

        if (GameManager.Instance != null)
            GameManager.Instance.BeginMatch();
    }

    // 🔹 Devuelve un índice no repetido de spawn aleatorio
    private int GetUniqueRandomSpawnIndex()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned in GameStarter!");
            return 0;
        }

        // Si ya usamos todos, reseteamos (por seguridad)
        if (usedSpawnIndexes.Count >= spawnPoints.Length)
            usedSpawnIndexes.Clear();

        int index;
        do
        {
            index = Random.Range(0, spawnPoints.Length);
        }
        while (usedSpawnIndexes.Contains(index));

        usedSpawnIndexes.Add(index);
        return index;
    }
}
