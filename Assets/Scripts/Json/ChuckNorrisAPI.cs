using UnityEngine;
using UnityEngine.Networking; // Necesario para UnityWebRequest
using System.Collections;      // Necesario para Corrutinas (IEnumerator)

// --- Clase Auxiliar para parsear el JSON ---
[System.Serializable]
public class JokeData
{
    public string icon_url;
    public string id;
    public string url;
    public string value; // Este es el campo que queremos: el chiste
}


// --- Clase Principal que se adjunta a un GameObject ---
public class ChuckNorrisAPI : MonoBehaviour
{
    // URL de la API
    private const string API_URL = "https://api.chucknorris.io/jokes/random";

    // --- Este es el MÉTODO PÚBLICO que llamarás desde el Botón ---
    public void ObtenerChiste()
    {
        // --- LOG 1 ---
        Debug.Log("Botón presionado. Iniciando corrutina...");

        // Inicia la corrutina que hace la llamada a la API
        StartCoroutine(HacerLlamadaAPI());
    }

    // --- Corrutina para la llamada Web ---
    private IEnumerator HacerLlamadaAPI()
    {
        // --- LOG 2 ---
        Debug.Log("Preparando la solicitud web (WebRequest) para: " + API_URL);

        // Usamos 'using' para asegurarnos de que el request se deseche correctamente
        using (UnityWebRequest request = UnityWebRequest.Get(API_URL))
        {
            // --- LOG 3 ---
            Debug.Log("Enviando solicitud... Esperando respuesta del servidor... (Aquí puede tardar si el WiFi es lento)");

            // Enviamos la solicitud y esperamos la respuesta
            yield return request.SendWebRequest();

            // --- LOG 4 ---
            Debug.Log("¡Respuesta recibida! Verificando estado...");

            // Comprobamos si hubo un error de red o de protocolo HTTP
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                // --- LOG 5 (Error) ---
                // Si hubo error, lo mostramos en la consola de Error (en rojo)
                Debug.LogError("FALLO LA CONEXIÓN: " + request.error);
            }
            else
            {
                // --- LOG 6 (Éxito) ---
                // Si la llamada fue exitosa...
                Debug.Log("ÉXITO. Código de respuesta: " + request.responseCode);

                string jsonRespuesta = request.downloadHandler.text;

                // 1. Mostramos la respuesta JSON completa (raw) en la consola
                Debug.Log("Respuesta JSON Completa: " + jsonRespuesta);

                // 2. Parseamos el JSON para obtener solo el chiste
                JokeData chiste = JsonUtility.FromJson<JokeData>(jsonRespuesta);

                // 3. Mostramos el chiste (el campo "value") en la consola
                Debug.Log("------------------------------");
                Debug.Log("--- EL CHISTE (del JSON): ---");
                Debug.Log(chiste.value);
                Debug.Log("------------------------------");
            }
        }
    }
}