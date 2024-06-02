using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateUserData : MonoBehaviour
{
    private readonly string ULI = "http://localhost/unitydata/UpdateUserData.php";

    //Call to update new data to user
    public void UpdateUser(string userName, string newData)
    {
        StartCoroutine(UpdateData(userName, newData));
    }

    IEnumerator UpdateData(string userName, string newData)
    {
        WWWForm form = new();
        form.AddField("userName", userName);
        form.AddField("newData", newData);

        using UnityWebRequest www = UnityWebRequest.Post(ULI, form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            //success
            string mess = www.downloadHandler.text;
            Debug.Log(mess);
        }
        www.Dispose();
    }
}
