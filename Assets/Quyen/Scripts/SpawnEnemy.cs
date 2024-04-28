using HongQuan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] private Transform enemyPrefab;

    [SerializeField] private float spawnRate = 2;
    private Vector3 spawnPosition = Vector3.zero;
    private Quaternion spawnRotation = Quaternion.Euler(0, 0, 0);

    public List<Transform> enemyList = new();

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

            Transform obj = SimplePool.Spawn(enemyPrefab);
            obj.SetPositionAndRotation(spawnPosition, spawnRotation);
            enemyList.Add(obj);
        }
    }
}
