using DG.Tweening;
using HongQuan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

public class DemoEnemy : MonoBehaviour, IHealth
{
    public float maxHealth = 100;
    public float currentHealth;
    public HealthTeamSide heathTeamSide;
    public Image healthBar;
    public GameObject healthUI;
    public Transform target;
    public Rigidbody rb;

    private NavMeshAgent agent;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public GameObject GameObject => gameObject;

    public HealthTeamSide HealthTeamSide => heathTeamSide;

    public UnityEvent<DemoEnemy> onDie;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        healthUI.SetActive(false);
        target = MainMapManager.instance.playerController.transform;
    }

    public void Regeneration(float regeneration, HealthEventHandler caller)
    {
        currentHealth += regeneration;
        UpdateHealthUI();
    }

    public void TakeDamge(float damge, HealthEventHandler caller)
    {
        currentHealth -= damge;
        UpdateHealthUI();
        if(currentHealth < 0)
        {
            Die();
        }
    }

    public void Die()
    {
        SimplePool.Despawn(gameObject);
        onDie?.Invoke(this);
        MainMapManager.instance.GetScore();

    }
    IEnumerator offHealthUI;
    private void UpdateHealthUI()
    {
        if(offHealthUI != null) StopCoroutine(offHealthUI);
        offHealthUI = this.DelayFunction(2,()=> healthUI.SetActive(false));
        healthUI.SetActive(true);
        healthBar.DOFillAmount(Mathf.Abs(currentHealth / maxHealth), .2f);
    }

    private void Update()
    {

    }

    private void OnAnimatorMove()
    {
        agent.destination = target.position;
    }

}
