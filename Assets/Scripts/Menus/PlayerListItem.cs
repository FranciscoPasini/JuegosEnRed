using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text text;
    private Player player;

    // Configurar el item con los datos del jugador
    public void SetUp(Player _player)
    {
        player = _player;
        text.text = _player.NickName;
    }

    // Cuando un jugador abandona la sala
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    // Cuando nosotros mismos salimos de la sala
    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
