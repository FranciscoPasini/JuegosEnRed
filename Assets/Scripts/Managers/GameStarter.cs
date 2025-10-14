using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStarter : MonoBehaviourPunCallbacks
{
    public static GameStarter Instance;

    [Header("Prefabs")]
    [SerializeField] private GameObject PaddlePrefab;     // prefab con PaddleController + PhotonView
    [SerializeField] private GameObject BallPrefab;       // prefab con BallController + PhotonView

    [Header("Spawns (4) - assign in inspector")]
    [SerializeField] private Transform[] spawnPoints = new Transform[4];

    [Header("UI")]
    public GameObject startPanel;
    [SerializeField] private Button playButton;

    private bool hasSpawnedLocalPaddle = false;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        // cada vez que la escena se carga, mostramos startPanel
        if (startPanel != null) startPanel.SetActive(true);

        // solo MasterClient puede iniciar partida
        if (playButton != null) playButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    // Llamado por botón Play (solo host verá y podrá clickear)
    public void StartMatch()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        // notificar a todos que inicien la partida
        photonView.RPC(nameof(RPC_StartMatchForAll), RpcTarget.AllBuffered);
    }

    // RPC: Iniciar partida para todos (cada cliente instancia su paddle)
    [PunRPC]
    private void RPC_StartMatchForAll()
    {
        // cada cliente instancia su paddle solo una vez
        if (hasSpawnedLocalPaddle) return;

        int myIndex = GetPlayerIndex();
        if (myIndex < 0) myIndex = 0; // fallback

        // clamp spawn index to 0..3 (si hay >4 jugadores, reuse indices cíclicamente)
        int spawnIndex = myIndex % 4;
        Transform spawn = spawnPoints.Length > 0 ? spawnPoints[spawnIndex] : transform;

        // PhotonNetwork.Instantiate: crea la paleta en la red
        GameObject paddle = PhotonNetwork.Instantiate(PaddlePrefab.name, spawn.position, spawn.rotation);
        // mandar datos de equipo/color al paddle (usa RPC local del paddle o SetOwnerData)
        int team = (spawnIndex % 2 == 0) ? 1 : 2; // 0->team1, 1->team2 (pero guardamos 1/2)
        paddle.GetComponent<PhotonView>().RPC("RPC_SetPlayerData", RpcTarget.AllBuffered, team, PhotonNetwork.NickName);

        hasSpawnedLocalPaddle = true;

        // cerrar UI local
        if (startPanel != null) startPanel.SetActive(false);

        // Si sos Master: pedir a GameManager que arranque (ej: spawnear pelota)
        if (PhotonNetwork.IsMasterClient && GameManager.Instance != null)
        {
            GameManager.Instance.BeginMatch();
        }
    }

    // Helper: devuelve índice del player en PlayerList en orden de PhotonNetwork.PlayerList
    private int GetPlayerIndex()
    {
        var list = PhotonNetwork.PlayerList;
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] == PhotonNetwork.LocalPlayer) return i;
        }
        return -1;
    }

    // Leave room (botón)
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");
    }
}
