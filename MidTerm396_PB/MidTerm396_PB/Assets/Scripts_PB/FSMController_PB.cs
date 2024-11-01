using Player;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

namespace AI.FactoryDesign
{
    public class FSMController_PB : MonoBehaviour
    {
        #region STATES
        private StateMachine_PB _stateMachine;

        private StateMachine_PB.State _patrol;
        private StateMachine_PB.State _chase;
        private StateMachine_PB.State _wander;
        private StateMachine_PB.State _evade;
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
        private Vector3 rayDirection;
        [SerializeField] private int FieldOfView = 45;
        [SerializeField] private MeshRenderer _npcRenderer;

        [Header("PATROL")]
        [SerializeField] private float _patrolSpeed = 6f;
        [SerializeField] private Transform[] _waypoints;
        [SerializeField] private int _targetWaypointIndex = 0;
        [SerializeField] private Material _patrolMaterial;
        [SerializeField] private float _patrolReachThreshold;
        [SerializeField] private float _chanceForWander;
        [SerializeField] private float _wanderCheckDuration;
        [SerializeField] private float _wanderStartCheckTimer;

        [Header("WANDER")]
        [SerializeField] private float _wanderSpeed = 6f;
        [SerializeField] private Vector3 _wanderPoint;
        [SerializeField] private float _wanderPointReachThreshold;
        [SerializeField] private Material _wanderMaterial;

        [SerializeField] private float _wanderTimer;
        [SerializeField] private float _wanderDuration;
        [SerializeField] private float _minWanderDuration;
        [SerializeField] private float _maxWanderDuration;

        [Header("CHASE")]
        [SerializeField] private float _chaseSpeed = 8f;
        [SerializeField] private Material _chaseMaterial;

        [Header("EVADE")]
        [SerializeField] private float _evadeSpeed = 7f;
        [SerializeField] private float _evadeDistanceThreshold = 4f;
        [SerializeField] private float _evadeDistanceCheck = 4f;
        [SerializeField] private Material _evadeMaterial;

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
            _stateMachine = new StateMachine_PB();

