using HongQuan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchivementManager : MonoBehaviour
{
    public HighScores highScores;

    public void AddScore(int score)
    {
        if (highScores.data == null) highScores.data = new List<int>();
        highScores.data.Add(score);
        highScores.data.Sort();
        Utilities.SaveData(JsonUtility.ToJson(highScores), Globle.HighScoreDataName);
    }
}

[System.Serializable]
public struct HighScores
{
    public List<int> data;
}

