using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GetUserData : MonoBehaviour
{
    private readonly string ULI = "https://tempquan.000webhostapp.com/GetUserData.php";
    private string data;
    public string Data => data;//Hold data from user

    public void GetData(string userName,System.Action<string> onSuccess)
    {
        StartCoroutine(JSonData(userName,onSuccess));
    }

    IEnumerator JSonData(string userName, System.Action<string> onSucces)
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
            onSucces?.Invoke(data);
            Debug.Log(data);
        }
        www.Dispose();
    }
}
