using HongQuan;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMapManager : MonoBehaviour
{
    public static MainMapManager instance { get; private set; }

    public int score;
    public int time = 0;

    [SerializeField] TMP_Text scoreTMP;
    [SerializeField] TMP_Text timeTMP;
    [SerializeField] UIAnimation endGameBoard;
    [SerializeField] TMP_Text scoreEndGameTMP;
    [SerializeField] Game.Entity zombiePrefab;
    [SerializeField] EnemyPortal[] enemyPortals;

    public PlayerController playerController;

    public List<DemoEnemy> enemyList = new List<DemoEnemy>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InvokeRepeating(nameof(UpdateTime), 0, 1);
        InitEnemyPortal();
    }


    private void RemoveEnemyFromList(DemoEnemy enemy)
    {
        enemy.onDie.RemoveListener(RemoveEnemyFromList);
        enemyList.Remove(enemy);
    }

    public void GetScore()
    {
        score++;
        scoreTMP.text = score.ToString();
    }

    private void UpdateTime()
    {
        time++;
        timeTMP.text = time.ToString();
    }

    public void EndGame()
    {
        CancelInvoke();
        StopAllCoroutines();
        Time.timeScale = 0;
        endGameBoard.Show();
        scoreEndGameTMP.text = GameManager.instance.score.ToString();
        GameManager.instance.achivementManager.AddScore(GameManager.instance.score);
    }

    public void NextLevel()
    {
        GameManager.instance.NextLevel();
    }

    public void InitEnemyPortal()
    {
        foreach(var p in enemyPortals)
        {
            p.SetEnemy(zombiePrefab);
            p.onDie.AddListener(DestroyPortal);
        }
    }

    private void DestroyPortal(Game.Entity entity)
    {
        entity.onDie.RemoveListener(DestroyPortal);
        GetScore();
        GameManager.instance.score++;
        if(score == enemyPortals.Length)
        {
            EndGame();
        }
    }
}
