using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerStateController : MonoBehaviourPun
{
    private IPlayerState currentState;
    [HideInInspector] public PhotonView pv;
    private SpriteRenderer spriteRenderer;

    private float bombTimer; // tiempo que queda si tiene bomba
    private bool hasBomb;

    [Header("Bomb Indicator")]
    [SerializeField] private GameObject bombIndicator;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log($"[{name}] Awake. PV? {(pv != null)} SR? {(spriteRenderer != null)}");

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
        {
            Debug.Log($"[{name}] Waiting for GameManager.Instance...");
            yield return null;
        }

        Debug.Log($"[{name}] Registering with GameManager. Owner: {(pv.Owner != null ? pv.Owner.NickName : "null")}, Actor: {(pv.Owner != null ? pv.Owner.ActorNumber : -1)}");
        //GameManager.Instance.RegisterPlayer(this);

        ChangeState(new NormalState());
    }

    private void Update()
    {
        if (!pv.IsMine) return; // solo controla su propio temporizador

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
        Debug.Log($"[{name}] ChangeState -> {newState?.GetType().Name}");
        currentState?.Exit(this);
        currentState = newState;
        currentState?.Enter(this);

        // Configuración automática según el estado
        if (newState is BombState)
        {
            hasBomb = true;
            bombTimer = 10f; // duración de la bomba
            SetColor(Color.red);

            if (bombIndicator != null)
                bombIndicator.SetActive(true);  // Mostrar círculo
        }
        else
        {
            hasBomb = false;
            SetColor(Color.white);

            if (bombIndicator != null)
                bombIndicator.SetActive(false); // Ocultar círculo
        }
    }

    public void SetColor(Color color)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.color = color;
    }

    [PunRPC]
    public void RPC_Die()
    {
        Debug.Log($"{name} murió con la bomba");
        gameObject.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            //GameManager.Instance.CheckWinner(); //verificar si queda solo uno
            //GameManager.Instance.Invoke(nameof(GameManager.Instance.AssignBombAfterDelay), 3f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasBomb) return; // solo el que tiene la bomba puede pasarla
        if (!pv.IsMine) return; // solo el dueño controla el traspaso

        PlayerStateController other = collision.gameObject.GetComponent<PlayerStateController>();
        if (other != null && !other.hasBomb)
        {
            // Pasar la bomba al otro jugador
            photonView.RPC("RPC_PassBomb", RpcTarget.AllBuffered, other.pv.Owner.ActorNumber);
        }
    }

    [PunRPC]
    private void RPC_PassBomb(int targetActor)
    {
        var players = FindObjectsOfType<PlayerStateController>();
        foreach (var p in players)
        {
            if (p.pv.Owner.ActorNumber == targetActor)
                p.ChangeState(new BombState());
            else if (p.hasBomb)
                p.ChangeState(new NormalState());
        }
    }
}
