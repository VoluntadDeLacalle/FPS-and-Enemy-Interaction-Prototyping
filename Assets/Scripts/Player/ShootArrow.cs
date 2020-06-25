using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class ShootArrow : MonoBehaviour
{
    public GameObject arrowPrefab;
    public float minFirePower = 50;
    public float maxFirePower = 500;
    public float firePower;
    
    private bool canShoot;
    private Ray directionRay;
    private Rigidbody rb;
    private FirstPersonController fpController;

    public Image chargeBar;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        fpController = GetComponent<FirstPersonController>();

        chargeBar.enabled = false;
        chargeBar.fillAmount = Remap(firePower, minFirePower, maxFirePower, 0, 1);

        firePower = minFirePower;
        canShoot = true;
    }

    void Update()
    {
        directionRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        ChargeBow();

        Debug.Log(rb.velocity);
    }

    void ChargeBow()
    {
        if (Input.GetMouseButtonDown(1))
        {
            firePower = minFirePower;

            chargeBar.enabled = true;
            chargeBar.fillAmount = Remap(firePower, minFirePower, maxFirePower, 0, 1);
        }

        if (Input.GetMouseButton(1))
        {
            if (canShoot)
            {
                if (firePower > maxFirePower)
                {
                    firePower = maxFirePower;
                }
                else
                {
                    firePower += (maxFirePower / 2) * Time.deltaTime;
                }

                chargeBar.fillAmount = Remap(firePower, minFirePower, maxFirePower, 0, 1);

                if (Input.GetMouseButtonDown(0))
                {
                    Shoot();

                    firePower = minFirePower;
                    chargeBar.enabled = false;
                    canShoot = false;
                }
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            firePower = minFirePower;
            chargeBar.enabled = false;
            canShoot = true;
        }
    }

    float Remap(float value, float low1, float high1, float low2, float high2)
    {
        return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
    }

    void Shoot()
    {
        GameObject obj = Instantiate(arrowPrefab);

        Vector3 lookDirection = directionRay.direction.normalized;

        obj.transform.position = directionRay.origin;
        obj.transform.rotation = Quaternion.LookRotation(directionRay.direction);
        obj.transform.up = obj.transform.forward;

        if (fpController.m_MoveDir.x > 0 || fpController.m_MoveDir.z > 0)
        {
            if (fpController.m_IsWalking)
            {
                obj.GetComponent<Rigidbody>().AddForce(lookDirection * (firePower + fpController.m_WalkSpeed));
            }
            else
            {
                obj.GetComponent<Rigidbody>().AddForce(lookDirection * (firePower + fpController.m_RunSpeed));
            }
        }
        else
        {
            obj.GetComponent<Rigidbody>().AddForce(lookDirection * firePower);
        }

        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), obj.GetComponent<Collider>(), true);
    }



}
