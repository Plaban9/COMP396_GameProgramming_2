using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class NPCControllerAdvanced : MonoBehaviour
{
    StateMachine stateMachine;
    StateMachine.State patrol;
    StateMachine.State attack;
    StateMachine.State runaway;


    [SerializeField] private bool bSafe;
    [SerializeField] int healthPoints = 100;
    [SerializeField] int enemyHealthPoints = 100;

    private void Start()
    {
        stateMachine = new StateMachine();

        //Patrol
        patrol = stateMachine.CreateState("Patrol");
        patrol.OnEnter = delegate
        {
            Debug.Log("Patrol.OnEnter...");
        };

        patrol.OnExit = delegate
        {
            Debug.Log("Patrol.OnExit...");
        };

        patrol.OnFrame = delegate
        {
            PatrolOnFrame();
        };

        //Attack
        attack = stateMachine.CreateState("Attack");
        attack.OnEnter = delegate
        {
            Debug.Log("Attack.OnEnter...");
        };

        attack.OnExit = delegate
        {
            Debug.Log("Attack.OnExit...");
        };

        attack.OnFrame = delegate
        {
            Debug.Log("Attack.OnFrame...");
        };

        //Runaway
        runaway = stateMachine.CreateState("Runaway");
        runaway.OnEnter = delegate
        {
            Debug.Log("Runaway.OnEnter...");
        };

        runaway.OnExit = delegate
        {
            Debug.Log("Runaway.OnExit...");
        };

        runaway.OnFrame = delegate
        {
            Debug.Log("Runaway.OnFrame...");

            if (Safe())
            {
                stateMachine.TransitionTo(patrol);
            }

            EvadeEnemy();
        };

        stateMachine.ready = true;
    }
    private bool Safe()
    {
        return bSafe; // MOCK
    }

    private void EvadeEnemy()
    {
        print($"Evade Enemy");
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private bool Threatened()
    {
        return !Safe();
    }

    private bool StrongerThanEnemy()
    {
        return healthPoints >= enemyHealthPoints;
    }

    void PatrolOnFrame()
    {
        Debug.Log("Patrol.OnFrame...");

        //throw new NotImplementedException();
        if (Threatened())
        {
            if (StrongerThanEnemy())
            {

                stateMachine.TransitionTo(attack);
            }
            else
            {
                stateMachine.TransitionTo(runaway);
            }
            return;
        }
        FollowPatrolPath();
    }

    private void FollowPatrolPath()
    {

    }
}
