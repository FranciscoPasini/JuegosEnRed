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
    private int currentSpawnIndex = 0;
    private MainMenuManager mainMenuManager;


    private void Start()
    {
        PhotonNetwork.JoinRandomOrCreateRoom(); // al iniciar la escena crea la sala del servidor
    }

    public override void OnJoinedRoom()
    {
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name,spawnPoint.position,spawnPoint.rotation); // crea el prefab del jugador, lo instancia, le asigna la posicion, y la rotacion
        player.GetComponent<PhotonView>().RPC(methodName: "RPC_SetNickname", RpcTarget.AllBuffered, PlayerPrefs.GetString(key: "playerNickname")); //se declara el metodo que vamos a usar (RPC_SetNickname), RpcTarget es a quien le voy a mandar la información, el all significa a quienes le llegan y el buffered es que también le llegue a los jugadores que ingresaron después al servidor, y finalmente sus parámetros
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        mainMenuManager.nickname = newPlayer.NickName;
        print(message: newPlayer.NickName + " entro al server");
    }
}
