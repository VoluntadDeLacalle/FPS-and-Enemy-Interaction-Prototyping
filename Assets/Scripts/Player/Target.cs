using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float health;

    public void TakeDamage(float amount)
    {
        health -= amount;
        if(health <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            Debug.Log("Triggered by Bullet");
            TakeDamage(FindObjectOfType<Gun>().damage);
        }

        else if(other.tag == "Sword" && FindObjectOfType<Gun>().animator.GetCurrentAnimatorStateInfo(0).IsName("Sword Attack") || 
            FindObjectOfType<Gun>().animator.GetAnimatorTransitionInfo(0).IsName("Sword Idle -> Sword Attack"))
        {
            Debug.Log("Triggered by Sword");
            TakeDamage(FindObjectOfType<Gun>().damage);
        }
    }

}
