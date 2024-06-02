using HongQuan;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Networking;

public class AchivementManager : MonoBehaviour
{
    public HighScores highScores;

    private readonly string uri = "https://tempquan.000webhostapp.com/UpdateHighScore.php";

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
        UpdateHighScore();
    }

    public void UpdateHighScore()
    {
        StartCoroutine(UpdateHighScoreToServer());
    }

    private IEnumerator UpdateHighScoreToServer()
    {
        WWWForm form = new WWWForm();
        if (PlayerPrefs.GetString("Username", "") == "") yield break;
        form.AddField("userName", PlayerPrefs.GetString("Username"));
        form.AddField("newScore", highScores.data[highScores.data.Count - 1].ToString());
        print(highScores.data[highScores.data.Count - 1]);
        using UnityWebRequest www = UnityWebRequest.Post(uri, form);
        yield return www.SendWebRequest();

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

    public int GetHighestScore()
    {
        int highest = 0;
        foreach(var p in data)
        {
            if(highest < p)
                highest = p;
        }
        return highest;
    }
}

