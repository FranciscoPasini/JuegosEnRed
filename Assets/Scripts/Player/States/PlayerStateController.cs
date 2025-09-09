using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerStateController : MonoBehaviourPun
{
    private IPlayerState currentState;
    [HideInInspector] public PhotonView pv;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log($"[{name}] Awake. PV? {(pv != null)} SR? {(spriteRenderer != null)}");
    }

    private void Start()
    {
        // Espera a que GameManager exista (evita race condition)
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
        GameManager.Instance.RegisterPlayer(this);

        // Set estado por defecto
        ChangeState(new NormalState());
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterPlayer(this);
            Debug.Log($"[{name}] Unregistered from GameManager on destroy");
        }
    }

    public void ChangeState(IPlayerState newState)
    {
        Debug.Log($"[{name}] ChangeState -> {newState?.GetType().Name}");
        if (currentState != null) currentState.Exit(this);
        currentState = newState;
        currentState?.Enter(this);
    }

    public void SetColor(Color color)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.color = color;
        else Debug.LogWarning($"[{name}] No SpriteRenderer to change color!");
    }
}
