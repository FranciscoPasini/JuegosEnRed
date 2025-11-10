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
    private bool canPassBomb = true; // cooldown flag
    private float passCooldown = 2f; // 1 segundo de cooldown

    [Header("Bomb Indicator")]
    [SerializeField] private GameObject bombIndicator;

    // Referencia al movimiento para modificar velocidad
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

            // ?? aumentar velocidad si tiene bomba
            if (playerMovement != null)
                playerMovement.SetSpeedMultiplier(1.15f); // 15% más rápido
        }
        else
        {
            hasBomb = false;
            SetColor(Color.white);
            if (bombIndicator != null) bombIndicator.SetActive(false);

            // ?? restaurar velocidad normal
            if (playerMovement != null)
                playerMovement.SetSpeedMultiplier(1f);
        }
    }

    public void SetColor(Color color)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
    }

    [PunRPC]
    public void RPC_Die()
    {
        gameObject.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.CheckWinner();
            GameManager.Instance.Invoke(nameof(GameManager.Instance.AssignBombAfterDelay), 3f);
        }
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
        if (!canPassBomb) return; // ?? cooldown activo

        PlayerStateController other = otherObj.GetComponent<PlayerStateController>();
        if (other == null || other.hasBomb) return;

        if (other.pv == null || other.pv.Owner == null) return;

        int targetActor = other.pv.Owner.ActorNumber;
        pv.RPC("RPC_PassBomb", RpcTarget.AllBuffered, targetActor);

        // ?? iniciar cooldown
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
                p.ChangeState(new BombState());
            else if (p.hasBomb)
                p.ChangeState(new NormalState());
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartBombTimer(10f);
        }
    }
}
