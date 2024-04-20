using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    [SerializeField] private int maxHP = 10;
    [SerializeField] private int currentHP;
    [SerializeField] private int speedRun = 3;
    [SerializeField] private int speedChase = 2;
    [SerializeField] private int speedWalk = 2;
    [SerializeField] private bool isDead = false;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;
    public bool IsDead => isDead;

    public int SpeedRun => speedRun;
    public int SpeedChase => speedChase;
    public int SpeedWalk => speedWalk;

    // Start is called before the first frame update
    public void Reset()
    {
        currentHP = maxHP;
        isDead = false;
    }

    private void Start()
    {
        currentHP = maxHP;
    }

    private void Update()
    {
        CheckDead();   
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0)
        {
            Die();
        }
    }    

    public void CheckDead()
    {
        isDead = currentHP <= 0;
    }    

    public void AddHP(int value)
    {
        currentHP += value;
    }

    private void Die()
    {
        SimplePool.Despawn(gameObject);
        MainMapManager.instance.GetScore();
    }
}
