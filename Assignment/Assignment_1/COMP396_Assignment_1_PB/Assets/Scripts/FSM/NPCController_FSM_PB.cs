using System;
using System.Collections.Generic;
using System.Linq;

using Unity.VisualScripting;

using UnityEngine;

using Random = UnityEngine.Random;

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
            REST,
            INTERACT_WITH_NPC,
            INSPECT_EQUIPMENT,
            ESCAPE,
            WAIT
        };

        [Serializable]
        public class RandomState
        {
            public NPC_STATE_PB state;
            public int weight;
            public float speed;

            public override string ToString()
            {
                return $"State: {state.HumanName()}, Weight: {weight}, Speed: {speed}";
            }
        }

        [Header("GENERAL ATTRIBUTES")]
        [SerializeField] private NPC_STATE_PB _currentState;
        [SerializeField] private int _health = 100;
        [SerializeField] private int _level = 5;
        [SerializeField] private int _viewDistance = 50;
        [SerializeField] private float _currentSpeed;
        [SerializeField] private float _viewAngleInDegrees = 45f; // i.e; from centre of vision half of the angle on each side
        [SerializeField] private GameObject _player;
        [SerializeField] private Vector3 _playerLastKnownPosition;
        [SerializeField] private float _waypointProximityThreshold;

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
        [SerializeField] private float _waitTimerInSecs = 20f;
        private float _startWaitTime;

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

        private void Start()
        {
            _lastRandomWeightDecisionTime = Time.time;
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

                case NPC_STATE_PB.REST:
                    HandleRestState();
                    break;

                case NPC_STATE_PB.INTERACT_WITH_NPC:
                    HandleInteractionWithNPCState();
                    break;

                case NPC_STATE_PB.INSPECT_EQUIPMENT:
                    HandleInspectEquipmentState();
                    break;
            }
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
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _viewDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackDistanceThreshold);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _escapeDistanceThreshold);

            Gizmos.color = gameObject.name.Contains("Coward") ? Color.cyan : Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _howlRadius);
        }

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
                _detectedTime = Time.time;
                SetState(NPC_STATE_PB.SUSPICIOUS);
            }
        }

        private void FollowPath()
        {
            if (Vector3.Distance(transform.position, _waypoints[_targetWaypointIndex].position) < _waypointProximityThreshold)
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
            var samePlaneDestination = new Vector3(destination.x, 1f, destination.z);

            transform.position = Vector3.MoveTowards(transform.position, samePlaneDestination, _currentSpeed * Time.deltaTime);
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
            var npcs = GetNPCsInRange();

            foreach (var npc in npcs)
            {
                if (npc.TryGetComponent(out NPCController_FSM_PB npc_ai))
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

        #region REST
        public void HandleRestState()
        {
            if (IsPlayerWithinSight())
            {
                _detectedTime = Time.time;
                SetState(NPC_STATE_PB.SUSPICIOUS);
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

                SetSpeed(_patrolSpeed);
                SetState(NPC_STATE_PB.PATROL);
            }
        }
        #endregion

        #region INTERACT_WITH_NPC
        private void HandleInteractionWithNPCState()
        {
            if (IsPlayerWithinSight())
            {
                _detectedTime = Time.time;
                SetState(NPC_STATE_PB.SUSPICIOUS);
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

                        _interactableNpc.GetComponent<NPCController_FSM_PB>().OnNPCInteractionStarted(this.gameObject);

                        return;
                    }
                }
            }

            if (_isInInteraction && _isTheInteractionStarter)
            {
                if (_interactDurationInSecs + _interactionTimer < Time.time)
                {
                    _interactableNpc.GetComponent<NPCController_FSM_PB>().OnNPCInteractionEndNotified(this.gameObject);
                    _isTheInteractionStarter = false;
                    _isInInteraction = false;

                    _interactableNpc = null;

                    SetSpeed(_patrolSpeed);
                    SetState(NPC_STATE_PB.PATROL);
                    HandleInitialStateAttributes();
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
                    SetSpeed(_patrolSpeed);
                    SetState(NPC_STATE_PB.PATROL);
                    HandleInitialStateAttributes();
                    return;
                }

                _interactPosition = GetMidPointBetweenTwoNPCs(_interactableNpc.transform.position);

                if (_interactableNpc.TryGetComponent(out NPCController_FSM_PB npc_ai))
                {
                    npc_ai.OnNPCInteractionStartNotified(gameObject, _interactPosition);
                }
                else
                {
                    SetSpeed(_patrolSpeed);
                    SetState(NPC_STATE_PB.PATROL);
                    HandleInitialStateAttributes();
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
            SetSpeed(_interactSpeed);
            SetState(NPC_STATE_PB.INTERACT_WITH_NPC);
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
            SetSpeed(_patrolSpeed);
            SetState(NPC_STATE_PB.PATROL);
            HandleInitialStateAttributes();
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
                SetState(NPC_STATE_PB.SUSPICIOUS);
                return;
            }

            if (_inspectDurationInSecs + _inspectStartTimer < Time.time)
            {
                D("Ended Weapon Inspection");

                _lastRandomWeightDecisionTime = Time.time;

                SetSpeed(_patrolSpeed);
                SetState(NPC_STATE_PB.PATROL);
            }
        }
        #endregion

        #region Helpers
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

                    if (randomState != null && randomState.state != GetState())
                    {
                        if (randomState.state == NPC_STATE_PB.INTERACT_WITH_NPC)
                        {
                            _isTheInteractionStarter = true;
                        }

                        HandleRandomInitialStateAttributes(randomState.state);
                        SetSpeed(randomState.speed);
                        SetState(randomState.state);

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

        private void HandleInitialStateAttributes()
        {
            switch (_currentState)
            {
                case NPC_STATE_PB.INTERACT_WITH_NPC:
                    SetupInteractWithNPCState();
                    break;

                case NPC_STATE_PB.INSPECT_EQUIPMENT:
                    D("Started Weapon Inspection");
                    _inspectStartTimer = Time.time;
                    break;
                case NPC_STATE_PB.PATROL:
                    _lastRandomWeightDecisionTime = Time.time;
                    break;

                case NPC_STATE_PB.REST:
                case NPC_STATE_PB.SUSPICIOUS:
                case NPC_STATE_PB.HOWL:
                case NPC_STATE_PB.ALERT:
                case NPC_STATE_PB.CHASE:
                case NPC_STATE_PB.ATTACK:
                case NPC_STATE_PB.SEARCH:
                case NPC_STATE_PB.ESCAPE:
                case NPC_STATE_PB.WAIT:
                    break;
            }
        }

        private void HandleRandomInitialStateAttributes(NPC_STATE_PB state)
        {
            switch (state)
            {
                case NPC_STATE_PB.INTERACT_WITH_NPC:
                    SetupInteractWithNPCState();
                    break;

                case NPC_STATE_PB.INSPECT_EQUIPMENT:
                    D("Started Weapon Inspection");
                    _inspectStartTimer = Time.time;
                    break;
                case NPC_STATE_PB.PATROL:
                    _lastRandomWeightDecisionTime = Time.time;
                    break;

                case NPC_STATE_PB.REST:
                case NPC_STATE_PB.SUSPICIOUS:
                case NPC_STATE_PB.HOWL:
                case NPC_STATE_PB.ALERT:
                case NPC_STATE_PB.CHASE:
                case NPC_STATE_PB.ATTACK:
                case NPC_STATE_PB.SEARCH:
                case NPC_STATE_PB.ESCAPE:
                case NPC_STATE_PB.WAIT:
                    break;

            }
        }

        private bool IsPlayerWithinSight()
        {
            return Vector3.Distance(transform.position, _player.transform.position) < _viewDistance;
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
        #endregion
    }
}
