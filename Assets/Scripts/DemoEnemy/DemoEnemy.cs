using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoEnemy : MonoBehaviour, IHeath
{
    public float maxHealth = 100;
    public float currentHealth;
    public Image healthBar;
    public float MaxHealth => maxHealth;

    public float CurrentHealth => currentHealth;

    private void OnEnable()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void Regeneration(float regeneration)
    {
        currentHealth += regeneration;
        UpdateHealthUI();
    }

    public void TakeDamge(float damge)
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
    }

    private void UpdateHealthUI()
    {
        healthBar.DOFillAmount(Mathf.Abs(currentHealth / maxHealth), .2f);
    }
}
