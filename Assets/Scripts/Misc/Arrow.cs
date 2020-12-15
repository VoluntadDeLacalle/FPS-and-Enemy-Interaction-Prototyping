using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Gavin Wrote This

public class Arrow : MonoBehaviour
{
    public float lifeSpan = 10;
    private float maxLifeSpan = 0;

    public float radiusOfArrow = 0;

    private Rigidbody rb;
    private bool hasHit = false;

    private Vector3 previousFramePosition;
    private Vector3 currentFramePosition;
    private Vector3 nextFramePosition;

    private Ray drawnBetweenRay;
    private Ray drawnPredictiveRay;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radiusOfArrow);

        /*Gizmos.color = Color.green;
        Gizmos.DrawRay(drawnBetweenRay);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(drawnPredictiveRay);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(previousFramePosition, .1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(currentFramePosition, .1f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(nextFramePosition, .1f);*/
    }

    void Awake()
    {
        maxLifeSpan = lifeSpan;
    }

    void Start()
    {
        previousFramePosition = transform.position;
        currentFramePosition = transform.position;
        nextFramePosition = transform.position;
    }

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        previousFramePosition = transform.position;
        currentFramePosition = transform.position;
        nextFramePosition = transform.position;

        lifeSpan = maxLifeSpan;
    }

    void Update()
    {
        if (!hasHit)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity);
            transform.up = transform.forward;

            CheckCollisions();
        }
        else
        {
            DespawnArrow();
        }
    }

    void FixedUpdate()
    {
        if (!hasHit)
        {
            previousFramePosition = currentFramePosition;
            currentFramePosition = transform.position;
            nextFramePosition = FindNextFramePosition(currentFramePosition);

            CheckCollisions();
        }
    }

    Vector3 FindNextFramePosition(Vector3 currentPosition)
    {
        float x = currentPosition.x + (rb.velocity.x * Time.fixedDeltaTime);
        float y = currentPosition.y + (rb.velocity.y * Time.fixedDeltaTime) + ((1 / 2) * -Physics.gravity.y * Time.fixedDeltaTime * Time.fixedDeltaTime);
        float z = currentPosition.z + (rb.velocity.z * Time.fixedDeltaTime);

        return new Vector3(x, y, z);
    }

    void CheckCollisions()
    {
        RaycastHit hitInfo;

        Ray betweenCollisionRay = new Ray(previousFramePosition, currentFramePosition - previousFramePosition);
        drawnBetweenRay = betweenCollisionRay;

        Ray predictiveCollisionRay = new Ray(currentFramePosition, rb.velocity);
        drawnPredictiveRay = predictiveCollisionRay;

        if ((Physics.SphereCast(betweenCollisionRay, radiusOfArrow, out hitInfo, Vector3.Distance(previousFramePosition, currentFramePosition))) || (Physics.SphereCast(predictiveCollisionRay, radiusOfArrow, out hitInfo, Vector3.Distance(currentFramePosition, nextFramePosition))))
        {
            if (hitInfo.transform.gameObject.tag != "Arrow" && hitInfo.transform.gameObject != GameManager.instance.player.gameObject)
            {
                transform.position = hitInfo.point;
                Hit(hitInfo.transform);
            }
        }
    }

    void Hit(Transform victim)
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        transform.parent = victim;

        hasHit = true;
    }

    void DespawnArrow()
    {
        lifeSpan -= Time.deltaTime;

        if (lifeSpan <= 0)
        {
            //In actual game, this would change the parent of the arrow back to the Object Pooler it came from.
            //transform.parent = null;
            transform.gameObject.SetActive(false);
        }
    }
}
