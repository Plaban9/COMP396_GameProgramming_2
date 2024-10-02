using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static NPCController_Modified_PB;

public class NPCController_Advanced_Modified_PB : MonoBehaviour
{
    #region STATES
    private StateMachine _stateMachine;

    private StateMachine.State _patrol;
    private StateMachine.State _suspicious;
    private StateMachine.State _alert;
    private StateMachine.State _chase;
    private StateMachine.State _attack;
    private StateMachine.State _search;
    private StateMachine.State _escape;
    private StateMachine.State _wait;
    private StateMachine.State _howl;
    #endregion

    [Header("GENERAL ATTRIBUTES")]
    [SerializeField] private string _currentState;
    [SerializeField] private int _health = 100;
    [SerializeField] private int _level = 10;
    [SerializeField] private int _viewDistance = 10;
    [SerializeField] private float _currentSpeed = 0f;
    [SerializeField] private float _viewAngleInDegrees = 45f; // i.e; from centre of vision half of the angle on each side
    [SerializeField] private GameObject _player;
    [SerializeField] private Vector3 _playerLastKnownPosition;

    [Header("PATROL")]
    [SerializeField] private float _patrolSpeed = 6f;
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private int _targetWaypointIndex = 0;

    [Header("SUSPICIOUS")]
    [SerializeField] private float _timeToConfirmInSecs = 1.5f;
    private float _detectedTime;

    [Header("ALERT")]
    [SerializeField] private float _alertSpeed = 5f;

    [Header("CHASE")]
    [SerializeField] private float _chaseSpeed = 8f;

    [Header("ATTACK")]
    [SerializeField] private float _attackDistanceThreshold = 4f;
    private float _startAttackTime;
    [SerializeField] private float _attackInterval = 2f;

    [Header("SEARCH")]
    [SerializeField] private float _searchSpeed = 3f;
    private float _searchStartTime;
    [SerializeField] private float _searchDurationInSecs = 5f;

    [Header("ESCAPE")]
    [SerializeField] private float _escapeSpeed = 7f;
    [SerializeField] private float _escapeDistanceThreshold = 20f;

    [Header("WAIT")]
    [SerializeField] private float _waitTimerInSecs = 5f;
    private float _startWaitTime;


    [Header("HOWL")]
    [SerializeField] private float _howlRadius = 35f;
    [SerializeField] private LayerMask _layerToHowl;

    [Header("DEBUG/TEST")]
    [SerializeField] private int _playerLevel = 10;
    [SerializeField] private int _playerHP = 100;

    // Start is called before the first frame update
    void Start()
    {
        CreateStates();
        _stateMachine.ready = true;
    }

