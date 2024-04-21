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
    public int time = 60;

    [SerializeField] DemoEnemy enemyPrefab;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] float spawnRate = 2;
    [SerializeField] float spawnRadious = 20;
    [SerializeField] public int enemyCount;

    [SerializeField] TMP_Text scoreTMP;
    [SerializeField] TMP_Text timeTMP;
    [SerializeField] UIAnimation endGameBoard;
    [SerializeField] TMP_Text scoreEndGameTMP;

    [SerializeField] Transform spawnPosCenter;

    public PlayerController playerController;

    public List<DemoEnemy> enemyList = new List<DemoEnemy>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InvokeRepeating(nameof(UpdateTime), 0, 1);
        StartCoroutine(SpawnLoop());

    }
    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);
            if (enemyList.Count >= 20) continue;
            if(enemyCount <= 0) yield break;
            enemyCount--;

            if (!Utilities.GetRandomSpawnPoint(spawnPosCenter.position, spawnRadious, groundLayerMask, out Vector3 spawnPoint)) continue;
            var obj = SimplePool.Spawn(enemyPrefab);
            //obj.rb.velocity = Vector3.zero;
            obj.transform.position = spawnPoint;
            obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 359), 0);
            obj.target = playerController.transform;
            obj.onDie.AddListener(RemoveEnemyFromList); 
            enemyList.Add(obj);
        }
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
        scoreEndGameTMP.text = score.ToString();
        GameManager.instance.achivementManager.AddScore(score);
    }

    public void PlayAgain()
    {
        GameManager.instance.transition.FullScreen(() => 
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("MainMap");
        }, null);
    }
}
