using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Gavin Wrote This

public class TeleporterEnemyBehavior : Enemy
{
    private NavMeshAgent nav;
    private EnemyStateMachine stateMachine;

    public float teleportTimer = 8f; //Make this a random range of numbers possibly?
    private float maxTeleportTimer = 0f;
    public float teleportAwayRadius = 0f;
    public float teleportAwayCushionRadius = 0f;

    public bool onlyTeleportBehind = false;
    [Range(0, 360)] public float teleportBehindAngle = 45f;

    public float attackDistance = 2f;
    public float attackTimer = 5f;
    private float maxAttackTimer = 0f;

    private Vector3 currentPlayerDestination = Vector3.zero;
    private Transform player = null;

    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        stateMachine = GetComponent<EnemyStateMachine>();

        stateMachine.state = EnemyStateMachine.StateType.Idle;

        maxTeleportTimer = teleportTimer;
        maxAttackTimer = attackTimer;
    }

    void Start()
    {
        player = GameManager.instance.player.transform;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(currentPlayerDestination, teleportAwayRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(currentPlayerDestination, teleportAwayCushionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        if (player != null)
        {
            Gizmos.color = Color.green;
            Vector3 teleportBehindAngleA = Quaternion.AngleAxis(-teleportBehindAngle / 2, Vector3.up) * -player.forward;
            Vector3 teleportBehindAngleB = Quaternion.AngleAxis(teleportBehindAngle / 2, Vector3.up) * -player.forward;

            Gizmos.DrawLine(currentPlayerDestination, currentPlayerDestination + teleportBehindAngleA * teleportAwayCushionRadius);
            Gizmos.DrawLine(currentPlayerDestination, currentPlayerDestination + teleportBehindAngleB * teleportAwayCushionRadius);
        }
    }

    void Update()
    {
        LookAtPlayer();
        PlayerCheck();
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = transform.position;
        return false;
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
                //nav.SetDestination(currentPlayerDestination);
            }

            return true;
        }

        return false;
    }

    Vector3 FindTeleportPoint(float innerRadius, float outerRadius, float angle)
    {
        float ratio = innerRadius / outerRadius;
        float radius = Mathf.Sqrt(Random.Range(ratio * ratio, 1f)) * outerRadius;

        var dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

        return currentPlayerDestination + dir * radius;
    }

    Vector3 FindTeleportPointBehind(float innerRadius, float outerRadius, float angle)
    {
        float ratio = innerRadius / outerRadius;
        float radius = Mathf.Sqrt(Random.Range(ratio * ratio, 1f)) * outerRadius;
        int sign = Random.Range(0, 2) * 2 - 1;
        Vector3 dir = Quaternion.AngleAxis(sign * angle, Vector3.up) * -player.forward;

        return currentPlayerDestination + dir * radius;
    }

    Vector3 TeleportAway()
    {
        Vector3 newDestination = FindTeleportPoint(teleportAwayCushionRadius + attackDistance, teleportAwayRadius, Random.Range(0, 2f * Mathf.PI));

        return newDestination;
    }

    public override void IdleEnter()
    {
        transform.position = TeleportAway();

        teleportTimer = maxTeleportTimer;
    }

    public override void Idling()
    {
        teleportTimer -= Time.deltaTime;

        if (teleportTimer <= 0f)
        {
            stateMachine.switchState(EnemyStateMachine.StateType.Attack);
        }
    }

    Vector3 TeleportToPlayer()
    {
        if (onlyTeleportBehind)
        {
            return FindTeleportPointBehind(attackDistance, attackDistance, Random.Range(0, teleportBehindAngle / 2));
        }
        else
        {
            return FindTeleportPoint(attackDistance, attackDistance, Random.Range(0, 2f * Mathf.PI));
        }
    }

    public override void AttackEnter()
    {
        transform.position = TeleportToPlayer();

        attackTimer = maxAttackTimer;
    }

    public override void Attacking()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            stateMachine.switchState(EnemyStateMachine.StateType.Idle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0.0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
