using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] string leaderboardKey = "global_highscore";
    public TMPro.TextMeshProUGUI tableText;
    [SerializeField] int amountToFetch = 10;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        // Si por alguna razón no hay sesión (ej. probaste la escena directo sin pasar por MainMenu)
        if (!LootLockerBootstrap.SessionStarted)
        {
            tableText.text = "No hay sesión iniciada.";
            return;
        }

        tableText.text = "Cargando puntajes...";

        LootLockerSDKManager.GetScoreList(leaderboardKey, amountToFetch, 0, response =>
        {
            if (!response.success)
            {
                tableText.text = "Error de conexión.";
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Rank Name         Partidas ganadas");
            sb.AppendLine("-----------------------------");

            var items = response.items;

            if (items == null || items.Length == 0)
            {
                sb.AppendLine("Nadie ha jugado aún.");
            }
            else
            {
                foreach (var item in items)
                {
                    string name = string.IsNullOrEmpty(item.player.name) ? "Player " + item.player.id : item.player.name;
                    sb.AppendLine($"{item.rank,4}  {name,-16} {item.score,6}");
                }
            }

            tableText.text = sb.ToString();
        });
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnSubmitScoreTMP(TMPro.TMP_InputField scoreInput)
    {
        if (int.TryParse(scoreInput.text, out var score))
        {
            LeaderboardService.SubmitScore(score, leaderboardKey, _ => Refresh());
        }
    }

    public void OnSetNameTMP(TMPro.TMP_InputField nameInput)
    {
        PlayerNameHelper.SetPlayerName(nameInput.text);
    }



}
