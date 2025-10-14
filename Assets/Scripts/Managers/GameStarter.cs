using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStarter : MonoBehaviourPunCallbacks
{
    public static GameStarter Instance;

    [Header("Prefabs")]
    [SerializeField] private GameObject paddlePrefab;
    [SerializeField] private GameObject ballPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("UI")]
    public GameObject startPanel;
    [SerializeField] private Button playButton;
    private bool hasSpawned = false;

    private void Awake()
    {
        Instance = this;

        // Carga de respaldo automática (por si Photon resetea las referencias)
        if (paddlePrefab == null)
            paddlePrefab = Resources.Load<GameObject>("PaddlePrefab");

        if (ballPrefab == null)
            ballPrefab = Resources.Load<GameObject>("BallPrefab");
    }

    public void StartMatch()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_StartMatchForAll), RpcTarget.AllBuffered);
            startPanel.SetActive(false);
            playButton.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    private void RPC_StartMatchForAll()
    {
        if (hasSpawned) return;

        if (paddlePrefab == null)
        {
            Debug.LogError(" PaddlePrefab no asignado, ni encontrado en Resources.");
            return;
        }

        int index = PhotonNetwork.LocalPlayer.ActorNumber % spawnPoints.Length;
        Transform spawn = spawnPoints[index];

        GameObject paddle = PhotonNetwork.Instantiate(paddlePrefab.name, spawn.position, spawn.rotation);
        paddle.GetComponent<PhotonView>().RPC("RPC_SetNickname", RpcTarget.AllBuffered, PlayerPrefs.GetString("playerNickname"));

        if (PhotonNetwork.IsMasterClient && ballPrefab != null)
        {
            PhotonNetwork.Instantiate(ballPrefab.name, Vector3.zero, Quaternion.identity);
        }

        hasSpawned = true;
    }
}
