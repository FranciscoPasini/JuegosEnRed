using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    public string gameSceneName;
    public TMP_InputField nickNameInputField;
    private const string nicknamekey = "playerNickname";
    public string nickname;
    public Button connectionButton;

    private void Start()
    {
        connectionButton.onClick.AddListener(HandleConnectButton);
        nickNameInputField.onValueChanged.AddListener(VerifyName);
    }

    private void VerifyName(string name)
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

    public void HandleConnectButton()
    {
        PlayerPrefs.SetString(nicknamekey, nickname);
        print(message:nickname + " is trying to connect");
        PhotonNetwork.ConnectUsingSettings();

        connectionButton.interactable = false;
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(nickname + " connected to server");
        SceneManager.LoadScene("Levels");
    }

    public void PlayButton()
    {
        OnConnectedToMaster();
    }
}