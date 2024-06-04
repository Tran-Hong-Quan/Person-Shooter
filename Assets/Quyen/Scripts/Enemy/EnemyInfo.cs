using HongQuan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour, IHealth
{
    [SerializeField] private float maxHP = 10;
    [SerializeField] private float currentHP;
    [SerializeField] private int speedRun = 3;
    [SerializeField] private int speedChase = 2;
    [SerializeField] private int speedWalk = 2;
    [SerializeField] private bool isDead = false;

    //Hong Quan
    [SerializeField] float attackRange = 1;
    [SerializeField] Vector2 attackCenter = Vector2.one;
    [SerializeField] float attackDamge = 10;
    [SerializeField] private HealthTeamSide heathTeamSide;

    private EnemyCtlr enemyCtlr;

    public bool IsDead => isDead;

    public int SpeedRun => speedRun;
    public int SpeedChase => speedChase;
    public int SpeedWalk => speedWalk;

    public float MaxHealth => maxHP;

    public float CurrentHealth => currentHP;
    public GameObject GameObject => gameObject;

    public HealthTeamSide HealthTeamSide => heathTeamSide;

    private void Awake()
    {
        enemyCtlr = GetComponent<EnemyCtlr>();
    }

    // Start is called before the first frame update
    public void Reset()
    {
        currentHP = maxHP;
        isDead = false;
    }

    private void Start()
    {
        int lv = GameManager.instance.level;
        if (lv <= 0) lv = 1;
        maxHP *= lv;
        speedRun *= lv;
        speedChase*=lv;
        attackDamge *= lv * 2 + 1;

        currentHP = maxHP;
    }

    private void Update()
    {
        CheckDead();
    }

    public void CheckDead()
    {
        isDead = currentHP <= 0;
    }

    public void AddHP(float value)
    {
        currentHP += value;
    }

    private void Die()
    {
        SimplePool.Despawn(gameObject);
        enemyCtlr.onDie?.Invoke(enemyCtlr);
    }

    public void TakeDamge(float damage, HealthEventHandler caller)
    {
        if (caller.teamSide == HealthTeamSide) return;
        currentHP -= damage;
        if (currentHP < 0)
        {
            Die();
        }
    }

    public void Regeneration(float regeneration, HealthEventHandler caller)
    {
        AddHP(regeneration);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position +
            transform.forward * attackCenter.x + new Vector3(0, attackCenter.y),
            attackRange);
    }

    //Hong Quan
    private void OnAttackAnimation()
    {
        var cols = Physics.OverlapSphere(transform.position +
            transform.forward * attackCenter.x + new Vector3(0, attackCenter.y), attackRange);
        foreach (var col in cols)
        {
            if (!col.TryGetComponent(out IHealth haveHealthEntity)) continue;

            haveHealthEntity.TakeDamge(attackDamge, new HealthEventHandler(gameObject, heathTeamSide));
        }
    }
}
