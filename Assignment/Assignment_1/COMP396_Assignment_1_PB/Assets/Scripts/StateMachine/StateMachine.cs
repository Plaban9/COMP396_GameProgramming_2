using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FactoryDesign
{
    public class StateMachine : MonoBehaviour
    {
        public class State
        {
            public string name;
            public Action OnFrame;
            public Action OnEnter;
            public Action OnExit;

            public override string ToString()
            {
                return name;
            }
        }

        public bool ready = false;
        public Dictionary<string, State> states = new Dictionary<string, State>();

        public State CurrentState
        {
            get; private set;
        }

        [SerializeField] private State _initialState;

        public State CreateState(string name)
        {
            var state = new State();
            state.name = name;

            // Will be good to check the DKV for duplicate states

            if (states.Count == 0)
            {
                _initialState = state;
            }

            states[name] = state;

            return state;
        }


        public void Update()
        {
            if (!ready)
            {
                return;
            }

            if (states.Count == 0 || _initialState == null)
            {
                Debug.LogError("State Machine with no states!");
                return;
            }

            if (CurrentState == null)
            {
                TransitionTo(_initialState);
            }

            if (CurrentState.OnFrame != null)
            {
                CurrentState.OnFrame();
            }
        }

        public void TransitionTo(State newState)
        {
            if (newState == null)
            {
                Debug.LogError("Trying to transition into null state!");
                return;
            }

            CurrentState?.OnExit?.Invoke();

            // Same as above
            //if (CurrentState != null && CurrentState.OnExit != null)
            //{
            //    CurrentState.OnExit();
            //}

            Debug.Log($"Trying from state {CurrentState} to {newState}.");
            CurrentState = newState;

            CurrentState?.OnEnter?.Invoke();

            // Same as above
            //if (CurrentState != null && CurrentState.OnEnter != null)
            //{
            //    CurrentState.OnEnter();
            //}

        }

        public void TransitionTo(string stateName)
        {
            if (!states.ContainsKey(stateName))
            {
                Debug.LogError($"State Machine doesn't contain state {stateName}!");
                return;
            }

            TransitionTo(states[stateName]);
        }
    }
}