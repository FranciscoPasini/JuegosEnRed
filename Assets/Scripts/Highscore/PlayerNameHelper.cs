using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNameHelper : MonoBehaviour
{
 
    public static void SetPlayerName(string name)
    {
        LootLockerSDKManager.SetPlayerName(name, response =>
        {
            if (!response.success) Debug.LogError("Fallo el nombre");
            else Debug.Log("Se ha puesto el nombre");
        });
    }

}