            //Patrol
            _patrol = _stateMachine.CreateState("Patrol");
            _patrol.OnEnter = delegate
            {
                D("Patrol.OnEnter...");
                _wanderStartCheckTimer = Time.time;
                SetMaterial(_patrolMaterial);
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

            //Chase
            _chase = _stateMachine.CreateState("Chase");
            _chase.OnEnter = delegate
            {
                D("Chase.OnEnter...");
                SetMaterial(_chaseMaterial);
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

            //Evade
            _evade = _stateMachine.CreateState("Evade");
            _evade.OnEnter = delegate
            {
                D("Evade.OnEnter...");
                SetMaterial(_evadeMaterial);
                SetSpeed(_evadeSpeed);
            };

            _evade.OnExit = delegate
            {
                D("Evade.OnExit...");
            };

            _evade.OnFrame = delegate
            {
                D("Evade.OnFrame...");

                HandleEvadeState();
            };

            //Wander
            _wander = _stateMachine.CreateState("Wander");
            _wander.OnEnter = delegate
            {
                D("Wander.OnEnter...");
                _wanderPoint = new Vector3(UnityEngine.Random.Range(-10, 10), transform.position.y, UnityEngine.Random.Range(-10, 10));
                _wanderTimer = Time.time;
                SetMaterial(_wanderMaterial);
                SetSpeed(_wanderSpeed);
            };

            _wander.OnExit = delegate
            {
                D("Wander.OnExit...");
            };

            _wander.OnFrame = delegate
            {
                D("Wander.OnFrame...");

                HandleWanderState();
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

        private void SetMaterial(Material material)
        {
            _npcRenderer.material = material;
        }

        #region Helpers
        private bool IsPlayerWithinSight()
        {
            //return Vector3.Distance(transform.position, _player.transform.position) < _viewDistance;
            return DetectPlayer();
        }

        //Detect perspective field of view for the AI Character
        private bool DetectPlayer()
        {
            rayDirection = (_player.transform.position - transform.position).normalized;

            if ((Vector3.Angle(rayDirection, transform.forward)) < FieldOfView)
            {
                RaycastHit hit;
                // Detect if player is within the field of view
                if (Physics.Raycast(transform.position, rayDirection, out hit, _viewDistance))
                {
                    var player = hit.collider.GetComponent<PlayerController_PB>();

                    return player != null;
                }
            }

            return false;
        }
        #endregion

        #region STATE HANDLERS
        #region PATROL
        private void HandlePatrolState()
        {
            FollowPath();

            if (_wanderStartCheckTimer + _wanderCheckDuration < Time.time)
            {
                D("Checking For Wander...");
                var chanceForRandom = UnityEngine.Random.Range(0f, 1f);

                if (chanceForRandom <= _chanceForWander)
                {
                    _wanderDuration = UnityEngine.Random.Range(_minWanderDuration, _maxWanderDuration);
                    _stateMachine.TransitionTo(_wander);
                    return;
                }

                _wanderStartCheckTimer = Time.time;
            }

            if (IsPlayerWithinSight())
            {
                _playerLastKnownPosition = _player.transform.position;

                PlayerController_PB playerController = _player.GetComponent<PlayerController_PB>();

                if (playerController.GetPowerUpStatus())
                {
                    var distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

                    if (distanceToPlayer < _evadeDistanceCheck)
                    {
                        _stateMachine.TransitionTo(_evade);
                    }
                }
                else
                {
                    _stateMachine.TransitionTo(_chase);
                }
            }
        }

        private void FollowPath()
        {
            if (Vector3.Distance(transform.position, _waypoints[_targetWaypointIndex].position) < _patrolReachThreshold)
            {
                _targetWaypointIndex = (_targetWaypointIndex + 1) % _waypoints.Length;
            }

            transform.position = Vector3.MoveTowards(transform.position, _waypoints[_targetWaypointIndex].position, _currentSpeed * Time.deltaTime);
        }
        #endregion

        #region CHASE
        private void HandleChaseState()
        {
            var distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

            if (IsPlayerWithinSight())
            {
                _playerLastKnownPosition = _player.transform.position;
                var destination = new Vector3(_playerLastKnownPosition.x, transform.position.y, _playerLastKnownPosition.z);
                transform.position = Vector3.MoveTowards(transform.position, destination, _currentSpeed * Time.deltaTime);
                return;
            }

            _stateMachine.TransitionTo(_patrol);
        }
        #endregion

        #region WANDER
        private void HandleWanderState()
        {
            FollowWanderPath();

            if (_wanderDuration + _wanderTimer < Time.time)
            {
                _stateMachine.TransitionTo(_patrol);
            }

            if (IsPlayerWithinSight())
            {
                _playerLastKnownPosition = _player.transform.position;

                PlayerController_PB playerController = _player.GetComponent<PlayerController_PB>();

                if (playerController.GetPowerUpStatus())
                {
                    var distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

                    if (distanceToPlayer < _evadeDistanceCheck)
                    {
                        _stateMachine.TransitionTo(_evade);
                    }
                }
                else
                {
                    _stateMachine.TransitionTo(_chase);
                }
            }
        }

        private void FollowWanderPath()
        {
            if (Vector3.Distance(transform.position, _wanderPoint) < _wanderPointReachThreshold)
            {
                _wanderPoint = new Vector3(UnityEngine.Random.Range(-10, 10), transform.position.y, UnityEngine.Random.Range(-10, 10));
                ;
            }

            transform.position = Vector3.MoveTowards(transform.position, _wanderPoint, _currentSpeed * Time.deltaTime);
        }
        #endregion

        #region Evade
        private void HandleEvadeState()
        {
            if (IsPlayerWithinSight())
            {
                _playerLastKnownPosition = _player.transform.position;
            }

            var distanceFromLastKnownPlayerPosition = Vector3.Distance(transform.position, _playerLastKnownPosition);

            if (distanceFromLastKnownPlayerPosition > _evadeDistanceThreshold)
            {
                _stateMachine.TransitionTo(_patrol);
                return;
            }

            var directionVectorFromEnemyToPlayer = (transform.position - _playerLastKnownPosition).normalized;
            var destination = directionVectorFromEnemyToPlayer.normalized * _evadeDistanceThreshold;
            destination = new Vector3(destination.x, transform.position.y, destination.z);

            transform.position = Vector3.MoveTowards(transform.position, destination, _currentSpeed * Time.deltaTime);
        }
        #endregion
        #endregion

        #region DEBUG
        private void D(string message, bool isError = false)
        {
            if (isError)
            {
                Debug.LogError($"<<{gameObject.name}>> <<FSMController_PB>> {message}");
                return;
            }

            Debug.Log($"<<{gameObject.name}>> <<FSMController_PB>> {message}");
        }

        private void OnDrawGizmos()
        {

            //Gizmos.color = gameObject.name.Contains("Coward") ? Color.white : Color.magenta;
            //Gizmos.DrawWireSphere(transform.position, _howlRadius);
            //Gizmos.Draw(transform.position, _howlRadius);

            if (!Application.isEditor || _player.transform.position == null)
                return;

            Debug.DrawLine(transform.position, _player.transform.position, Color.red);

            Vector3 frontRayPoint = transform.position + (transform.forward * _viewDistance);

            //Approximate perspective visualization
            Vector3 leftRayPoint = Quaternion.Euler(0f, FieldOfView * 0.5f, 0f) * frontRayPoint;

            Vector3 rightRayPoint = Quaternion.Euler(0f, -FieldOfView * 0.5f, 0f) * frontRayPoint;

            Debug.DrawLine(transform.position, frontRayPoint, Color.green);
            Debug.DrawLine(transform.position, leftRayPoint, Color.green);
            Debug.DrawLine(transform.position, rightRayPoint, Color.green);
        }
        #endregion
    }
}
