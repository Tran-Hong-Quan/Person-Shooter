using HongQuan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class EnemyPortal : Game.Entity, IHealth
{
    public UnityEvent<Game.Entity> onSpawnEnemy;

    [SerializeField] float maxHealt = 1000;
    [SerializeField] Image healthBar;
    [SerializeField] GameObject healthUI;
    [SerializeField] Game.Entity[] enemyPrefab;
    [SerializeField] float spawnRate = 5;
    [SerializeField] int maxEnemyToSpawn = 15;
    [SerializeField] HealthTeamSide heathTeamSide;

    [SerializeField] ParticleSystem dieEffect;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip exploseClip;

    public List<Game.Entity> enemies = new List<Game.Entity>();

    [HideInInspector]
    public float currentHealt;

    IEnumerator offHealthUI;

    public float MaxHealth => maxHealt;

    public float CurrentHealth => currentHealt;
    public GameObject GameObject => gameObject;

    public HealthTeamSide HealthTeamSide => heathTeamSide;

    private void Awake()
    {
        currentHealt = maxHealt;
    }

    private void OnEnable()
    {
        UpdateHealthUI();
        healthUI.SetActive(false);
    }

    private void UpdateHealthUI()
    {
        if (offHealthUI != null) StopCoroutine(offHealthUI);
        offHealthUI = this.DelayFunction(2, () => healthUI.SetActive(false));
        healthUI.SetActive(true);
        healthBar.DOFillAmount(Mathf.Abs(currentHealt / maxHealt), .2f);
    }

    private void Die()
    {
        onDie?.Invoke(this);
        var dieEff = Instantiate(dieEffect);
        dieEff.transform.position = transform.position;
        audioSource.PlayOneShot(exploseClip);
        Destroy(gameObject);
    }

    public void TakeDamge(float damage, HealthEventHandler caller)
    {
        currentHealt -= damage;
        //Debug.Log("Health " + currentHealt);
        UpdateHealthUI();
        if (currentHealt <= 0)
        {
            Die();
        }
    }

    public void Regeneration(float regeneration, HealthEventHandler caller)
    {
        currentHealt += regeneration;
        UpdateHealthUI();
    }
    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            if (enemyPrefab != null && enemies.Count <= maxEnemyToSpawn)
            {
                if(MainMapManager.instance.playerController == null) yield break;
                var e = SimplePool.Spawn(enemyPrefab.RandomElement(), transform.position + Vector3.up,Quaternion.identity);
                e.GetComponent<EnemyCtlr>().SetChaseTarget(MainMapManager.instance.playerController.transform);
                enemies.Add(e);
                e.onDie.AddListener(RemoveEnemy);
                onSpawnEnemy?.Invoke(e);
            }
            yield return new WaitForSeconds(spawnRate);
        }
    }

    private void RemoveEnemy(Game.Entity e)
    {
        e.onDie.RemoveListener(RemoveEnemy);
        enemies.Remove(e);
    }

    public void SetEnemy(Game.Entity[] go)
    {
        enemyPrefab = go;
        StopAllCoroutines();
        StartCoroutine(SpawnEnemy());
    }
}
