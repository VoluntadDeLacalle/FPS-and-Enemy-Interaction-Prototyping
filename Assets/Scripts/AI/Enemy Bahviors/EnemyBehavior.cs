using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : Enemy
{
    private NavMeshAgent nav;
    private NavMeshObstacle navObj;
    private EnemyStateMachine stateMachine;

    public float attackDistance = 2f;
    public float rotationSpeed = 2f;

    public bool isAttacking = false;
    public bool startMoving = false;
    public bool getCloser = false;
    public bool stopMoving = false;

    private Vector3 currentPlayerDestination = Vector3.zero;

    void Awake()
    {
        navObj = GetComponent<NavMeshObstacle>();
        nav = GetComponent<NavMeshAgent>();
        stateMachine = GetComponent<EnemyStateMachine>();
    }

    void OnEnable()
    {
        bool activeEnemy = false;
        for (int i = 0; i < GameManager.instance.enemies.Count; i++)
        {
            if (this == GameManager.instance.enemies[i])
            {
                activeEnemy = true;
                break;
            }
        }

        if (!activeEnemy)
        {
            GameManager.instance.enemies.Add(this);
        }
    }

    void Start()
    {
        currentPlayerDestination = GameManager.instance.player.gameObject.transform.position;
        nav.SetDestination(currentPlayerDestination);
    }

    void Update()
    {
        LookAtPlayer();
        PlayerCheck();
    }

    void LookAtPlayer()
    {
        Vector3 playerLookDirection = currentPlayerDestination - transform.position;
        playerLookDirection.y = 0;

        transform.rotation = Quaternion.LookRotation(playerLookDirection);
        
        ///Differnt look style
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerLookDirection), rotationSpeed * Time.deltaTime);
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

    public override void Chasing()
    {
        if (startMoving && !navObj.enabled)
        {
            nav.enabled = true;
            nav.SetDestination(currentPlayerDestination);

            startMoving = false;
            stopMoving = false;

            isAttacking = false;
        }

        if (!isAttacking)
        {
            CheckAttack();
        }

        if (getCloser)
        {
            foreach (var enemy in GameManager.instance.enemies)
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

    void OnDestroy()
    {
        GameManager.instance.enemies.Remove(this);

        foreach (var enemy in GameManager.instance.enemies)
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
