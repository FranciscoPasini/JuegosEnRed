using UnityEngine;
using Photon.Pun;
using TMPro;

public class Chat : MonoBehaviourPunCallbacks
{
    public TMP_InputField inputFieldChat;
    public GameObject message;
    public GameObject content;

    private PhotonView pv;
    public static bool IsTyping { get; private set; }

    private void Start()
    {
        pv = GetComponent<PhotonView>();

        // Detecta cuándo se empieza o deja de escribir
        inputFieldChat.onSelect.AddListener(delegate { IsTyping = true; });
        inputFieldChat.onDeselect.AddListener(delegate { IsTyping = false; });

        // Detecta cuándo se presiona Enter dentro del campo
        inputFieldChat.onSubmit.AddListener(OnSubmitMessage);

        inputFieldChat.DeactivateInputField();
        IsTyping = false;
    }

    private void Update()
    {
        // Abre el chat con T
        if (Input.GetKeyDown(KeyCode.T))
        {
            inputFieldChat.ActivateInputField();
            IsTyping = true;
        }

        // Si presiona Escape mientras escribe, cancela el chat
        if (Input.GetKeyDown(KeyCode.Escape) && inputFieldChat.isFocused)
        {
            inputFieldChat.text = "";
            inputFieldChat.DeactivateInputField();
            IsTyping = false;
        }
    }

    private void OnSubmitMessage(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        pv.RPC("GetMessage", RpcTarget.All, $"{PhotonNetwork.NickName}: {text}");
        inputFieldChat.text = "";
        inputFieldChat.DeactivateInputField();
        IsTyping = false;
    }

    [PunRPC]
    private void GetMessage(string receivedMessage)
    {
        GameObject newMessage = Instantiate(message, Vector3.zero, Quaternion.identity, content.transform);
        newMessage.GetComponent<Message>().myMessage.text = receivedMessage;
    }
}
