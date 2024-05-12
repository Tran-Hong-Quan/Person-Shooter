using HongQuan;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HighScoreBoard : MonoBehaviour
{
    [SerializeField] YourHighScoreElement yourScorePrefabs;
    [SerializeField] Transform yourScoreContent;

    [SerializeField] WorldHighScoreElement worldHighScorePrefabs;
    [SerializeField] Transform worldScoreContent;

    [SerializeField] HighScoreData[] worldHighScore;
    private readonly string ULI = "https://tempquan.000webhostapp.com/GetHighScore.php";

    private void Start()
    {
        var highScores = GameManager.instance.achivementManager.highScores;
        for (int i = 0; i < highScores.data.Count; i++)
        {
            var score = highScores.data[i];
            var go = Instantiate(yourScorePrefabs);
            go.transform.SetParent(yourScoreContent, false);
            go.gameObject.SetActive(true);
            go.rank_TMP.text = "#" + (i + 1).ToString();
            go.score_TMP.text = score.ToString();
        }
    }

    IEnumerator GetText(Action<string> onDone)
    {
        UnityWebRequest www = UnityWebRequest.Get(ULI);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            onDone?.Invoke("");
        }
        else
        {
            onDone?.Invoke(www.downloadHandler.text);
        }
        www.Dispose();
    }

    private void OnEnable()
    {
        StartCoroutine(GetText((result) =>
        {
            Debug.Log(result);
            worldHighScore = JsonHelper.FromJsonArray<HighScoreData>(result);
            UpdateHighWorldHighScoreBoard();
        }));
    }

    private void UpdateHighWorldHighScoreBoard()
    {
        for (int i = 0; i < worldHighScore.Length; i++)
        {
            var go = Instantiate(worldHighScorePrefabs);
            go.transform.SetParent(worldScoreContent, false);
            go.gameObject.SetActive(true);
            go.rank_TMP.text = "#" + (i + 1).ToString();
            go.score_TMP.text = worldHighScore[i].high_score.ToString();
            go.userName_TMP.text = worldHighScore[i].user_name;
        }
    }

    [Serializable]
    public struct HighScoreData {
        public string user_name;
        public int high_score;
    }
}
