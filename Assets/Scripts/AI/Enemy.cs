using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public virtual void Attacking() { }

    public virtual void Idling() { }

    public virtual void Chasing() { }
}
