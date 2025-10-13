using Photon.Pun;
using UnityEngine;

public class BallController : MonoBehaviourPun
{
    [SerializeField] private float speed = 6f;
    private Vector2 dir;

    private void Start()
    {
        // solo el master define la dirección inicial
        if (PhotonNetwork.IsMasterClient)
        {
            float x = (Random.value < 0.5f) ? -1f : 1f;
            float y = Random.Range(-0.5f, 0.5f);
            dir = new Vector2(x, y).normalized;
        }
    }

    private void Update()
    {
        // Solo MasterClient mueve la pelota
        if (!PhotonNetwork.IsMasterClient) return;

        transform.Translate(dir * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Rebote en paredes superior/inferior (tag "Wall")
        if (collision.collider.CompareTag("Wall"))
        {
            dir.y = -dir.y;
            return;
        }

        // Rebote en paletas (tag "Paddle")
        if (collision.collider.CompareTag("Paddle"))
        {
            // invertir x y darle algo de variación según punto de impacto
            float hitFactor = (transform.position.y - collision.transform.position.y) / collision.collider.bounds.size.y;
            dir = new Vector2(-dir.x, hitFactor).normalized;
            return;
        }

        // Goals: si colisiona con Goal1 (izquierda) o Goal2 (derecha)
        if (collision.collider.CompareTag("Goal1"))
        {
            // si Goal1 está a la izquierda, el equipo 2 anota
            GameManager.Instance.AddScore(2);
            // destruir la pelota networked (se regenerará por GameManager)
            PhotonNetwork.Destroy(this.photonView);
            return;
        }

        if (collision.collider.CompareTag("Goal2"))
        {
            GameManager.Instance.AddScore(1);
            PhotonNetwork.Destroy(this.photonView);
            return;
        }
    }
}
