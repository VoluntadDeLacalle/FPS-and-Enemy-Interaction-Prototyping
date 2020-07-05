using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyBehavior : Enemy
{
    private NavMeshAgent nav;
    private NavMeshObstacle navObj;
    private EnemyStateMachine stateMachine;
    private EnemyRaycastShoot raycastShoot;

    public float attackDistance = 2f;
    public float rotationSpeed = 2f;

    private bool startMoving = false;
    private bool isAttacking = false;

    private Vector3 currentPlayerDestination = Vector3.zero;

    void Awake()
    {
        navObj = GetComponent<NavMeshObstacle>();
        nav = GetComponent<NavMeshAgent>();
        stateMachine = GetComponent<EnemyStateMachine>();
        raycastShoot = GetComponent<EnemyRaycastShoot>();
    }

    void Start()
    {
        currentPlayerDestination = GameManager.instance.player.gameObject.transform.position;
        nav.SetDestination(currentPlayerDestination);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(gameObject.transform.position, attackDistance);
    }

    void Update()
    {
        LookAtPlayer();
        PlayerCheck();
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

    void LookAtPlayer()
    {
        Vector3 playerLookDirection = currentPlayerDestination - transform.position;
        //playerLookDirection.y = 0;

        //transform.rotation = Quaternion.LookRotation(playerLookDirection);

        ///Differnt look style
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerLookDirection), rotationSpeed * Time.deltaTime);
    }

    public override void Attacking()
    {
        raycastShoot.Shooting();

        if (PlayerCheck())
        {
            navObj.enabled = false;
            startMoving = true;
            
            stateMachine.switchState(EnemyStateMachine.StateType.Chase);
        }
    }

    public override void Chasing()
    {
        if (startMoving && !navObj.enabled)
        {
            nav.enabled = true;
            nav.SetDestination(currentPlayerDestination);

            startMoving = false;
            isAttacking = false;
        }

        if (Vector3.Distance(transform.position, currentPlayerDestination) < attackDistance && !isAttacking)
        {
            nav.enabled = false;
            navObj.enabled = true;
            isAttacking = true;

            stateMachine.switchState(EnemyStateMachine.StateType.Attack);
        }
    }
}
