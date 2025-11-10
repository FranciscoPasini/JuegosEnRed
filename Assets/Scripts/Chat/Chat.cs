using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class Chat : MonoBehaviourPunCallbacks
{
    public TMP_InputField inputFieldChat;
    public GameObject message;
    public GameObject content;
    public RectTransform chatPanel;
    public ScrollRect scrollRect;

    private PhotonView pv;
    private Vector3 originalPosition;  
    private Vector3 hiddenPosition;     
    private bool chatVisible = true; 

    public static bool IsTyping { get; private set; }

    private void Start()
    {
        pv = GetComponent<PhotonView>();

        // Detecta cuándo se empieza o deja de escribir
        inputFieldChat.onSelect.AddListener(delegate { IsTyping = true; });
        inputFieldChat.onDeselect.AddListener(delegate { IsTyping = false; });

        originalPosition = chatPanel.anchoredPosition;
        hiddenPosition = originalPosition + new Vector3(-1000f, 0, 0);

        // Chat visible al inicio
        chatPanel.anchoredPosition = originalPosition;

        // Detecta cuándo se presiona Enter dentro del campo
        inputFieldChat.onSubmit.AddListener(OnSubmitMessage);

        inputFieldChat.DeactivateInputField();
        IsTyping = false;
    }

    private void Update()
    {
        // Abre el chat con T
        if (Input.GetKeyDown(KeyCode.T) && !IsTyping)
        {
            ToggleChat();
        }

        //Cierra el chat
        if (Input.GetKeyDown(KeyCode.Escape) && inputFieldChat.isFocused)
        {
            CloseChat();
        }
    }

    private void ToggleChat()
    {
        chatVisible = !chatVisible;

        if (chatVisible)
        {
            chatPanel.anchoredPosition = originalPosition;
            inputFieldChat.ActivateInputField();
            IsTyping = true;
        }
    }

    public void CloseChat()
    {
        chatVisible = false;
        inputFieldChat.DeactivateInputField();
        chatPanel.anchoredPosition = hiddenPosition;
        IsTyping = false;
    }

    public void SendMessage()
    {
        if (string.IsNullOrEmpty(inputFieldChat.text.Trim())) return;

        pv.RPC("GetMessage", RpcTarget.All, PhotonNetwork.NickName + ": " + inputFieldChat.text);
        inputFieldChat.text = "";
    }

    private void OnSubmitMessage(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        pv.RPC("GetMessage", RpcTarget.All, $"{PhotonNetwork.NickName}: {text}");
        inputFieldChat.text = "";
        inputFieldChat.ActivateInputField();
        IsTyping = true;
    }

    [PunRPC]
    private void GetMessage(string receivedMessage)
    {
        GameObject newMessage = Instantiate(message, Vector3.zero, Quaternion.identity, content.transform);
        newMessage.GetComponent<Message>().myMessage.text = receivedMessage;

        //  Actualizamos el scroll para que baje automáticamente
        Canvas.ForceUpdateCanvases(); // fuerza actualización del layout
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
