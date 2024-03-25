using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMapManager : MonoBehaviour
{
    [SerializeField] DemoEnemy enemyPrefab;
    [SerializeField] GenRandomObjectPerRate genRandomObject;

    private void Start()
    {
        genRandomObject.RandomSpawn(enemyPrefab.gameObject, obj =>
        {
            var enemy = obj.GetComponent<DemoEnemy>();
        });
    }
}
