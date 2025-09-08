using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro; // si usas TextMeshPro

public class BombTimer : MonoBehaviourPunCallbacks
{
    public double countdownTime = 10.0; // segundos de duración
    private double endTime;

    public TMP_Text timerText; // UI para mostrar el tiempo

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // El MasterClient define el momento de fin y lo sincroniza
            endTime = PhotonNetwork.Time + countdownTime;
            photonView.RPC("SyncTimer", RpcTarget.All, endTime);
        }
    }

    void Update()
    {
        if (endTime > 0)
        {
            double timeLeft = endTime - PhotonNetwork.Time;

            if (timeLeft > 0)
            {
                if (timerText != null)
                    timerText.text = Mathf.CeilToInt((float)timeLeft).ToString();
            }
            else
            {
                if (timerText != null)
                    timerText.text = "0";

                // Solo una vez: acción cuando el timer termina
                if (PhotonNetwork.IsMasterClient)
                {
                    OnTimerEnd();
                }

                // Desactivar para que no siga llamando
                endTime = 0;
            }
        }
    }

    [PunRPC]
    void SyncTimer(double networkEndTime)
    {
        endTime = networkEndTime;
    }

    void OnTimerEnd()
    {
        Debug.Log("? Timer terminado - hacer lógica aquí");
        // Ejemplo: elegir un jugador con la bomba, explotar, iniciar nueva ronda...
    }
}
