using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    EnemyBehavior enemy;

    void Awake()
    {
        enemy = GetComponent<EnemyBehavior>();
    }

    void Start()
    {
        OnStateEnter();
    }

    public enum StateType
    {
        Chase,
        Attack,
        Idle
    }

    public StateType state = StateType.Chase;

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

                break;
            case StateType.Attack:

                break;
            case StateType.Idle:

                break;
        }
    }
}
