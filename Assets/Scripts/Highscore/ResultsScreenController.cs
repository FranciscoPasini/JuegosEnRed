using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultsScreenController : MonoBehaviour
{
    [SerializeField] private LeaderboardUI leaderboardUI;

    private void Start()
    {
        // Refresca la tabla automáticamente al entrar
        if (leaderboardUI != null)
            leaderboardUI.Refresh();
    }

    // Llamado por el botón "Volver a jugar"
    public void OnPlayAgainButton()
    {
        SceneManager.LoadScene("Levels"); // Nombre de tu escena de juego
    }
}
