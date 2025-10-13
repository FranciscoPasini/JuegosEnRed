using Photon.Pun;
using UnityEngine;

public class PaddleController : MonoBehaviourPun
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float clampY = 4.0f; // límite superior/inferior

    private int team = 1;
    private string playerName = "";

    // Datos iniciales enviados por GameStarter (RPC)
    [PunRPC]
    public void RPC_SetPlayerData(int teamAssigned, string nick)
    {
        team = teamAssigned;
        playerName = nick;

        // color por equipo (podés personalizar o usar colores elegidos)
        if (spriteRenderer != null)
        {
            spriteRenderer.color = (team == 1) ? Color.blue : Color.red;
        }

        name = $"Paddle_{playerName}_Team{team}";
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        float input = Input.GetAxisRaw("Vertical"); // W/S o flechas
        Vector3 pos = transform.position;
        pos.y += input * moveSpeed * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, -clampY, clampY);
        transform.position = pos;
    }

    // Opcional: evitar colisión entre paletas (pueden ser triggers o layer collision)
}
