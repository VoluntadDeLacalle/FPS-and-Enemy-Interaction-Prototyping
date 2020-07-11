using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public virtual void Attacking() { }
    public virtual void AttackEnter() { }

    public virtual void Idling() { }
    public virtual void IdleEnter() { }

    public virtual void Chasing() { }
    public virtual void ChaseEnter() { }

    public virtual void Fleeing() { }
    public virtual void FleeEnter() { }
}
