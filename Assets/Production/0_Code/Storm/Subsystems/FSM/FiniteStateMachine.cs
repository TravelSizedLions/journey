using System;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Subsystems.FSM {
  /// <summary>
  /// A class for creating state-based agents.
  /// </summary>
  public interface IStateMachine {
    /// <summary>
    /// Initialize the state machine with the beginning state.
    /// </summary>
    /// <param name="startState">The entry state.</param>
    void StartMachine(State startState);

    /// <summary>
    /// A callback for changing the state.
    /// </summary>
    /// <param name="oldState">The previous state.</param>
    /// <param name="newState">The next state.</param>
    void OnStateChange(State oldState, State newState);

    /// <summary>
    /// Pull the animation trigger for a given state.
    /// </summary>
    /// <param name="name">The name of the animation trigger.</param>
    void SetAnimParam(string name);

    /// <summary>
    /// Get the currently active state.
    /// </summary>
    /// <returns>The current active state.</returns>
    State GetCurrentState();

    /// <summary>
    /// Registers a new state in the machine
    /// </summary>
    /// <param name="state">The state to register.</param>
    void RegisterState(State state);

    /// <summary>
    /// Whether or not a state is already register in this state machine.
    /// </summary>
    /// <typeparam name="S">The type of state.</typeparam>
    /// <returns>True if the state's already been registered. False otherwise.</returns>
    bool ContainsState<S>() where S: State;

    /// <summary>
    /// Pull a state from the machine's cache.
    /// </summary>
    /// <typeparam name="S">The type of the state to get.</typeparam>
    /// <returns>The state.</returns>
    S GetState<S>() where S : State;


    /// <summary>
    /// Send a signal to the active state.
    /// </summary>
    /// <param name="obj">The GameObject that sent the signal</param>
    void Signal(GameObject obj);
  }


  /// <summary>
  /// A class for creating state-based agents.
  /// </summary>
  public class FiniteStateMachine : MonoBehaviour, IStateMachine {

    #region Fields
    /// <summary>
    /// The current state.
    /// </summary>
    private State state;

    /// <summary>
    /// The list of states in the graph.
    /// </summary>
    private Dictionary<Type, State> stateCache;

    /// <summary>
    /// The animator for the agent.
    /// </summary>
    private Animator animator;
    #endregion

    #region  Unity API
    private void Start() {
      stateCache = new Dictionary<Type, State>();
      animator = GetComponent<Animator>();
    }

    private void Update() {
      state.OnUpdateGeneral();
      state.OnUpdate();
    }

    private void FixedUpdate() {
      state.OnFixedUpdateGeneral();
      state.OnFixedUpdate();
    }
    #endregion

    #region Public Interface

    /// <summary>
    /// Initialize the state machine with the beginning state.
    /// </summary>
    /// <param name="startState">The entry state.</param>
    public void StartMachine(State startState) {
      stateCache = new Dictionary<Type, State>();
      animator = GetComponent<Animator>();

      state = startState;
      state.HiddenOnStateAdded(this);
      state.EnterState();

      // Makes sure trigger is cleared and ready.
      animator.ResetTrigger(startState.GetAnimParam());
    }

    /// <summary>
    /// A callback for changing the state.
    /// </summary>
    /// <param name="oldState">The previous state.</param>
    /// <param name="newState">The next state.</param>
    public void OnStateChange(State oldState, State newState) {
      state = newState;
      oldState.ExitState();
      newState.EnterState();
    }

    /// <summary>
    /// Pull the animation trigger for a given state.
    /// </summary>
    /// <param name="name">The name of the animation trigger.</param>
    public void SetAnimParam(string name) {
      animator.SetTrigger(name);
    }

    /// <summary>
    /// Get the currently active state.
    /// </summary>
    /// <returns>The current active state.</returns>
    public State GetCurrentState() {
      return state;
    }

    /// <summary>
    /// Registers a new state in the machine
    /// </summary>
    /// <param name="state">The state to register.</param>
    public void RegisterState(State state) {
      Type key = state.GetType();
      if (!stateCache.ContainsKey(key)) {
        stateCache.Add(key, state);
      }
    }

    /// <summary>
    /// Whether or not a state is already register in this state machine.
    /// </summary>
    /// <typeparam name="S">The type of state.</typeparam>
    /// <returns>True if the state's already been registered. False otherwise.</returns>
    public bool ContainsState<S>() where S : State {
      return stateCache.ContainsKey(typeof(S));
    }

    /// <summary>
    /// Pull a state from the machine's cache.
    /// </summary>
    /// <typeparam name="S">The type of the state to get.</typeparam>
    /// <returns>The state.</returns>
    public S GetState<S>() where S : State {
      return (S)stateCache[typeof(S)];
    }

    /// <summary>
    /// Whether or not the state machine is in a specific state.
    /// </summary>
    /// <typeparam name="S">The state to check.</typeparam>
    /// <returns>True if the state machine is in the state. False otherwise.</returns>
    public bool IsInState<S>() where S : State {
      if (ContainsState<S>()) {
        return stateCache[typeof(S)].enabled;
      }

      return false;
    }

    public void Signal(GameObject obj) {
      state.OnSignal(obj);
    }
    #endregion
  }
}