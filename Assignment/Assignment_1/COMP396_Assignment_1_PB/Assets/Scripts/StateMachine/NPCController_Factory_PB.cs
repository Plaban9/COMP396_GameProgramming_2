using AI.Simple_FSM;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Unity.VisualScripting;

using UnityEngine;

using Random = UnityEngine.Random;

namespace AI.FactoryDesign
{
    public class NPCController_Factory_PB : MonoBehaviour
    {
        [Serializable]
        public class RandomState
        {
            public string state;
            public int weight;
            public float speed;

            public override string ToString()
            {
                return $"State: {state}, Weight: {weight}, Speed: {speed}";
            }
        }

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
        private StateMachine.State _rest;
        private StateMachine.State _interactWithNPC;
        private StateMachine.State _inspectEquipment;
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
        [SerializeField] private float _waypointProximityThreshold;

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

        [Header("REST")]
        [SerializeField] private Transform _restWaypoint;
        [SerializeField] private float _restSpeed = 5f;
        [SerializeField] private float _restDurationInSecs = 5f;
        private float _restStartTime = 5f;
        private bool _isResting;

        [Header("INTERACT_WITH_NPC")]
        [SerializeField] private GameObject[] _otherNpc;
        private GameObject _interactableNpc;
        [SerializeField] private Vector3 _interactPosition;
        [SerializeField] private float _interactSpeed = 5f;
        [SerializeField] private float _interactDurationInSecs = 5f;
        [SerializeField] private float _interactRadius = 5f;
        [SerializeField] private bool _isTheInteractionStarter = false;
        [SerializeField] private bool _isInInteraction = false;
        private float _interactionTimer;

        [Header("INSPECT_EQUIPMENT")]
        [SerializeField] private float _inspectDurationInSecs = 5f;
        private float _inspectStartTimer;

        [Header("RANDOM_WEIGHTED_STATES")]
        [SerializeField] private List<RandomState> _randomStates;
        [SerializeField] private float _randomStateTransitionDurationInSecs = 5f;
        private float _lastRandomWeightDecisionTime;
        private int _totalRandomWeight;

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

                _lastRandomWeightDecisionTime = Time.time;
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

            //Rest
            _rest = _stateMachine.CreateState("Rest");

            _rest.OnEnter = delegate
            {
                SetSpeed(_restSpeed);
                D("Rest.OnEnter...");
            };

            _rest.OnExit = delegate
            {
                D("Rest.OnExit...");
            };

            _rest.OnFrame = delegate
            {
                D("Rest.OnFrame...");

                HandleRestState();
            };

            //Interact with NPC
            _interactWithNPC = _stateMachine.CreateState("Interact_with_NPC");

            _interactWithNPC.OnEnter = delegate
            {
                D("Interact_with_NPC.OnEnter...");
                SetupInteractWithNPCState();
            };

            _interactWithNPC.OnExit = delegate
            {
                D("Interact_with_NPC.OnExit...");
            };

            _interactWithNPC.OnFrame = delegate
            {
                D("Interact_with_NPC.OnFrame...");

                HandleInteractionWithNPCState();
            };

            //Inspect Equipment
            _inspectEquipment = _stateMachine.CreateState("InspectEquipment");

            _inspectEquipment.OnEnter = delegate
            {
                D("InspectEquipment.OnEnter...");
                _inspectStartTimer = Time.time;
            };

            _inspectEquipment.OnExit = delegate
            {
                D("InspectEquipment.OnExit...");
            };

