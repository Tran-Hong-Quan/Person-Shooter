using HongQuan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMapManager : MonoBehaviour
{
    public static MainMapManager instance { get; private set; }

    [SerializeField] DemoEnemy enemyPrefab;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] float spawnRate = 2;
    [SerializeField] float spawnRadious = 20;

    public PlayerController playerController;

    public List<DemoEnemy> enemyList = new List<DemoEnemy>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }
    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);
            if (enemyList.Count >= 20) continue;

            if (!Utilities.GetRandomSpawnPoint(playerController.transform.position, spawnRadious, groundLayerMask, out Vector3 spawnPoint)) continue;
            var obj = SimplePool.Spawn(enemyPrefab);
            obj.rb.velocity = Vector3.zero;
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
}
