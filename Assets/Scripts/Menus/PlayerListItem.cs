using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private Player player;

    public void SetUp(Player p)
    {
        player = p;
        if (text != null) text.text = p.NickName;
    }
}
