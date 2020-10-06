using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public float damage;
    public float range;
    public Camera fpsCam;
    public GameObject bullet;
    public GameObject barrel;
    public float bulletSpeed;

    public Animator animator;
    AnimatorStateInfo stateInfo;
    public BoxCollider swordCollider;
    public bool isRaycast;

    public GameObject hitEffect;
    public float impactForce;

    public float attackStatDegredation;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(isRaycast)
                Shoot();
            
            if (!isRaycast)
            {
                animator.SetBool("isAttacking", true);
                swordCollider.enabled = true;
                FindObjectOfType<PlayerUI>().hunger -= attackStatDegredation;
                FindObjectOfType<PlayerUI>().hydration -= attackStatDegredation;
            }
        }

        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            animator.SetBool("isAttacking", false);
            swordCollider.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ShootPhysical();
        }

        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            isRaycast = !isRaycast;

            if (isRaycast)
                print("Toggled ON");
            else
                print("Toggled OFF");
        }
    }

    public void Shoot()
    {
        RaycastHit hit;
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            Target target = hit.transform.GetComponent<Target>();
            
            if(target != null)
            {
                target.TakeDamage(damage);
            }

            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            GameObject impactObject = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactObject, 2f);
        }
    }

    public void ShootPhysical()
    {
        GameObject cloneBullet = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation) as GameObject;
        cloneBullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);
        Destroy(cloneBullet, 5f);
    }

    private void OnAnimatorMove()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsTag("Attack"))
        {

            animator.ApplyBuiltinRootMotion();
        }
    }
}
