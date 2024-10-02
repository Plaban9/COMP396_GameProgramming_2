using System;

using UnityEngine;

public class NPCController_PB : MonoBehaviour
{
    public enum NpcState_PB
    {
        Patrol = 0,
        Attack,
        RunAway
    };

    private NpcState_PB _currentState;
    private GameObject _other;

    [SerializeField] private float _tickCooldown = 0.15f; //0.15s
    private float _tick;

    [Header("Temporary (Mock) Variables")]
    [SerializeField] bool bSafe;
    [SerializeField] int healthPoints = 100;
    [SerializeField] int enemyHealthPoints = 100;
    [SerializeField] private float _speed = 5f;
    [SerializeField] GameObject _enemy;
    [SerializeField] GameObject _player;
    Rigidbody _rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        _currentState = NpcState_PB.Patrol;
        _rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //_tickCooldown -= Time.deltaTime;
        //if (_tick <= 0)
        //{
            try
            {
                FSMUpdate();

                if (Input.GetKeyDown(KeyCode.R))
                {
                    FSMUpdateTest(NpcState_PB.RunAway);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error with NPC State Machine - {e}");
            }
        //    _tick = _tickCooldown;
        //}
    }

    private void FSMUpdate()
    {
        //throw new NotImplementedException();

        switch (_currentState)
        {
            case NpcState_PB.Patrol:
                HandlePatrol();
                break;
            case NpcState_PB.Attack:
                HandleAttack();
                break;
            case NpcState_PB.RunAway:
                HandleRunAway();
                break;
        }


    }

    private void FSMUpdateTest(NpcState_PB state)
    {
        //throw new NotImplementedException();

        switch (state)
        {
            case NpcState_PB.Patrol:
                HandlePatrol();
                break;
            case NpcState_PB.Attack:
                HandleAttack();
                break;
            case NpcState_PB.RunAway:
                HandleRunAway();
                break;
        }


    }

    #region STATE HANDLING

    private void ChangeState(NpcState_PB newState)
    {
        NpcState_PB lastState = _currentState;
        _currentState = newState;

        if (lastState != _currentState)
        {
            Debug.Log($"NPC changing state ({lastState} -> {_currentState})");
        }
    }

    #endregion

    #region PATROL RELATED
    private void HandlePatrol()
    {
        //throw new NotImplementedException();
        if (Threatened())
        {
            if (StrongerThanEnemy())
            {

                ChangeState(NpcState_PB.Attack);
            }
            else
            {
                ChangeState(NpcState_PB.RunAway);
            }
            return;
        }
        FollowPatrolPath();

    }


    /*
     * Progress through the given patrol path
     */
    private void FollowPatrolPath()
    {
        throw new NotImplementedException();
    }

    private bool Threatened()
    {
        return !Safe();
    }

    private bool StrongerThanEnemy()
    {
        return healthPoints >= enemyHealthPoints;
    }
    #endregion

    #region ATTACK RELATED
    private void HandleAttack()
    {
        if (WeakerThanEnemy())
        {
            ChangeState(NpcState_PB.RunAway);
        }
        else
        {
            AttackWithMelee();
        }
    }

    private void AttackWithMelee()
    {
       
    }

    private bool WeakerThanEnemy()
    {
        return !StrongerThanEnemy();
    }

    #endregion

    #region RUNAWAY RELATED
    private void HandleRunAway()
    {
        EvadeEnemy();
        if (Safe() && false) //Test
        {
            ChangeState(NpcState_PB.Patrol);
        }

    }

    private bool Safe()
    {
        return bSafe; // MOCK
    }

    private void EvadeEnemy()
    {
        print($"Evade Enemy");

        Vector3 vectorFromEnemyToPlayer = (_player.transform.position - _enemy.transform.position);
        Vector3 directionVectorFromEnemyToPlayer = vectorFromEnemyToPlayer.normalized;
        float magnitudeVectorFromEnemyToPlayer = vectorFromEnemyToPlayer.magnitude;

        var preCalculatedSpeed = _speed * Time.deltaTime;

        Debug.Log("Direction: " + directionVectorFromEnemyToPlayer);

        _rigidBody.velocity = (-1 * directionVectorFromEnemyToPlayer * preCalculatedSpeed);
        //_rigidBody.Move((-1 * directionVectorFromEnemyToPlayer * preCalculatedSpeed) + gameObject.transform.position,  + gameObject.transform.position, );
    }
    #endregion
}

