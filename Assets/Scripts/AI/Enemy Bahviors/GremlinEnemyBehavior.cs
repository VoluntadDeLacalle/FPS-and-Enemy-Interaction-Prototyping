using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GremlinEnemyBehavior : Enemy
{
    [HideInInspector]
    public NavMeshAgent nav;
    [HideInInspector]
    public NavMeshObstacle navObj;
    [HideInInspector]
    public EnemyStateMachine stateMachine;

    [Header("Enemy Ranges")]
    public float attackDistance = 2f;

    [Header("Enemy Speeds")]
    public float normalSpeed = 3.5f;
    public float fleeingSpeed = 7.5f;

    [Header("Enemy Fleeing Settings")]
    public float fleeingDestinationDistance = 2f;
    public float fleeTimer = 0f;
    private float maxFleeTimer = 0f;
    
    [HideInInspector]
    public bool isAttacking = false;
    [HideInInspector]
    public bool startMoving = false;
    [HideInInspector]
    public bool getCloser = false;
    [HideInInspector]
    public bool stopMoving = false;

    private Vector3 currentPlayerDestination = Vector3.zero;
    private Vector3 fleeDestination = Vector3.zero;

    private List<GameObject> chasers = new List<GameObject>();

    void Awake()
    {
        navObj = GetComponent<NavMeshObstacle>();
        nav = GetComponent<NavMeshAgent>();
        stateMachine = GetComponent<EnemyStateMachine>();

        maxFleeTimer = fleeTimer;
    }

    void OnEnable()
    {
        bool activeEnemy = false;
        for (int i = 0; i < GameManager.instance.gremlins.Count; i++)
        {
            if (this == GameManager.instance.gremlins[i])
            {
                activeEnemy = true;
                break;
            }
        }

        if (!activeEnemy)
        {
            GameManager.instance.gremlins.Add(this);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(fleeDestination, fleeDestination + (Vector3.up * 4));
    }

    void Start()
    {
        currentPlayerDestination = GameManager.instance.player.gameObject.transform.position;
        nav.SetDestination(currentPlayerDestination);

        SetChasers(GameManager.instance.player.gameObject);
    }

    void Update()
    {
        if (stateMachine.state != EnemyStateMachine.StateType.Flee)
        {
            PlayerCheck();
            LookAtPlayer();
        }
    }

    void LookAtPlayer()
    {
        Vector3 playerLookDirection = currentPlayerDestination - transform.position;
        playerLookDirection.y = 0;

        transform.rotation = Quaternion.LookRotation(playerLookDirection);
    }

    bool PlayerCheck()
    {
        if (Vector3.Distance(GameManager.instance.player.gameObject.transform.position, currentPlayerDestination) > (attackDistance / 2))
        {
            currentPlayerDestination = GameManager.instance.player.gameObject.transform.position;
            if (nav.enabled)
            {
                nav.SetDestination(currentPlayerDestination);
            }

            return true;
        }

        return false;
    }

    public override void Attacking()
    {
        if (PlayerCheck())
        {
            isAttacking = false;

            navObj.enabled = false;
            startMoving = true;
            stopMoving = false;

            stateMachine.switchState(EnemyStateMachine.StateType.Chase);
        }
    }

    public override void Idling()
    {
        if (PlayerCheck())
        {
            navObj.enabled = false;
            startMoving = true;
            stopMoving = false;

            stateMachine.switchState(EnemyStateMachine.StateType.Chase);
        }

        CheckAttack();
    }

    public void ResetNav(Vector3 destination)
    {
        if (startMoving && !navObj.enabled)
        {
            nav.enabled = true;
            nav.SetDestination(destination);

            startMoving = false;
            stopMoving = false;

            isAttacking = false;
        }
    }

    public override void Chasing()
    {
        ResetNav(currentPlayerDestination);

        if (!isAttacking)
        {
            CheckAttack();
        }

        if (getCloser)
        {
            foreach (var enemy in GameManager.instance.gremlins)
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) < attackDistance && enemy.navObj.enabled && enemy.isAttacking)
                {
                    nav.enabled = false;
                    navObj.enabled = true;
                    getCloser = false;
                    stopMoving = true;
                    stateMachine.switchState(EnemyStateMachine.StateType.Idle);
                }
            }

        }

        if (nav.pathStatus != NavMeshPathStatus.PathComplete && !getCloser && !stopMoving && !isAttacking)
        {
            getCloser = true;
        }
    }

    void CheckAttack()
    {
        if (Vector3.Distance(transform.position, currentPlayerDestination) < attackDistance && !isAttacking)
        {
            nav.enabled = false;
            navObj.enabled = true;
            isAttacking = true;

            stateMachine.switchState(EnemyStateMachine.StateType.Attack);
        }
    }

    public void SetChasers(GameObject chaser)
    {
        chasers.Add(chaser);
    }

    public override void FleeEnter()
    {
        nav.speed = fleeingSpeed;
        fleeTimer = maxFleeTimer;

        Vector3 fleeDir = Vector3.zero;
        for (int i = 0; i < chasers.Count; i++)
        {
            fleeDir += (transform.position - chasers[i].transform.position);
        }

        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            fleeDir = Vector3.RotateTowards(fleeDir, fleeDir + transform.right * 5, 90, 2);
        }
        else
        {
            fleeDir = Vector3.RotateTowards(fleeDir, fleeDir + (-transform.right) * 5, 90, 2);
        }

        fleeDestination = fleeDir;
        ResetNav(fleeDir);
        fleeDestination.y = nav.pathEndPosition.y;
    }

    public override void Fleeing()
    {
        fleeTimer -= Time.deltaTime;

        if (Vector3.Distance(transform.position, fleeDestination) < fleeingDestinationDistance || fleeTimer <= 0)
        {
            nav.speed = normalSpeed;
            startMoving = true;
            stateMachine.switchState(EnemyStateMachine.StateType.Chase);
        }
    }

    void OnDestroy()
    {
        GameManager.instance.gremlins.Remove(this);

        foreach (var enemy in GameManager.instance.gremlins)
        {
            if (Vector3.Distance(enemy.transform.position, currentPlayerDestination) > attackDistance)
            {
                enemy.navObj.enabled = false;
                enemy.startMoving = true;
                enemy.stopMoving = false;
            }
        }
    }
}
