using DG.Tweening;
using HongQuan;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyCtlr : Game.Entity
{
    private EnemyInfo enemyInfo;
    private Animator animator;
    private NavMeshAgent agent;
    [SerializeField] private Transform target;

    private readonly int speedRotation = 5;
    private bool dead = false;
    private bool run = false;
    private bool chase = false;
    private bool idle = true;
    private bool attack = false;
    private bool walk = false;

    [SerializeField] private float timeIdle = 5f;
    [SerializeField] private float disChase = 20f;
    [SerializeField] private float disRun = 10f;
    [SerializeField] private float disAttack = 1.5f;
    public float maxDistance = 5f;
    private Vector3 targetPosition;
    public float distance;
    public Image healthBar;
    public GameObject healthUI;

    private float countTime = 0;

    private void Awake()
    {
        enemyInfo = GetComponent<EnemyInfo>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        //player = MainMapManager.instance.playerController.transform;
    }

    private void Start()
    {
        float lv = GameManager.instance.level;
        if (lv <= 0) lv = 1;
        disChase*=lv;
        disRun*=lv;

        RandomPosition();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateValue();
        CheckTime();
        SetAnimation();
        Move();
    }

    private void Move()
    {
        if (target == null)
        {
            idle = true;
            return;
        }
        if (chase)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, speedRotation * Time.deltaTime);
            }
            agent.destination = target.position;
            agent.speed = enemyInfo.SpeedChase;
        }
        else if (run)
        {
            agent.destination = target.position;
            agent.speed = enemyInfo.SpeedRun;
        }
        else if (walk)
        {
            agent.speed = enemyInfo.SpeedWalk;
            WalkInPlace();
        }
    }

    private void UpdateValue()
    {
        if (target != null)
        {
            distance = Vector3.Distance(transform.position, target.position);
        }

        dead = enemyInfo.IsDead;
        chase = (distance <= disChase);
        run = (distance <= disRun);
        attack = (distance <= disAttack);
    }

    private void CheckTime()
    {
        if (walk)
        {
            countTime = 0;
            return;
        }

        countTime += Time.deltaTime;
        if (countTime > timeIdle)
        {
            idle = false;
            walk = true;
        }
    }

    private void WalkInPlace()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, speedRotation * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f && walk)
        {
            RandomPosition();
            walk = false;
            idle = true;
        }
    }

    private void RandomPosition()
    {
        Vector3 randomOffset = new(Random.Range(-maxDistance, maxDistance), 0f, Random.Range(-maxDistance, maxDistance));
        targetPosition = transform.position + randomOffset;
    }

    private void SetAnimation()
    {
        animator.SetBool("dead", dead);
        animator.SetBool("walk", walk);
        animator.SetBool("attack", attack);
        animator.SetBool("idle", idle);
        animator.SetBool("run", run);
        animator.SetBool("chase", chase);
    }

    IEnumerator offHealthUI;
    private void UpdateHealthUI()
    {
        if (offHealthUI != null) StopCoroutine(offHealthUI);
        offHealthUI = this.DelayFunction(2, () => healthUI.SetActive(false));
        healthUI.SetActive(true);
        healthBar.DOFillAmount(Mathf.Abs(enemyInfo.CurrentHealth / enemyInfo.MaxHealth), .2f);
    }

    private void OnEnable()
    {
        enemyInfo.Reset();
        UpdateHealthUI();
        healthUI.SetActive(false);
    }

    public void RemveChaseTarget()
    {
        target = null;
    }

    public void SetChaseTarget(Transform target)
    {
        this.target = target;
    }
}
