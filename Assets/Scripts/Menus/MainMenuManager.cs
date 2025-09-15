using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    public string gameSceneName;

    public TMP_InputField nickNameInputField;
    private const string nicknamekey = "playerNickname";
    public string nickname;

    public Button connectionButton;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;

    public GameObject mainMenuPanel;
    public GameObject roomsPanel;
    public GameObject setRoomsPanel;

    public static MainMenuManager Instance;
    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        connectionButton.onClick.AddListener(ConnectionPhoton); // en el inputfield agrega el metodo ConnectionPhoton
        nickNameInputField.onValueChanged.AddListener(VerifyName);
    }

    private void VerifyName(string name) // verifica que el nombre se pueda usar
    {
        if(nickNameInputField.text.Length == 0)  // si el nombre no tiene más de 0 letras tira error
        {
            connectionButton.interactable = false;
        }

        if (nickNameInputField.text.Length >= 1 && !connectionButton.interactable)  // si tiene 1 o más tira true
        {
            connectionButton.interactable = true;
        }

        nickname = name;  // asigna nombre
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
        SceneManager.LoadScene("Levels");
    }
    public void ConnectionPhoton() // nos conecta al master despues de ingresar el nombre
    {
        PlayerPrefs.SetString(nicknamekey, nickname);
        print(message:nickname + " is trying to connect");
        PhotonNetwork.ConnectUsingSettings();

        connectionButton.interactable = false;
    }

    public override void OnConnectedToMaster() // lo que hace ni bien nos conecta al server
    {
        Debug.Log(nickname + " is connected");
        PhotonNetwork.JoinLobby(); // Te suscribís al lobby para recibir la lista de rooms
        OpenPanel(setRoomsPanel); ; // abre el panel de setRooms
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(roomNameInputField.text, options);
    }

    public void JoinRoom (RoomInfo info) // nos une al lobby que queremos al ingresar el nombre
    {
        PhotonNetwork.JoinRoom(info.Name);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject); // limpiar lista vieja
        }

        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList) continue; // no mostrar rooms cerradas

            Instantiate(roomListItemPrefab, roomListContent)
                .GetComponent<RoomlistPrefab>()
                .SetUp(room);
        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void OpenPanel(GameObject panel)
    {
        mainMenuPanel.SetActive(false);
        roomsPanel.SetActive(false);
        setRoomsPanel.SetActive(false);

        panel.SetActive(true);
    }
}