    private void CreateStates()
    {
        _stateMachine = new StateMachine();

        //Patrol
        _patrol = _stateMachine.CreateState("Patrol");
        _patrol.OnEnter = delegate
        {
            D("Patrol.OnEnter...");

            SetSpeed(_patrolSpeed);
        };

        _patrol.OnExit = delegate
        {
            D("Patrol.OnExit...");
        };

        _patrol.OnFrame = delegate
        {
            D("Patrol.OnFrame...");

            HandlePatrolState();
        };

        //Attack
        _attack = _stateMachine.CreateState("Attack");
        _attack.OnEnter = delegate
        {
            D("Attack.OnEnter...");
        };

        _attack.OnExit = delegate
        {
            D("Attack.OnExit...");
        };

        _attack.OnFrame = delegate
        {
            D("Attack.OnFrame...");

            HandleAttackState();
        };

        //Escape
        _escape = _stateMachine.CreateState("Escape");
        _escape.OnEnter = delegate
        {
            D("Escape.OnEnter...");

            SetSpeed(_escapeSpeed);
        };

        _escape.OnExit = delegate
        {
            D("Escape.OnExit...");
        };

        _escape.OnFrame = delegate
        {
            D("Escape.OnFrame...");

            HandleEscapeState();
        };

        //Suspicious
        _suspicious = _stateMachine.CreateState("Suspicious");
        _suspicious.OnEnter = delegate
        {
            D("Suspicious.OnEnter...");
        };

        _suspicious.OnExit = delegate
        {
            D("Suspicious.OnExit...");
        };

        _suspicious.OnFrame = delegate
        {
            D("Suspicious.OnFrame...");

            HandleSuspiciousState();
        };


        //Alert
        _alert = _stateMachine.CreateState("Alert");
        _alert.OnEnter = delegate
        {
            D("Alert.OnEnter...");

            SetSpeed(_alertSpeed);
        };

        _alert.OnExit = delegate
        {
            D("Alert.OnExit...");
        };

        _alert.OnFrame = delegate
        {
            D("Alert.OnFrame...");

            HandleAlertState();
        };


        //Chase
        _chase = _stateMachine.CreateState("Chase");
        _chase.OnEnter = delegate
        {
            D("Chase.OnEnter...");

            SetSpeed(_chaseSpeed);
        };

        _chase.OnExit = delegate
        {
            D("Chase.OnExit...");
        };

        _chase.OnFrame = delegate
        {
            D("Chase.OnFrame...");

            HandleChaseState();
        };


        //Search
        _search = _stateMachine.CreateState("Search");
        _search.OnEnter = delegate
        {
            D("Search.OnEnter...");

            SetSpeed(_searchSpeed);
        };

        _search.OnExit = delegate
        {
            D("Search.OnExit...");
        };

        _search.OnFrame = delegate
        {
            D("Search.OnFrame...");

            HandleSearchState();
        };


        //Wait
        _wait = _stateMachine.CreateState("Wait");
        _wait.OnEnter = delegate
        {
            D("Wait.OnEnter...");
        };

        _wait.OnExit = delegate
        {
            D("Wait.OnExit...");
        };

        _wait.OnFrame = delegate
        {
            D("Wait.OnFrame...");

            HandleWaitState();
        };


        //Howl
        _howl = _stateMachine.CreateState("Howl");

        _howl.OnEnter = delegate
        {
            D("Howl.OnEnter...");
        };

        _howl.OnExit = delegate
        {
            D("Howl.OnExit...");
        };

        _howl.OnFrame = delegate
        {
            D("Howl.OnFrame...");

            HandleHowlState();
        };
    }


    // Update is called once per frame
    void Update()
    {
        UpdateStateMachine();
    }

    private void UpdateStateMachine()
    {
        try
        {
            _currentState = _stateMachine?.CurrentState?.ToString();
            _stateMachine.Update();
        }
        catch (Exception exception)
        {
            D($"ERROR! in FSMUpdate: {exception}", true);
        }
    }

    private void SetSpeed(float speed)
    {
        _currentSpeed = speed;
    }

    #region Helpers
    private bool IsPlayerWithinSight()
    {
        return Vector3.Distance(transform.position, _player.transform.position) < _viewDistance;
    }
    #endregion

    #region STATE HANDLERS
    #region PATROL
    private void HandlePatrolState()
    {
        FollowPath();

        if (IsPlayerWithinSight())
        {
            _playerLastKnownPosition = _player.transform.position;
            _detectedTime = Time.time;
            _stateMachine.TransitionTo(_suspicious);
        }
    }

    private void FollowPath()
    {
        if (Vector3.Distance(transform.position, _waypoints[_targetWaypointIndex].position) < 1.5f)
        {
            _targetWaypointIndex = (_targetWaypointIndex + 1) % _waypoints.Length;
        }

        transform.position = Vector3.MoveTowards(transform.position, _waypoints[_targetWaypointIndex].position, _currentSpeed * Time.deltaTime);
    }
    #endregion

    #region SUSPICIOUS
    private void HandleSuspiciousState()
    {
        if (_timeToConfirmInSecs + _detectedTime > Time.time)
        {
            return;
        }

        _stateMachine.TransitionTo(_howl);
    }
    #endregion

    #region ALERT
    private void HandleAlertState()
    {
        if (IsPlayerWithinSight())
        {
            _playerLastKnownPosition = _player.transform.position;

            var isBetterThanPlayer = _level >= _playerLevel && _health >= _playerHP;

            if (isBetterThanPlayer)
            {
                _stateMachine.TransitionTo(_chase);
            }
            else
            {
                _stateMachine.TransitionTo(_escape);
            }
        }
        else
        {
            _stateMachine.TransitionTo(_search);
        }
    }
    #endregion

