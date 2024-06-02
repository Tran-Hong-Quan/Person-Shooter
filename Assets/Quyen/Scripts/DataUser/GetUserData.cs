using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GetUserData : MonoBehaviour
{
    private readonly string ULI = "http://localhost/unitydata/GetUserData.php";
    private string data;
    public string Data => data;//Hold data from user

    public void GetData(string userName)
    {
        StartCoroutine(JSonData(userName));
    }

    IEnumerator JSonData(string userName)
    {
        WWWForm form = new();
        form.AddField("userName", userName);

        using UnityWebRequest www = UnityWebRequest.Post(ULI, form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            //If success
            string mess = www.downloadHandler.text;
            data = mess;
            Debug.Log(data);
        }
        www.Dispose();
    }
}
