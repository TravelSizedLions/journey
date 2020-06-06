using System;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Subsystems.FSM {
  public interface IStateMachine {
    void StartMachine(State startState);

    void OnStateChange(State oldState, State newState);

    void SetAnimParam(string name);

    State GetCurrentState();

    void RegisterState(State state);

    bool ContainsState<S>() where S: State;

    S GetState<S>() where S : State;
  }


  public class FiniteStateMachine : MonoBehaviour, IStateMachine {

    private State state;

    private Dictionary<Type, State> stateCache;

    private Animator animator;

    private void Start() {
      stateCache = new Dictionary<Type, State>();
      animator = GetComponent<Animator>();
    }

    private void Update() {
      state.OnUpdate();
    }

    private void FixedUpdate() {
      state.OnFixedUpdate();
    }

    public void StartMachine(State startState) {
      stateCache = new Dictionary<Type, State>();
      animator = GetComponent<Animator>();

      state = startState;
      state.HiddenOnStateAdded(this);
      state.EnterState();
    }

    public void OnStateChange(State oldState, State newState) {
      state = newState;
      oldState.ExitState();
      newState.EnterState();
    }

    public void SetAnimParam(string name) {
      animator.SetTrigger(name);
    }

    public State GetCurrentState() {
      return state;
    }

    public void RegisterState(State state) {
      Type key = state.GetType();
      if (!stateCache.ContainsKey(key)) {
        stateCache.Add(key, state);
      }
    }

    public bool ContainsState<S>() where S : State {
      return stateCache.ContainsKey(typeof(S));
    }

    /// <summary>
    /// SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSState.
    /// </summary>
    public S GetState<S>() where S : State {
      return (S)stateCache[typeof(S)];
    }
  }
}