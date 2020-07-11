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

    public float attackDistance = 2f;
    public float normalSpeed = 3.5f;
    public float fleeingSpeed = 7.5f;
    public float fleeingDestinationDistance = 2f;

    public bool isAttacking = false;
    public bool startMoving = false;
    public bool getCloser = false;
    public bool stopMoving = false;

    private Vector3 currentPlayerDestination = Vector3.zero;

    private List<GameObject> chasers = new List<GameObject>();

    void Awake()
    {
        navObj = GetComponent<NavMeshObstacle>();
        nav = GetComponent<NavMeshAgent>();
        stateMachine = GetComponent<EnemyStateMachine>();
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

    void Start()
    {
        currentPlayerDestination = GameManager.instance.player.gameObject.transform.position;
        nav.SetDestination(currentPlayerDestination);

        SetChasers(GameManager.instance.player.gameObject);
    }

    void Update()
    {
        PlayerCheck();

        if (stateMachine.state != EnemyStateMachine.StateType.Flee)
        {
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
    }

    public override void Fleeing()
    {
        Vector3 fleeDir = Vector3.zero;
        for (int i = 0; i < chasers.Count; i++)
        {
            fleeDir += (transform.position - chasers[i].transform.position);
        }

        ResetNav(fleeDir);

        if (Vector3.Distance(transform.position, nav.pathEndPosition) < fleeingDestinationDistance)
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