    #region CHASE
    private void HandleChaseState()
    {
        var distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
        var canAttack = (distanceToPlayer <= _attackDistanceThreshold);

        if (IsPlayerWithinSight())
        {
            _playerLastKnownPosition = _player.transform.position;

            if (canAttack)
            {
                _stateMachine.TransitionTo(_attack);
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _currentSpeed * Time.deltaTime);
            return;
        }

        _stateMachine.TransitionTo(_search);
    }
    #endregion

    #region ATTACK
    private void HandleAttackState()
    {
        if (IsPlayerWithinSight())
        {
            _playerLastKnownPosition = _player.transform.position;

            var distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

            if (distanceToPlayer <= _attackDistanceThreshold)
            {
                if (_attackInterval + _startAttackTime < Time.time)
                {
                    D("ATTACK");
                    _startAttackTime = Time.time;
                }
            }
            else
            {
                _stateMachine.TransitionTo(_chase);
            }

            return;
        }

        _stateMachine.TransitionTo(_alert);
    }
    #endregion

    #region SEARCH
    private void HandleSearchState()
    {
        if (IsPlayerWithinSight())
        {
            _playerLastKnownPosition = _player.transform.position;

            var isBetterThanPlayer = _level >= _playerLevel && _health >= _playerHP;

            if (isBetterThanPlayer)
            {
                _stateMachine.TransitionTo(_chase);

                return;
            }
            else
            {
                _stateMachine.TransitionTo(_escape);

                return;
            }
        }

        if (Vector3.Distance(transform.position, _playerLastKnownPosition) > 1.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _playerLastKnownPosition, _currentSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _playerLastKnownPosition) < 1.5f)
            {
                D("Start Search");
                _searchStartTime = Time.time;
            }

            return;
        }

        if (Vector3.Distance(transform.position, _playerLastKnownPosition) < 1.5f)
        {
            if (_searchDurationInSecs + _searchStartTime < Time.time)
            {
                _stateMachine.TransitionTo(_patrol);

                return;
            }
        }
    }
    #endregion

    #region ESCAPE
    private void HandleEscapeState()
    {
        if (IsPlayerWithinSight())
        {
            _playerLastKnownPosition = _player.transform.position;
        }

        var distanceFromLastKnownPlayerPosition = Vector3.Distance(transform.position, _playerLastKnownPosition);

        if (distanceFromLastKnownPlayerPosition > _escapeDistanceThreshold)
        {
            _startWaitTime = Time.time;
            _stateMachine.TransitionTo(_wait);
            return;
        }

        var directionVectorFromEnemyToPlayer = (transform.position - _playerLastKnownPosition).normalized;
        var destination = directionVectorFromEnemyToPlayer.normalized * _escapeDistanceThreshold;

        transform.position = Vector3.MoveTowards(transform.position, destination, _currentSpeed * Time.deltaTime);
    }
    #endregion

    #region WAIT
    private void HandleWaitState()
    {
        if (IsPlayerWithinSight())
        {
            _stateMachine.TransitionTo(_escape);

            return;
        }

        if (_waitTimerInSecs + _startWaitTime < Time.time)
        {
            D("Wait Over");

            _stateMachine.TransitionTo(_patrol);
        }
    }
    #endregion

    #region HOWL
    private void HandleHowlState()
    {
        var npcs = Physics.OverlapSphere(transform.position, _howlRadius, _layerToHowl);

        foreach (var npc in npcs)
        {
            if (npc.TryGetComponent(out NPCController_Advanced_Modified_PB npc_ai))
            {
                npc_ai.OnHowlNotified(gameObject, _playerLastKnownPosition);
            }
        }

        _stateMachine.TransitionTo(_alert);
    }

    public void OnHowlNotified(GameObject gameObject, Vector3 lastKnownLocation)
    {
        D($"Howl notified by {gameObject.name}");

        if (_currentState.Equals("Patrol") || _currentState.Equals("Suspicious") || _currentState.Equals("Search"))
        {
            _playerLastKnownPosition = lastKnownLocation;
            _stateMachine.TransitionTo(_alert);
        }
    }
    #endregion
    #endregion

    private void D(string message, bool isError = false)
    {
        if (isError)
        {
            Debug.LogError($"<<{gameObject.name}>> <<NPCController_Modified_PB>> {message}");
            return;
        }

        Debug.Log($"<<{gameObject.name}>> <<NPCController_Modified_PB>> {message}");
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = gameObject.name.Contains("Coward") ? Color.white : Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _howlRadius);
    }
}
