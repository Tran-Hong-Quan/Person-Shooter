using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Account : MonoBehaviour
{
    private static Account instance;
    public static Account Instance => instance;

    private readonly string ULI = "http://localhost/UnityData/Infor.php";

    [SerializeField] private string user_name;
    [SerializeField] private int high_score;
    [SerializeField] private string email;
    [SerializeField] private string date_created;

    private void Awake()
    {
        instance = this;
    }

    public void Information()
    {
        StartCoroutine(ShowInformation());
    }

    IEnumerator ShowInformation()
    {
        WWWForm form = new();
        form.AddField("userName", Login.Instance.Username.text);

        using UnityWebRequest www = UnityWebRequest.Post(ULI, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            string jsonArrayString = www.downloadHandler.text;
            LoadFormJSON(jsonArrayString);
        }
    }

    public void LoadFormJSON(string jsonString)
    {
        JsonUtility.FromJsonOverwrite(jsonString, this);
    }
}
