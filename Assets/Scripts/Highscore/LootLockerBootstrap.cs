using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootLockerBootstrap : MonoBehaviour
{
    public static bool SessionStarted {  get; private set; }
    public static string PlayerId { get; private set; }

    [SerializeField] string playerIdentifier = "";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        StartGuest();
    }

    void StartGuest()
    {
        LootLockerSDKManager.StartGuestSession(playerIdentifier, response =>
        {
            if (!response.success)
            {
                Debug.LogError("Fallo");
                return;
            }

            PlayerId = response.player_id.ToString();

            SessionStarted = true;
            Debug.Log("Conectado");
        });
    }
}
