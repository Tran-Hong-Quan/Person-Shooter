using HongQuan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AchivementManager : MonoBehaviour
{
    public HighScores highScores;

    private readonly string uri;

    private void Awake()
    {
        highScores = JsonUtility.FromJson<HighScores>(Utilities.LoadData(Globle.HighScoreDataName));
    }

    public void AddScore(int score)
    {
        if (highScores.data == null) highScores.data = new List<int>();
        highScores.data.Add(score);
        highScores.data.Sort();
        Utilities.SaveData(JsonUtility.ToJson(highScores), Globle.HighScoreDataName);
        StartCoroutine(UpdateHighScoreToServer());
    }

    private IEnumerator UpdateHighScoreToServer()
    {
        WWWForm form = new WWWForm();
        if (PlayerPrefs.GetString("Username", "") == "") yield break;
        form.AddField("user_name", PlayerPrefs.GetString("UserName"));
        form.AddField("high_score", highScores.data[highScores.data.Count - 1].ToString());

        using UnityWebRequest www = UnityWebRequest.Post(uri, form);
        yield return www.SendWebRequest();
        MainMenu.instance.loadingUI.gameObject.SetActive(false);

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
        }
    }
}

[System.Serializable]
public struct HighScores
{
    public List<int> data;
}

