using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifeSpan = 10;
    private float maxLifeSpan = 0;

    private Rigidbody rb;
    private bool hasHit = false;

    private Vector3 previousFramePosition = Vector3.zero;
    private Vector3 currentFramePosition = Vector3.zero;

    private Ray drawnBetweenRay;
    private Ray drawnPredictiveRay;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(drawnBetweenRay);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(drawnPredictiveRay);
    }

    void Awake()
    {
        maxLifeSpan = lifeSpan;
    }

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        previousFramePosition = transform.position;
        currentFramePosition = transform.position;

        lifeSpan = maxLifeSpan;
    }

    void Update()
    {
        if (!hasHit)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity);
            transform.up = transform.forward;
        }
        else
        {
            DespawnArrow();
        }

        if (!hasHit)
        {
            CheckCollisions();
        }
    }

    void CheckCollisions()
    {
        previousFramePosition = currentFramePosition;
        currentFramePosition = transform.position;

        RaycastHit hitInfo;

        Ray betweenCollisionRay = new Ray(previousFramePosition, currentFramePosition - previousFramePosition);
        drawnBetweenRay = betweenCollisionRay;

        Ray predictiveCollisionRay = new Ray(currentFramePosition, rb.velocity);
        drawnPredictiveRay = predictiveCollisionRay;

        if ((Physics.Raycast(betweenCollisionRay, out hitInfo, Vector3.Distance(previousFramePosition, (betweenCollisionRay.origin + betweenCollisionRay.direction)))) || (Physics.Raycast(predictiveCollisionRay, out hitInfo, Vector3.Distance(currentFramePosition, (predictiveCollisionRay.origin + predictiveCollisionRay.direction)))))
        {
            if (hitInfo.transform.gameObject.tag != "Arrow" && hitInfo.transform.gameObject != GameManager.instance.player.gameObject)
            {
                transform.position = hitInfo.point;
                Hit(hitInfo.transform);
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Arrow")
        {
            Hit(other.gameObject.transform);
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
