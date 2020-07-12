using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargerEnemyBehavior : Enemy
{
    private NavMeshAgent nav;
    private NavMeshObstacle navObj;
    private EnemyStateMachine stateMachine;

    [Header("Enemy Ranges")]
    [Tooltip("Red Gizmo Wire Sphere")]
    public float detectionRange = 0f;
    [Tooltip("Blue Gizmo Wire Sphere")]
    public float attackDistance = 0f;

    [Header("Charge Settings")]
    public float chargeTimer = 0f;
    private float maxChargeTimer = 0f;
    [Tooltip("Currently not being used for anything. Will probably implment later on.")]
    public float normalSpeed = 3.5f;
    public float chargeSpeed = 7f;
    [Tooltip("Use this to show how far past the player the enemy will charge.")]
    public float lengthPastPlayer = 1f;
    
    [Header("Enemy Interaction Settings")]
    [Tooltip("White Gizmo Wire Sphere")]
    public float sphereCastRadius = 0f;

    [Header("Player Knockback Settings")]
    public float knockbackUpwardMagnitude = 1f;
    public float knockbackForce = 1f;

    [HideInInspector]
    public bool isAttacking = false;
    [HideInInspector]
    public bool startMoving = false;

    Vector3 currentPlayerDestination = Vector3.zero;
    Vector3 chargeDestination = Vector3.zero;
    Vector3 knockbackDirectionGizmo = Vector3.zero;

    void Awake()
    {
        navObj = GetComponent<NavMeshObstacle>();
        nav = GetComponent<NavMeshAgent>();
        stateMachine = GetComponent<EnemyStateMachine>();
        stateMachine.state = EnemyStateMachine.StateType.Idle;

        maxChargeTimer = chargeTimer;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, sphereCastRadius);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(chargeDestination, chargeDestination + (Vector3.up * 4));

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + knockbackDirectionGizmo * 3);
    }

    void Start()
    {
        currentPlayerDestination = GameManager.instance.player.transform.position;
    }

    void Update()
    {
        PlayerCheck();
    }

    bool PlayerCheck()
    {
        if (Vector3.Distance(GameManager.instance.player.gameObject.transform.position, currentPlayerDestination) > (attackDistance / 2) && stateMachine.state != EnemyStateMachine.StateType.Idle)
        {
            currentPlayerDestination = GameManager.instance.player.gameObject.transform.position;

            return true;
        }

        return false;
    }

    void LookAtPlayer()
    {
        Vector3 playerLookDirection = currentPlayerDestination - transform.position;
        playerLookDirection.y = 0;

        transform.rotation = Quaternion.LookRotation(playerLookDirection);

        ///Differnt look style
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerLookDirection), rotationSpeed * Time.deltaTime);
    }

    void MoveGremlins()
    {
        RaycastHit[] hitInfos = new RaycastHit[] { };
        hitInfos = Physics.SphereCastAll(transform.position, sphereCastRadius, chargeDestination - transform.position, Vector3.Distance(transform.position, chargeDestination));

        List<GremlinEnemyBehavior> gremlinsHit = new List<GremlinEnemyBehavior>();
        for (int i = 0; i < hitInfos.Length; i++)
        {
            if (hitInfos[i].transform.gameObject.tag == "Gremlin")
            {
                gremlinsHit.Add(hitInfos[i].transform.gameObject.GetComponent<GremlinEnemyBehavior>());
            }
        }

        foreach (var gremlin in gremlinsHit)
        {
            if (gremlin.stateMachine.state != EnemyStateMachine.StateType.Flee)
            {
                gremlin.SetChasers(this.gameObject);

                gremlin.isAttacking = false;
                gremlin.navObj.enabled = false;
                gremlin.startMoving = true;
                gremlin.stopMoving = false;

                gremlin.stateMachine.switchState(EnemyStateMachine.StateType.Flee);
            }
        }
    }
    
    void FindChargeDestination()
    {
        currentPlayerDestination = GameManager.instance.player.transform.position;
        Vector3 chargeDirection = currentPlayerDestination - transform.position;
        chargeDirection.Normalize();
        chargeDestination = currentPlayerDestination + (chargeDirection * lengthPastPlayer);

        transform.rotation = Quaternion.LookRotation(chargeDirection);

        MoveGremlins();
    }

    public override void AttackEnter()
    {
        chargeTimer = maxChargeTimer;
        nav.speed = chargeSpeed;

        FindChargeDestination();
    }

    public override void Attacking()
    {
        if (Vector3.Distance(transform.position, chargeDestination) < attackDistance)
        {
            isAttacking = false;

            navObj.enabled = false;
            startMoving = true;

            stateMachine.switchState(EnemyStateMachine.StateType.Idle);
            return;
        }

        if (chargeTimer > 0)
        {
            FindChargeDestination();
        }

        chargeTimer -= Time.deltaTime;
        if (chargeTimer <= 0 && !nav.hasPath && !isAttacking)
        {
            nav.SetDestination(chargeDestination);
        }

        if (Vector3.Distance(transform.position, currentPlayerDestination) < attackDistance)
        {
            nav.ResetPath();

            //if (GameManager.instance.player.rb != null)
            //{
            //    Vector3 knockbackDirection = (currentPlayerDestination - transform.position).normalized + (Vector3.up * knockbackUpwardMagnitude);
            //    knockbackDirectionGizmo = knockbackDirection;

            //    GameManager.instance.player.rb.AddForce(knockbackDirection * knockbackForce);
            //}

            chargeTimer = maxChargeTimer;
            FindChargeDestination();
        }
    }

    public override void Idling()
    {
        if (Vector3.Distance(GameManager.instance.player.transform.position, transform.position) < detectionRange)
        {
            stateMachine.switchState(EnemyStateMachine.StateType.Attack);
        }
    }

}
