using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargerEnemyBehavior : Enemy
{
    private NavMeshAgent nav;
    private NavMeshObstacle navObj;
    private EnemyStateMachine stateMachine;

    public float detectionRange = 0f;
    public float attackDistance = 0f;

    public float chargeTimer = 0f;
    private float maxChargeTimer = 0f;
    public float normalSpeed = 3.5f;
    public float chargeSpeed = 7f;
    public float lengthPastPlayer = 1f;
    
    public float sphereCastRadius = 0f;

    public bool isAttacking = false;
    public bool startMoving = false;

    Vector3 currentPlayerDestination = Vector3.zero;
    Vector3 chargeDestination = Vector3.zero;

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
    }

    void Start()
    {
        currentPlayerDestination = GameManager.instance.player.transform.position;
    }

    void Update()
    {
        PlayerCheck();
        //LookAtPlayer();
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
        hitInfos = Physics.SphereCastAll(transform.position, sphereCastRadius, currentPlayerDestination - transform.position, Vector3.Distance(currentPlayerDestination, transform.position));

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
            gremlin.SetChasers(this.gameObject);

            gremlin.isAttacking = false;
            gremlin.navObj.enabled = false;
            gremlin.startMoving = true;
            gremlin.stopMoving = false;

            gremlin.stateMachine.switchState(EnemyStateMachine.StateType.Flee);
        }
    }
    
    void FindChargeDestination()
    {
        currentPlayerDestination = GameManager.instance.player.transform.position;
        Vector3 chargeDirection = currentPlayerDestination - transform.position;
        chargeDirection.Normalize();
        chargeDestination = currentPlayerDestination + (chargeDirection * lengthPastPlayer);

        transform.rotation = Quaternion.LookRotation(chargeDirection);
    }

    public override void AttackEnter()
    {
        chargeTimer = maxChargeTimer;
        nav.speed = chargeSpeed;

        FindChargeDestination();

        MoveGremlins();
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
            //Apply knockback to player.
            nav.ResetPath();

            chargeTimer = maxChargeTimer;
            FindChargeDestination();
        }
    }

    public void ResetNav(Vector3 destination)
    {
        if (startMoving && !navObj.enabled)
        {
            nav.enabled = true;
            nav.SetDestination(destination);

            startMoving = false;

            isAttacking = false;
        }
    }

    public override void ChaseEnter()
    {
        nav.speed = normalSpeed;
    }

    public override void Chasing()
    {
        ResetNav(currentPlayerDestination);
    }

    public override void Idling()
    {
        if (Vector3.Distance(GameManager.instance.player.transform.position, transform.position) < detectionRange)
        {
            stateMachine.switchState(EnemyStateMachine.StateType.Attack);
        }
    }

}
