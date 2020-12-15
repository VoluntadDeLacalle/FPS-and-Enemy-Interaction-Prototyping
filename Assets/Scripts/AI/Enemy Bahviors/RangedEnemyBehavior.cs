using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

//Gavin Wrote This

public class RangedEnemyBehavior : Enemy
{
    private EnemyMovement3D agentMovement3D;
    private EnemyStateMachine stateMachine;
    private EnemyRaycastShoot raycastShoot;

    public float rotationSpeed = 2f;

    [Header("Enemy Ranges")]
    public float innerAttackRadius = 2f;
    public float outerAttackRadius = 2f;

    [Header("Altitude Range - Yellow Gizmo Line")]
    public float minAltitude = 6f;
    public float maxAltitude = 2f;

    private bool startMoving = false;
    private bool isAttacking = false;

    private Vector3 currentPlayerDestination = Vector3.zero;
    private float currentAltitude = 0;
    private float firstChaseEnter = 0f;

    List<Vector3> directionCheckList = new List<Vector3>();

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

        Gizmos.color = Color.cyan;
        foreach(Vector3 vec in directionCheckList)
        {
            Gizmos.DrawLine(currentPlayerDestination + vec, currentPlayerDestination + vec + Vector3.up * 3);
        }
    }

    void Update()
    {
        LookAtPlayer();
        PlayerCheck();
    }

    bool FindTargetPoint(float innerRadius, float outerRadius, float angle, float currentAltitude, out Vector3 agentDestination)
    {
        agentDestination = Vector3.zero;
        float ratio = innerRadius / outerRadius;
        float radius = Mathf.Sqrt(Random.Range(ratio * ratio, 1f)) * outerRadius;

        var dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        Vector3 targetPoint = currentPlayerDestination + dir * radius;
        targetPoint.y = currentAltitude;

        Vector3 playerPosition = GameManager.instance.player.transform.position;
        if (Physics.Raycast(playerPosition, (targetPoint - playerPosition).normalized, radius))
        {
            List<Vector3> differentDirs = new List<Vector3>();
            
            Vector3 originalDir = dir;
            for (int i = 0; i < 7; i++)
            {
                originalDir = Quaternion.AngleAxis(-45, Vector3.up) * originalDir;
                differentDirs.Add(originalDir);
            }

            directionCheckList = differentDirs;

            for (int i = 0; i < differentDirs.Count; i++)
            {
                int rand = Random.Range(0, differentDirs.Count);
                Vector3 currentDir = differentDirs[rand];

                Vector3 currentTargetPoint = currentPlayerDestination + currentDir * radius;
                currentTargetPoint.y = currentAltitude;
                if (Physics.Raycast(playerPosition, (currentTargetPoint - playerPosition).normalized, radius))
                {
                    differentDirs.Remove(currentDir);
                    continue;
                }
                else
                {
                    //Debug.Log("Found it!");

                    agentDestination = currentTargetPoint;
                    return true;
                }
            }
        }
        else
        {

            agentDestination = targetPoint;
            return true;
        }

        return false;
    }

    void SetTargetPlayer()
    {
        currentAltitude = Random.Range(minAltitude, maxAltitude);
        Vector3 target;
        if(FindTargetPoint(innerAttackRadius, outerAttackRadius, Random.Range(0, 2 * Mathf.PI), currentAltitude,out target))
        {
            agentMovement3D.SetDestination(target);
        }
        else
        {
            Debug.Log("Idle.");
            stateMachine.switchState(EnemyStateMachine.StateType.Idle);
        }
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
             || (Vector3.Distance(transform.position, agentMovement3D.endOfPath) < .5f && agentMovement3D.isMoving)) && !isAttacking)
        {
            agentMovement3D.Stop();

            isAttacking = true;
            stateMachine.switchState(EnemyStateMachine.StateType.Attack);
        }
    }
}
