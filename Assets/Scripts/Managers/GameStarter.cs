using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStarter : MonoBehaviourPunCallbacks
{
    public GameObject obstaculos;

    public static GameStarter Instance;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints; // ← CAMBIAR a array de spawn points

    public Transform playerListContent;
    public GameObject playerListItemPrefab;

    public GameObject startPanel;
    [SerializeField] private GameObject playButton;
    private bool hasSpawned = false;

    GameManager gameManager;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        obstaculos.SetActive(false);
    }

    public void Start()
    {
        RefreshPlayerList();
    }

    public void Update()
    {
        RefreshPlayerList();

        // Actualizar visibilidad del botón de inicio
        if (playButton != null)
            playButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    private void OnEnable()
    {
        if (startPanel != null)
            startPanel.SetActive(true);
    }

    public void StartMatch()
    {
        if (PhotonNetwork.IsMasterClient) // solo el host puede iniciar
        {
            photonView.RPC("RPC_StartMatchForAll", RpcTarget.AllBuffered);
            startPanel.SetActive(false);
            if (playButton != null)
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

    // MÉTODO PARA OBTENER POSICIÓN DE SPAWN
    private Vector3 GetSpawnPosition(Player player)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points defined, using default");
            return Vector3.zero;
        }

        // Usar ActorNumber para asignar spawn point único
        int playerNumber = player.ActorNumber - 1;
        int spawnIndex = playerNumber % spawnPoints.Length;

        return spawnPoints[spawnIndex].position;
    }

    [PunRPC]
    private void RPC_StartMatchForAll()
    {
        if (hasSpawned) return; // evitar duplicados

        // Obtener posición de spawn para este jugador
        Vector3 spawnPosition = GetSpawnPosition(PhotonNetwork.LocalPlayer);

        GameObject player = PhotonNetwork.Instantiate(
            playerPrefab.name,
            spawnPosition,
            Quaternion.identity
        );

        player.GetComponent<PhotonView>().RPC(
            "RPC_SetNickname",
            RpcTarget.AllBuffered,
            PlayerPrefs.GetString("playerNickname")
        );

        ClosePanel(startPanel);
        hasSpawned = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.BeginMatch();
        }
        obstaculos.SetActive(true);
    }
}