            _inspectEquipment.OnFrame = delegate
            {
                D("InspectEquipment.OnFrame...");

                HandleInspectEquipmentState();
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



        #region Helpers
        private bool IsPlayerWithinSight()
        {
            return Vector3.Distance(transform.position, _player.transform.position) < _viewDistance;
        }
        private void SetSpeed(float speed)
        {
            _currentSpeed = speed;
        }

        private bool HandleRandomStates()
        {
            if (_randomStates != null)
            {
                if (_lastRandomWeightDecisionTime + _randomStateTransitionDurationInSecs <= Time.time)
                {
                    _lastRandomWeightDecisionTime = Time.time;
                    _totalRandomWeight = _randomStates.Sum(state => state.weight);

                    var randomStateResult = UnityEngine.Random.Range(0, _totalRandomWeight);
                    var randomState = GetStateBasedOnRandomWeightedPercentage(randomStateResult, _totalRandomWeight);

                    //D($"Total Weight: {_totalRandomWeight}");
                    //D($"Random State Number: {randomStateResult}");
                    //D($"Selected State: {randomState}");

                    if (randomState != null && !randomState.state.Equals(_currentState))
                    {
                        if (randomState.state.Equals("Interact_with_NPC"))
                        {
                            _isTheInteractionStarter = true;
                        }

                        _stateMachine.TransitionTo(_stateMachine.states[randomState.state]);

                        return true;
                    }
                }
            }

            return false;
        }

        private RandomState GetStateBasedOnRandomWeightedPercentage(int randomStateResult, int totalRandomWeight)
        {
            var prevRandom = 0;

            for (int i = 0; i < _randomStates.Count; i++)
            {
                RandomState randomState = _randomStates[i];

                if (randomStateResult > prevRandom && randomStateResult < randomState.weight + prevRandom)
                {
                    return randomState;
                }

                prevRandom += randomState.weight;
                //D("Previous Random: " + prevRandom);
            }

            return null;
        }

        private Collider[] GetNPCsInRange()
        {
            return Physics.OverlapSphere(transform.position, _howlRadius, _layerToHowl);
        }

        private bool TravelToWaypoint(Transform waypoint)
        {
            transform.position = Vector3.MoveTowards(transform.position, waypoint.position, _currentSpeed * Time.deltaTime);
            return Vector3.Distance(transform.position, waypoint.position) < _waypointProximityThreshold;
        }

        private bool TravelToWaypoint(Vector3 waypoint)
        {
            transform.position = Vector3.MoveTowards(transform.position, waypoint, _currentSpeed * Time.deltaTime);
            return Vector3.Distance(transform.position, waypoint) < _waypointProximityThreshold;
        }
        #endregion

        #region STATE HANDLERS
        #region PATROL
        private void HandlePatrolState()
        {
            FollowPath();

            if (HandleRandomStates())
            {
                return;
            }

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
            var samePlaneDestination = new Vector3(destination.x, 1f, destination.z);

            transform.position = Vector3.MoveTowards(transform.position, samePlaneDestination, _currentSpeed * Time.deltaTime);
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
                if (npc.TryGetComponent(out NPCController_Factory_PB npc_ai))
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

        #region REST
        public void HandleRestState()
        {
            if (IsPlayerWithinSight())
            {
                _detectedTime = Time.time;
                _stateMachine.TransitionTo(_suspicious);
                return;
            }

            if (TravelToWaypoint(_restWaypoint) && !_isResting)
            {
                _restStartTime = Time.time;
                _isResting = true;

                D("Started Rest");

                return;
            }

            if (_restDurationInSecs + _restStartTime < Time.time && _isResting)
            {
                D("Ended Rest");

                _isResting = false;
                _lastRandomWeightDecisionTime = Time.time;

                _stateMachine.TransitionTo(_patrol);
            }
        }
        #endregion

        #region INTERACT_WITH_NPC
        private void HandleInteractionWithNPCState()
        {
            if (IsPlayerWithinSight())
            {
                _detectedTime = Time.time;
                _stateMachine.TransitionTo(_suspicious);
                return;
            }

            if (_interactableNpc != null)
            {
                if (!_isInInteraction && TravelToWaypoint(_interactPosition))
                {
                    if (Vector3.Distance(transform.position, _interactableNpc.transform.position) < _interactRadius * 2 && !_isInInteraction && _isTheInteractionStarter)
                    {
                        _isInInteraction = true;
                        _interactionTimer = Time.time;

                        _interactableNpc.GetComponent<NPCController_Factory_PB>().OnNPCInteractionStarted(this.gameObject);

                        return;
                    }
                }
            }

            if (_isInInteraction && _isTheInteractionStarter)
            {
                if (_interactDurationInSecs + _interactionTimer < Time.time)
                {
                    _interactableNpc.GetComponent<NPCController_Factory_PB>().OnNPCInteractionEndNotified(this.gameObject);
                    _isTheInteractionStarter = false;
                    _isInInteraction = false;

                    _interactableNpc = null;

                    _stateMachine.TransitionTo(_patrol);
                }
            }
        }

        private void SetupInteractWithNPCState()
        {
            if (_isTheInteractionStarter)
            {
                var npcs = FilterNPCs(GetNPCsInRange(), this.GetComponent<Collider>());

                D("NPCs: " + npcs.Count);

                if (npcs.Count > 0)
                {
                    _interactableNpc = npcs[Random.Range(0, npcs.Count)].gameObject;
                }
                else
                {
                    _interactableNpc = _otherNpc.Length > 0 ? _otherNpc[Random.Range(0, _otherNpc.Length)] : null;
                }

                if (_interactableNpc == null)
                {
                    _stateMachine.TransitionTo(_patrol);
                    return;
                }

                _interactPosition = GetMidPointBetweenTwoNPCs(_interactableNpc.transform.position);

                if (_interactableNpc.TryGetComponent(out NPCController_Factory_PB npc_ai))
                {
                    npc_ai.OnNPCInteractionStartNotified(gameObject, _interactPosition);
                }
                else
                {
                    _stateMachine.TransitionTo(_patrol);
                }
            }
        }

        private List<Collider> FilterNPCs(Collider[] colliders, Collider colliderToExclude)
        {
            var filteredNPCList = new List<Collider>();

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliderToExclude == colliders[i])
                {
                    continue;
                }

                filteredNPCList.Add(colliders[i]);
            }

            return filteredNPCList;
        }

