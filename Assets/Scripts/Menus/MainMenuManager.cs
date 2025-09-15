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
        connectionButton.onClick.AddListener(OnJoinedRoom); // en el inputfield agrega el metodo OnJoinedRoom
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

    public override void OnJoinedRoom() //habilita que se pueda ingresar a photon
    {
        if (connectionButton.interactable == true)
        {
            ConnectionPhoton();
        }

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
        OpenPanel(setRoomsPanel); ; // abre el panel de setRooms
        Debug.Log(nickname + " is connected");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        SceneManager.LoadScene("Levels");
    }

    public void JoinRoom (RoomInfo info) // nos une al lobby que queremos al ingresar el nombre
    {
        PhotonNetwork.JoinRoom(info.Name);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform transform in roomListContent)  //actualiza la lista borrando
        {
            Destroy(transform.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++) 
        {
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomlistPrefab>().SetUp(roomList[i]); //creamos, el prefabRoonItem, lista de nuestras rooms, agregamos nuestro Roomlistprefab.cs y llamamos a nuestro setUp para agregar a las listas
        }
    }

    public void ExitButton()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void OpenPanel(GameObject panel)
    {
        mainMenuPanel.SetActive(false);
        roomsPanel.SetActive(false);
        setRoomsPanel.SetActive(false);

        panel.SetActive(true);
    }
}