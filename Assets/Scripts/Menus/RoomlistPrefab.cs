using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;

public class RoomlistPrefab : MonoBehaviour
{
    [SerializeField] TMP_Text roomName;
    RoomInfo roomInfo;
    public void SetUp(RoomInfo _roomInfo)
    {
        roomInfo = _roomInfo;
        roomName.text = _roomInfo.Name;
    }

    public void OnClick()
    {
        MainMenuManager.Instance.JoinRoom(roomInfo);
    }
}
