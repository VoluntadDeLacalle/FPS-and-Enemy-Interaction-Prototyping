using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class RangedEnemyBehavior : Enemy
{
    private EnemyMovement3D agentMovement3D;
    private EnemyStateMachine stateMachine;
    private EnemyRaycastShoot raycastShoot;

    public float innerAttackRadius = 2f;
    public float outerAttackRadius = 2f;
    public float rotationSpeed = 2f;

    [Header("Altitude Range - Yellow Gizmo Line")]
    public float minAltitude = 6f;
    public float maxAltitude = 2f;

    private bool startMoving = false;
    private bool isAttacking = false;

    private Vector3 currentPlayerDestination = Vector3.zero;
    private float currentAltitude = 0;

    private float firstChaseEnter = 0f;

    void Awake()
    {
        agentMovement3D = GetComponent<EnemyMovement3D>();
        stateMachine = GetComponent<EnemyStateMachine>();
        raycastShoot = GetComponent<EnemyRaycastShoot>();
    }

    void Start()
    {
        currentPlayerDestination = GameManager.instance.player.gameObject.transform.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(currentPlayerDestination, innerAttackRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(currentPlayerDestination, outerAttackRadius);

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

    Vector3 FindTargetPoint(float innerRadius, float outerRadius, float angle)
    {
        float ratio = innerRadius / outerRadius;
        float radius = Mathf.Sqrt(Random.Range(ratio * ratio, 1f)) * outerRadius;

        var dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

        return currentPlayerDestination + dir * radius;
    }

    void SetTargetPlayer()
    {
        currentAltitude = Random.Range(minAltitude, maxAltitude);
        float targetAngle = Vector3.Angle(transform.position, currentPlayerDestination) + 90;

        Vector3 target = FindTargetPoint(innerAttackRadius, outerAttackRadius, Random.Range(0, 2 * Mathf.PI));
        target.y = currentAltitude;

        agentMovement3D.SetDestination(target);
    }

    bool PlayerCheck()
    {
        if (Vector3.Distance(GameManager.instance.player.gameObject.transform.position, currentPlayerDestination) > ((outerAttackRadius) / 2))
        {
            currentPlayerDestination = GameManager.instance.player.gameObject.transform.position;
            SetTargetPlayer();

            return true;
        }

        return false;
    }

    void LookAtPlayer()
    {
        Vector3 playerLookDirection = GameManager.instance.player.transform.position - transform.position;

        if (!agentMovement3D.isMoving)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerLookDirection), rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(playerLookDirection);
        }
    }

    public override void Attacking()
    {
        raycastShoot.Shooting();

        if (PlayerCheck())
        {
            startMoving = true;
            
            stateMachine.switchState(EnemyStateMachine.StateType.Chase);
        }
    }

    public override void ChaseEnter()
    {
        if (firstChaseEnter != 0f)
        {
            SetTargetPlayer();
        }
        else
        {
            firstChaseEnter = 1f;
        }
    }

    public override void Chasing()
    {
        if (!agentMovement3D.isMoving)
        {
            SetTargetPlayer();
        }

        if (startMoving)
        { 
            startMoving = false;
            isAttacking = false;
        }

        if ((Vector3.Distance(transform.position, currentPlayerDestination) < innerAttackRadius + (Mathf.Abs(outerAttackRadius - innerAttackRadius) / 2)
             || Vector3.Distance(transform.position, agentMovement3D.endOfPath) < .5f) && !isAttacking)
        {
            agentMovement3D.Stop();

            isAttacking = true;
            stateMachine.switchState(EnemyStateMachine.StateType.Attack);
        }
    }
}
