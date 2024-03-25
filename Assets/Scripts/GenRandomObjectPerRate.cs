using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenRandomObjectPerRate : MonoBehaviour
{
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] float spawnRate = 2;

    [SerializeField] Transform spawnCenter;
    [SerializeField] float spawnRadious = 20;

    public void RandomSpawn(GameObject gameObject, System.Action<GameObject> onSpawn = null)
    {
        StartCoroutine(SpawnLoop(gameObject, onSpawn));
    }

    private IEnumerator SpawnLoop(GameObject gameObject, System.Action<GameObject> onSpawn)
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);

            if (!GetRandomSpawnPoint(out Vector3 spawnPoint)) continue;
            var obj = SimplePool.Spawn(gameObject);
            obj.transform.position = spawnPoint;
            obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 359), 0);
            onSpawn?.Invoke(obj);
        }
    }

    private bool GetRandomSpawnPoint(out Vector3 point)
    {
        Vector3 castCenter = spawnCenter.position + Vector3.up * 50f;
        castCenter += spawnRadious * new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        if (Physics.Raycast(castCenter, Vector3.down, out RaycastHit hitInfo, 100, groundLayerMask))
        {
            point = hitInfo.point;
            return true;
        }
        point = default;
        return false;
    }
}
