using HongQuan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreBoard : MonoBehaviour
{
    [SerializeField] YourHighScoreElement yourScorePrefabs;
    [SerializeField] Transform yourScoreContent;
    private void Start()
    {
        var highScores = JsonUtility.FromJson<HighScores>(Utilities.LoadData(Globle.HighScoreDataName));
        for (int i = 0; i < highScores.data.Count; i++)
        {
            var score = highScores.data[i];
            var go = Instantiate(yourScorePrefabs);
            go.gameObject.SetActive(true);
            go.rank_TMP.text = "#" + (i + 1).ToString();
            go.score_TMP.text = score.ToString();
        }
    }
}
