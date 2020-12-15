using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Gavin Wrote This

public class Player : MonoBehaviour
{
    public Rigidbody rb = null;

    void Awake()
    {
        GameManager.instance.player = this;

        if (GetComponent<Rigidbody>() != null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }
}