        public void OnNPCInteractionStartNotified(GameObject gameObject, Vector3 interactPosition)
        {
            D($"NPC Notified for interaction start notified by {gameObject.name}");

            _interactPosition = interactPosition;
            _interactableNpc = gameObject;

            _stateMachine.TransitionTo(_interactWithNPC);
        }

        public void OnNPCInteractionStarted(GameObject gameObject)
        {
            D($"NPC Notified for interaction started by {gameObject.name}");

            _isInInteraction = true;
        }

        public void OnNPCInteractionEndNotified(GameObject gameObject)
        {
            D($"NPC Notified for interaction end notified by {gameObject.name}");

            _isInInteraction = false;
            _stateMachine.TransitionTo(_patrol);
        }

        private Vector3 GetMidPointBetweenTwoNPCs(Vector3 npcPosition)
        {
            var x_diff = (npcPosition.x - this.transform.position.x) / 2f;
            var y_diff = (npcPosition.y - this.transform.position.y) / 2f;
            var z_diff = (npcPosition.z - this.transform.position.z) / 2f;

            return new Vector3(this.transform.position.x + x_diff, this.transform.position.y + y_diff, this.transform.position.z + z_diff);
        }
        #endregion

        #region INSPECT_EQUIPMENT
        public void HandleInspectEquipmentState()
        {
            if (IsPlayerWithinSight())
            {
                _detectedTime = Time.time;
                _stateMachine.TransitionTo(_suspicious);
                return;
            }

            if (_inspectDurationInSecs + _inspectStartTimer < Time.time)
            {
                D("Ended Weapon Inspection");

                _lastRandomWeightDecisionTime = Time.time;

                _stateMachine.TransitionTo(_patrol);
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
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _viewDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackDistanceThreshold);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _escapeDistanceThreshold);

            Gizmos.color = gameObject.name.Contains("Coward") ? Color.cyan : Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _howlRadius);
        }
    }
}
