using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerStateController : MonoBehaviourPun
{
    private IPlayerState currentState;
    [HideInInspector] public PhotonView pv;
    private SpriteRenderer spriteRenderer;

    private float bombTimer;
    private bool hasBomb;
    private bool canPassBomb = true;
    private float passCooldown = 1f;

    [Header("Bomb Indicator")]
    [SerializeField] private GameObject bombIndicator;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();

        if (bombIndicator == null)
            bombIndicator = transform.Find("BombIndicator")?.gameObject;
    }

    private void Start()
    {
        StartCoroutine(RegisterWhenGameManagerReady());
    }

    private IEnumerator RegisterWhenGameManagerReady()
    {
        while (GameManager.Instance == null)
            yield return null;

        GameManager.Instance.RegisterPlayer(this);
        ChangeState(new NormalState());
    }

    private void Update()
    {
        if (!pv.IsMine) return;

        if (hasBomb)
        {
            bombTimer -= Time.deltaTime;
            if (bombTimer <= 0f)
            {
                pv.RPC("RPC_Die", RpcTarget.AllBuffered);
            }
        }
    }

    public void ChangeState(IPlayerState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState?.Enter(this);

        if (newState is BombState)
        {
            hasBomb = true;
            bombTimer = 10f;
            SetColor(Color.red);
            if (bombIndicator != null) bombIndicator.SetActive(true);

            if (playerMovement != null)
                playerMovement.SetSpeedMultiplier(1.15f);
        }
        else
        {
            hasBomb = false;
            SetColor(Color.white);
            if (bombIndicator != null) bombIndicator.SetActive(false);

            if (playerMovement != null)
                playerMovement.SetSpeedMultiplier(1f);
        }
    }
    public void Stun(float duration)
    {
        if (!pv.IsMine) return;
        StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        if (playerMovement != null)
            playerMovement.EnableMovement(false);
        SetColor(Color.magenta);

        yield return new WaitForSeconds(duration);

        if (playerMovement != null)
            playerMovement.EnableMovement(true);
        if (currentState is BombState)
            SetColor(Color.red);
        else
            SetColor(Color.white);
    }

    public void SetColor(Color color)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryHandleCollisionWith(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHandleCollisionWith(other.gameObject);
    }

    private void TryHandleCollisionWith(GameObject otherObj)
    {
        if (!hasBomb) return;
        if (!pv.IsMine) return;
        if (!canPassBomb) return;

        PlayerStateController other = otherObj.GetComponent<PlayerStateController>();
        if (other == null || other.hasBomb) return;

        if (other.pv == null || other.pv.Owner == null) return;

        int targetActor = other.pv.Owner.ActorNumber;
        pv.RPC("RPC_PassBomb", RpcTarget.AllBuffered, targetActor);

        StartCoroutine(BombPassCooldown());
    }

    private IEnumerator BombPassCooldown()
    {
        canPassBomb = false;
        yield return new WaitForSeconds(passCooldown);
        canPassBomb = true;
    }

    [PunRPC]
    private void RPC_PassBomb(int targetActor)
    {
        var players = FindObjectsOfType<PlayerStateController>();
        foreach (var p in players)
        {
            int ownerActor = (p.pv.Owner != null) ? p.pv.Owner.ActorNumber : -1;
            if (ownerActor == targetActor)
            {
                p.ChangeState(new BombState());
                p.Stun(1f);
            }
            else if (p.hasBomb)
                p.ChangeState(new NormalState());
        }

        if (hasBomb && PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.StartBombTimer(10f, pv.Owner.ActorNumber);
        }
    }

    [PunRPC]
    public void RPC_Die()
    {
        if (pv.IsMine)
        {
            ChangeState(new EliminatedState());
        }

        // 1. Desactivamos al jugador visualmente
        gameObject.SetActive(false);

        // 2. [IMPORTANTE] Todos verifican si hay un ganador
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CheckWinner();
        }

        // 3. Solo el Master se encarga de la lógica de reiniciar rondas si hay más de 1 vivo
        if (PhotonNetwork.IsMasterClient)
        {
            // Solo si quedan jugadores para seguir jugando la ronda
            // (Si CheckWinner detecta 1 solo vivo, se encarga de terminar el juego)
            GameManager.Instance.Invoke(nameof(GameManager.Instance.AssignBombAfterDelay), 3f);
        }
    }
}
