using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameStarter : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<Transform> playersSpawnPoints = new List<Transform>();
    private int currentSpawnIndex = 0;

    private void Start()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name,spawnPoint.position,spawnPoint.rotation);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print(message: newPlayer.NickName + " entro al server");
    }

    /*private string GetPlayerPosition()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }*/
}
