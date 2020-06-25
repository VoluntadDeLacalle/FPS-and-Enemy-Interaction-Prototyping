using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasHit = false;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!hasHit)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity);
            transform.up = transform.forward;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Arrow")
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            hasHit = true;
        }
    }
}
