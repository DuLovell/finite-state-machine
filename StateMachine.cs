using System;
using System.Collections.Generic;
using FSM.Attributes;
using JetBrains.Annotations;
using UnityEngine;
#nullable enable

// Notes
// 1. What a finite state machine is
// 2. Examples where you'd use one
//     AI, Animation, Game State
// 3. Parts of a State Machine
//     States & Transitions
// 4. States - 3 Parts
//     Tick - Why it's not Update()
//     OnEnter / OnExit (setup & cleanup)
// 5. Transitions
//     Separated from states so they can be re-used
//     Easy transitions from any state

namespace FSM
{
   public abstract class StateMachine : MonoBehaviour
   {
      [ReadOnlyInspector] [UsedImplicitly]
      [SerializeField] protected string? _currentStateName;
      
      private readonly Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type, List<Transition>>();
      private List<Transition> _currentTransitions = new List<Transition>();
      private readonly List<Transition> _anyTransitions = new List<Transition>();
   
      private static readonly List<Transition> _emptyTransitions = new List<Transition>(0);
      
      private readonly List<Action> _currentActions = new List<Action>();

      private State? _currentState;

      protected void SetState(State state)
      {
         if (state == CurrentState) {
            return;
         }
         
         foreach (Transition currentTransition in _currentTransitions) {
            if (currentTransition.TriggerEvent == null) {
               continue;
            }

            foreach (Action currentAction in _currentActions) {
               currentTransition.TriggerEvent.Event -= currentAction;  
            }
         }

         if (CurrentState != null) {
            CurrentState.OnExit();
         }
         CurrentState = state;

         _transitions.TryGetValue(CurrentState.GetType(), out _currentTransitions);
         _currentTransitions ??= _emptyTransitions;

         CurrentState.OnEnter();

         _currentActions.Clear();
         foreach (Transition currentTransition in _currentTransitions) {
            if (currentTransition.TriggerEvent == null) {
               continue;
            }

            void SetStateAction() => SetState(currentTransition.To);
            currentTransition.TriggerEvent.Event += SetStateAction;
            _currentActions.Add(SetStateAction);
         }
      }

      protected void AddTransition(State from, State to, Func<bool> predicate)
      {
         if (_transitions.TryGetValue(from.GetType(), out var transitions) == false)
         {
            transitions = new List<Transition>();
            _transitions[from.GetType()] = transitions;
         }
      
         transitions.Add(new Transition(to, predicate));
      }
      
      protected void AddTransition(State from, State to, EventObject triggerEvent)
      {
         if (_transitions.TryGetValue(from.GetType(), out var transitions) == false)
         {
            transitions = new List<Transition>();
            _transitions[from.GetType()] = transitions;
         }
      
         transitions.Add(new Transition(to, triggerEvent));
      }

      protected void AddAnyTransition(State state, Func<bool> predicate)
      {
         _anyTransitions.Add(new Transition(state, predicate));
      }

      private void Update()
      {
         Transition? transition = GetTransition();
         if (transition != null) {
            SetState(transition.To);
         }

         CurrentState?.OnUpdate();
      }

      private void FixedUpdate()
      {
         CurrentState?.OnFixedUpdate();
      }

      private void OnTriggerEnter2D(Collider2D other)
      {
         CurrentState?.OnTriggerEnter2D(other);
      }

      private Transition? GetTransition()
      {
         foreach (Transition? transition in _anyTransitions) {
            if (transition.Condition != null && transition.Condition()) {
               return transition;
            }
         }

         foreach (Transition? transition in _currentTransitions) {
            if (transition.Condition != null && transition.Condition()) {
               return transition;
            }
         }

         return null;
      }

      private State? CurrentState
      {
         get
         {
            return _currentState;
         }
         set
         {
            _currentState = value;
            _currentStateName = value?.Name;
         }
      }
   }
}
