using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    Enemy enemy;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    void Start()
    {
        OnStateEnter();
    }

    public enum StateType
    {
        Chase,
        Attack,
        Idle,
        Flee
    }

    public StateType state = StateType.Idle;

    void Update()
    {
        switch (state)
        {
            case StateType.Chase:
                enemy.Chasing();
                break;
            case StateType.Attack:
                enemy.Attacking();
                break;
            case StateType.Idle:
                enemy.Idling();
                break;
            case StateType.Flee:
                enemy.Fleeing();
                break;
        }
    }

    public void switchState(StateType newState)
    {
        state = newState;
        OnStateEnter();
    }

    public void OnStateEnter()
    {
        switch (state)
        {
            case StateType.Chase:
                enemy.ChaseEnter();
                break;
            case StateType.Attack:
                enemy.AttackEnter();
                break;
            case StateType.Idle:
                enemy.IdleEnter();
                break;
            case StateType.Flee:
                enemy.FleeEnter();
                break;
        }
    }
}
