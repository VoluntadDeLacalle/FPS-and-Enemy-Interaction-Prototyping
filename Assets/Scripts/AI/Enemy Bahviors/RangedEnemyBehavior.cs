using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class RangedEnemyBehavior : Enemy
{
    private EnemyMovement3D navGridAgent;
    private EnemyStateMachine stateMachine;
    private EnemyRaycastShoot raycastShoot;

    public float attackDistance = 2f;
    public float rotationSpeed = 2f;

    [Header("Altitude Range - Yellow Gizmo Line")]
    public float minAltitude = 6f;
    public float maxAltitude = 2f;

    private bool startMoving = false;
    private bool isAttacking = false;

    private Vector3 currentPlayerDestination = Vector3.zero;

    void Awake()
    {
        stateMachine = GetComponent<EnemyStateMachine>();
        raycastShoot = GetComponent<EnemyRaycastShoot>();
    }

    void Start()
    {
        currentPlayerDestination = GameManager.instance.player.gameObject.transform.position;


        //navGridAgent.SetDestination()
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(gameObject.transform.position, attackDistance);

        Vector3 minAltitudeposition = new Vector3(transform.position.x, minAltitude, transform.position.z);
        Vector3 maxAltitudeposition = new Vector3(transform.position.x, maxAltitude, transform.position.z);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(minAltitudeposition, maxAltitudeposition);
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


            return true;
        }

        return false;
    }

    void LookAtPlayer()
    {
        Vector3 playerLookDirection = currentPlayerDestination - transform.position;
        
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerLookDirection), rotationSpeed * Time.deltaTime);
    }

    public override void Attacking()
    {
        raycastShoot.Shooting();

        if (PlayerCheck())
        {
            //navObj.enabled = false;
            startMoving = true;
            
            stateMachine.switchState(EnemyStateMachine.StateType.Chase);
        }
    }

    public override void Chasing()
    {
        if (startMoving)// && !navObj.enabled)
        {
            //nav.enabled = true;
            //nav.SetDestination(currentPlayerDestination);

            startMoving = false;
            isAttacking = false;
        }

        if (Vector3.Distance(transform.position, currentPlayerDestination) < attackDistance && !isAttacking)
        {
            //nav.enabled = false;
            //navObj.enabled = true;

            //transform.DOMove(transform.position + Vector3.up * upwardYAltitude, flyingUpSpeed);

            isAttacking = true;
            stateMachine.switchState(EnemyStateMachine.StateType.Attack);
        }
    }
}
