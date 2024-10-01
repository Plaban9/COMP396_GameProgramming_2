using AI.FactoryDesign;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace AI.Simple_FSM
{
    public class NPCController_FSM_PB : MonoBehaviour
    {
        public enum NPC_STATE_PB
        {
            PATROL,
            SUSPICIOUS,
            HOWL,
            ALERT,
            CHASE,
            ATTACK,
            SEARCH,
            ESCAPE,
            WAIT
        };

        [Header("GENERAL ATTRIBUTES")]
        [SerializeField] private NPC_STATE_PB _currentState;
        [SerializeField] private int _health = 100;
        [SerializeField] private int _level = 5;
        [SerializeField] private int _viewDistance = 50;
        [SerializeField] private float _currentSpeed;
        [SerializeField] private float _viewAngleInDegrees = 45f; // i.e; from centre of vision half of the angle on each side
        [SerializeField] private GameObject _player;
        [SerializeField] private Vector3 _playerLastKnownPosition;

        [Header("HOWL")]
        [SerializeField] private float _howlRadius = 35f;
        [SerializeField] private LayerMask _layerToHowl;

        [Header("PATROL")]
        [SerializeField] private float _patrolSpeed = 5f;
        [SerializeField] private Transform[] _waypoints;
        [SerializeField] private int _targetWaypointIndex = 0;

        [Header("SUSPICIOUS")]
        [SerializeField] private float _timeToConfirmInSecs = 2f;
        private float _detectedTime;

        [Header("ALERT")]
        [SerializeField] private float _alertSpeed = 3f;

        [Header("CHASE")]
        [SerializeField] private float _chaseSpeed = 7.5f;

        [Header("ATTACK")]
        [SerializeField] private float _attackDistanceThreshold = 5f;
        private float _startAttackTime;
        [SerializeField] private float _attackInterval = 2f;

        [Header("SEARCH")]
        [SerializeField] private float _searchSpeed = 2.5f;
        private float _searchStartTime;
        [SerializeField] private float _searchDurationInSecs = 5f;

        [Header("ESCAPE")]
        [SerializeField] private float _escapeSpeed = 7f;
        [SerializeField] private float _escapeDistanceThreshold = 70f;

        [Header("WAIT")]
        private float _startWaitTime;
        [SerializeField] private float _waitTimerInSecs = 20f;

        [Header("DEBUG/TEST")]
        [SerializeField] private int _playerLevel = 10;
        [SerializeField] private int _playerHP = 100;

        private void Start()
        {
            SetSpeed(_patrolSpeed);
            SetState(NPC_STATE_PB.PATROL);
        }

        private void Update()
        {
            try
            {
                FSMUpdate();
            }
            catch (Exception exception)
            {
                D($"ERROR! in FSMUpdate: {exception}", true);
            }
        }

        private void FSMUpdate()
        {
            switch (_currentState)
            {
                case NPC_STATE_PB.PATROL:
                    HandlePatrolState();
                    break;

                case NPC_STATE_PB.SUSPICIOUS:
                    HandleSuspiciousState();
                    break;

                case NPC_STATE_PB.ALERT:
                    HandleAlertState();
                    break;

                case NPC_STATE_PB.CHASE:
                    HandleChaseState();
                    break;

                case NPC_STATE_PB.ATTACK:
                    HandleAttackState();
                    break;

                case NPC_STATE_PB.SEARCH:
                    HandleSearchState();
                    break;

                case NPC_STATE_PB.ESCAPE:
                    HandleEscapeState();
                    break;

                case NPC_STATE_PB.WAIT:
                    HandleWaitState();
                    break;

                case NPC_STATE_PB.HOWL:
                    HandleHowlState();
                    break;
            }
        }

        private void SetSpeed(float speed)
        {
            _currentSpeed = speed;
        }

        private void SetState(NPC_STATE_PB stateToSet)
        {
            D($"Setting State: {stateToSet}");
            _currentState = stateToSet;
        }

        public NPC_STATE_PB GetState()
        {
            return _currentState;
        }

        public bool CheckState(NPC_STATE_PB stateToCheck)
        {
            return stateToCheck == _currentState;
        }

        private void D(string message, bool isError = false)
        {
            if (isError)
            {
                Debug.LogError($"<<{gameObject.name}>> <<NPCController_FSM_PB>> {message}");
                return;
            }

            Debug.Log($"<<{gameObject.name}>> <<NPCController_FSM_PB>> {message}");
        }

        private void OnDrawGizmos()
        {

            Gizmos.color = gameObject.name.Contains("Coward") ? Color.white : Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _howlRadius);
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
                _detectedTime = Time.time;
                SetState(NPC_STATE_PB.SUSPICIOUS);
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

            //SetSpeed(_alertSpeed);
            //SetState(NPC_STATE_PB.ALERT);
            SetState(NPC_STATE_PB.HOWL);
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
                    SetSpeed(_chaseSpeed);
                    SetState(NPC_STATE_PB.CHASE);
                }
                else
                {
                    SetSpeed(_escapeSpeed);
                    SetState(NPC_STATE_PB.ESCAPE);
                }
            }
            else
            {
                SetSpeed(_searchSpeed);
                SetState(NPC_STATE_PB.SEARCH);
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
                    SetState(NPC_STATE_PB.ATTACK);
                    return;
                }

                transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _currentSpeed * Time.deltaTime);
                return;
            }

            SetSpeed(_searchSpeed);
            SetState(NPC_STATE_PB.SEARCH);
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
                    SetSpeed(_chaseSpeed);
                    SetState(NPC_STATE_PB.CHASE);
                }

                return;
            }

            SetSpeed(_alertSpeed);
            SetState(NPC_STATE_PB.ALERT);
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
                    SetSpeed(_chaseSpeed);
                    SetState(NPC_STATE_PB.CHASE);

                    return;
                }
                else
                {
                    SetSpeed(_escapeSpeed);
                    SetState(NPC_STATE_PB.ESCAPE);

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
                    SetSpeed(_patrolSpeed);
                    SetState(NPC_STATE_PB.PATROL);

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
                SetState(NPC_STATE_PB.WAIT);
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
                SetSpeed(_escapeSpeed);
                SetState(NPC_STATE_PB.ESCAPE);

                return;
            }

            if (_waitTimerInSecs + _startWaitTime < Time.time)
            {
                D("Wait Over");
                SetSpeed(_patrolSpeed);
                SetState(NPC_STATE_PB.PATROL);
            }
        }
        #endregion

        #region HOWL
        private void HandleHowlState()
        {
            var npcs = Physics.OverlapSphere(transform.position, _howlRadius, _layerToHowl);

            foreach (var npc in npcs)
            {
                if (npc.TryGetComponent(out NPCController_Factory_PB npc_ai))
                {
                    npc_ai.OnHowlNotified(gameObject, _playerLastKnownPosition);
                }
            }

            SetState(NPC_STATE_PB.ALERT);
        }

        public void OnHowlNotified(GameObject gameObject, Vector3 lastKnownLocation)
        {
            D($"Howl notified by {gameObject.name}");

            if (_currentState.Equals("Patrol") || _currentState.Equals("Suspicious") || _currentState.Equals("Search"))
            {
                _playerLastKnownPosition = lastKnownLocation;
                SetState(NPC_STATE_PB.ALERT);
            }
        }
        #endregion
        #endregion
    }
}
