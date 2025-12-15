using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardService : MonoBehaviour
{
    // Sube un puntaje directamente (reemplaza el anterior)
    public static void SubmitScore(int score, string leaderboardKey, System.Action<bool> onDone = null)
    {
        LootLockerSDKManager.SubmitScore("", score, leaderboardKey, response =>
        {
            if (!response.success)
            {
                // [CORREGIDO] Quitamos "+ response.Error" porque daba error de compilación
                Debug.LogError("Fallo al subir el score.");
                onDone?.Invoke(false);
                return;
            }
            Debug.Log("Se envio el score: " + score);
            onDone?.Invoke(true);
        });
    }

    public static void AddScore(int pointsToAdd, string leaderboardKey, System.Action<bool> onDone = null)
    {
        // Validamos que tengamos el ID antes de llamar
        if (string.IsNullOrEmpty(LootLockerBootstrap.PlayerId))
        {
            Debug.LogError("No se puede sumar puntaje: No hay PlayerID (¿No inició sesión?)");
            onDone?.Invoke(false);
            return;
        }

        // [CORREGIDO] Ahora pasamos 'LootLockerBootstrap.PlayerId' como segundo argumento
        LootLockerSDKManager.GetMemberRank(leaderboardKey, LootLockerBootstrap.PlayerId, (response) =>
        {
            int currentScore = 0;

            // Validamos response.success y que score no sea nulo
            if (response.success && response.score != 0)
            {
                currentScore = response.score;
                Debug.Log($"Puntaje anterior encontrado: {currentScore}. Sumando {pointsToAdd}...");
            }
            else
            {
                Debug.Log("Jugador nuevo o error al leer rank. Empezando de 0.");
            }

            int totalScore = currentScore + pointsToAdd;

            SubmitScore(totalScore, leaderboardKey, onDone);
        });
    }
}