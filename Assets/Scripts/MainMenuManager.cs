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
    private string nickname;
    public Button connectionButton;

    private void Start()
    {
        connectionButton.onClick.AddListener(HandleConnectButton);
        nickNameInputField.onValueChanged.AddListener(VerifyName);
    }

    private void VerifyName(string name)
    {
        if(nickNameInputField.text.Length == 0) 
        {
            connectionButton.interactable = false;
        }

        if (nickNameInputField.text.Length >= 1 && !connectionButton.interactable)
        {
            connectionButton.interactable = true;
        }

        nickname = name;
    }

    public void HandleConnectButton()
    {
        PlayerPrefs.SetString(nicknamekey, nickname);
        print(message:nickname + " is trying to conncet");
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