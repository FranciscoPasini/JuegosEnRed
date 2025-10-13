using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    [Header("Prefabs")]
    [SerializeField] private GameObject ballPrefab; // assign ball prefab

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;    // "0 - 0"
    [SerializeField] private TMP_Text infoText;     // opcional: mensajes

    private int scoreTeam1 = 0;
    private int scoreTeam2 = 0;

    private GameObject currentBall;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void BeginMatch()
    {
        // solo MasterClient decide spawnear la pelota inicial
        if (!PhotonNetwork.IsMasterClient) return;
        SpawnBall();
    }

    // MasterClient spawnea la pelota de sala - usar InstantiateRoomObject si disponible
    [PunRPC]
    private void RPC_SpawnBallNetworked()
    {
        // fallback local (no debería llamarse por no-master)
        if (currentBall != null) Destroy(currentBall);
        currentBall = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
    }

    public void SpawnBall()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Destruir pelota vieja en la red (si existe)
        if (currentBall != null)
        {
            PhotonNetwork.Destroy(currentBall.GetComponent<PhotonView>());
            currentBall = null;
        }

        // Preferible: PhotonNetwork.InstantiateRoomObject (si disponible). Si no, PhotonNetwork.Instantiate desde el master:
#if PUN_2_OR_NEWER
        // Intenta InstantiateRoomObject (recomendada para objetos de sala)
        try
        {
            currentBall = PhotonNetwork.InstantiateRoomObject(ballPrefab.name, Vector3.zero, Quaternion.identity);
        }
        catch
        {
            // fallback si no existe método o hay error
            currentBall = PhotonNetwork.Instantiate(ballPrefab.name, Vector3.zero, Quaternion.identity);
        }
#else
        currentBall = PhotonNetwork.Instantiate(ballPrefab.name, Vector3.zero, Quaternion.identity);
#endif

        // Actualizamos UI
        UpdateScoreUI();
        if (infoText != null) infoText.text = "";
    }

    // Llamar cuando un equipo anota
    public void AddScore(int team)
    {
        if (!PhotonNetwork.IsMasterClient) return; // que lo maneje el master

        if (team == 1) scoreTeam1++;
        else scoreTeam2++;

        UpdateScoreUI();

        // comprobar victoria
        if (scoreTeam1 >= 5 || scoreTeam2 >= 5)
        {
            photonView.RPC(nameof(RPC_EndGame), RpcTarget.AllBuffered, scoreTeam1, scoreTeam2);
        }
        else
        {
            // reinicia ronda tras 1s
            Invoke(nameof(SpawnBall), 1f);
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"{scoreTeam1} - {scoreTeam2}";
    }

    [PunRPC]
    private void RPC_EndGame(int s1, int s2)
    {
        // Mostrar resultado y reiniciar escena para todos (master hace LoadLevel)
        if (infoText != null)
        {
            int winner = s1 > s2 ? 1 : 2;
            infoText.text = $"¡Equipo {winner} ganó! Reiniciando...";
        }

        if (PhotonNetwork.IsMasterClient)
        {
            // reset scores
            scoreTeam1 = 0;
            scoreTeam2 = 0;
            UpdateScoreUI();

            // Cargar escena Levels para reiniciar y mostrar StartPanel otra vez
            PhotonNetwork.LoadLevel("Levels");
        }
    }
}
