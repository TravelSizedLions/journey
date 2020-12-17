using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Storm.Subsystems.FSM {

  /// <summary>
  /// The state-facing interface for the state machine.
  /// </summary>
  public interface IStateMachineExternal {
    /// <summary>
    /// Whether or not the state machine is running.
    /// </summary>
    bool Running { get; }

    /// <summary>
    /// The default starting state for the machine.
    /// </summary>
    string StartingState { get; set; }

    /// <summary>
    /// Initialize the state machine with the beginning state.
    /// </summary>
    /// <param name="startState">The entry state.</param>
    void StartMachine(State startState);

    /// <summary>
    /// Pause state transitions on the machine. Update and FixedUpdate logic will still run.
    /// </summary>
    void Pause();

    /// <summary>
    /// Resume state transitions on the machine.
    /// </summary>
    void Resume();

    /// <summary>
    /// Whether or not the state machine is in a specific state.
    /// </summary>
    /// <typeparam name="S">The state to check.</typeparam>
    /// <returns>True if the state machine is in the state. False otherwise.</returns>
    bool IsInState<S>() where S : State;

    /// <summary>
    /// Send a signal to the active state.
    /// </summary>
    /// <param name="obj">The GameObject that sent the signal</param>
    void Signal(GameObject obj);


    /// <summary>
    /// Force a state change. This should ONLY be used to drive animation during
    /// cutscenes.
    /// </summary>
    /// <typeparam name="S">The state to change to.</typeparam>
    void ChangeState<S>() where S : State;
  }

  /// <summary>
  /// The state-facing interface for the state machine.
  /// </summary>
  public interface IStateMachineInternal {
    /// <summary>
    /// Whether or not the state machine is running.
    /// </summary>
    bool Running { get; }

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
    /// Registers a new state in the machine
    /// </summary>
    /// <param name="state">The state to register.</param>
    void RegisterState(State state);

    /// <summary>
    /// Whether or not a state is already register in this state machine.
    /// </summary>
    /// <typeparam name="S">The type of state.</typeparam>
    /// <returns>True if the state's already been registered. False otherwise.</returns>
    bool ContainsState<S>() where S : State;

    /// <summary>
    /// Pull a state from the machine's cache.
    /// </summary>
    /// <typeparam name="S">The type of the state to get.</typeparam>
    /// <returns>The state.</returns>
    S GetState<S>() where S : State;
  }

  public interface IStateMachine : IStateMachineExternal, IStateMachineInternal { }

  /// <summary>
  /// A class for creating state-based agents.
  /// </summary>
  public class FiniteStateMachine : MonoBehaviour, IStateMachine {

    #region Properties
    /// <summary>
    /// Whether or not the state machine is running.
    /// </summary>
    public bool Running { get { return running; } }

    /// <summary>
    /// The starting state for this State Machine.
    /// </summary>
    public string StartingState { 
      get { return startState; }
      set { startState = value; } 
    }
    #endregion

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

    /// <summary>
    /// Whether or not the machine is currently running.
    /// </summary>
    private bool running;

    /// <summary>
    /// The starting state for this state machine.
    /// </summary>
    [SerializeField]
    [Tooltip("The starting state for this state machine.")]
    [ValueDropdown("GetStateTypes")]
    private string startState;
    #endregion

    #region  Unity API
    private void Start() {
      stateCache = new Dictionary<Type, State>();
      animator = GetComponent<Animator>();
    }

    private void Update() {
      if (state != null) {
        state.OnUpdateGeneral();
        state.OnUpdate();
      } 
    }

    private void FixedUpdate() {
      if (state != null) {
        state.OnFixedUpdateGeneral();
        state.OnFixedUpdate();
      }
    }
    #endregion

    #region Public Interface

    public void StartMachine(Type startState = null) {
      if (startState == null) {
        startState = Type.GetType(StartingState);
      }

      StateDriver driver = StateDriver.For(startState);
      driver.StartMachine(this);
    }

    /// <summary>
    /// Initialize the state machine with the beginning state.
    /// </summary>
    /// <param name="startState">The entry state.</param>
    public void StartMachine(State startState) {
      running = true;
      stateCache = new Dictionary<Type, State>();
      animator = GetComponent<Animator>();

      state = startState;
      state.HiddenOnStateAdded(this);
      state.EnterState();

      // Makes sure trigger is cleared and ready.
      animator.ResetTrigger(startState.GetAnimParam());
    }

    /// <summary>
    /// Pause state transitions on the machine. Update and FixedUpdate logic will still run.
    /// </summary>
    public void Pause() {
      running = false;
    }

    /// <summary>
    /// Resume state transitions on the machine.
    /// </summary>
    public void Resume() {
      running = true;
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
      stateCache = stateCache ?? new Dictionary<Type, State>();
      return stateCache.ContainsKey(typeof(S));
    }

    /// <summary>
    /// Pull a state from the machine's cache.
    /// </summary>
    /// <typeparam name="S">The type of the state to get.</typeparam>
    /// <returns>The state.</returns>
    public S GetState<S>() where S : State {
      return (S) stateCache[typeof(S)];
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
      if (state != null) {
        state.OnSignalGeneral(obj);
        state.OnSignal(obj);
      }
    }

    /// <summary>
    /// Force a state change. This should ONLY be used to drive animation during
    /// cutscenes.
    /// </summary>
    /// <typeparam name="S">The state to change to.</typeparam>
    public void ChangeState<S>() where S : State {

      State newState = null;
      if (ContainsState<S>()) {
        newState = GetState<S>();

      } else {
        newState = gameObject.AddComponent<S>();
        RegisterState(newState);
        newState.HiddenOnStateAdded(this);
      }

      if (state != null) {
        state.ExitState();
      }
      
      state = newState;
      state.EnterState();
    }
    #endregion


    /// <summary>
    /// Get a list of the player's types.
    /// </summary>
    private ValueDropdownList<string> GetStateTypes() {
      ValueDropdownList<string> types = new ValueDropdownList<string>();

      foreach (Type t in FiniteStateMachine.GetSubtypesOfType(typeof(State))) {
        string typeName = t.ToString();

        string[] subs = typeName.Split('.');
        string simpleName = subs[subs.Length-1];
        string letter = (""+simpleName[0]).ToUpper();

        string folder = letter + "/" + simpleName;

        types.Add(new ValueDropdownItem<string>(folder, t.ToString()));
      }

      return types;
    }

    /// <summary>
    /// Within a code assembly, searches for all subtypes of the given type.
    /// </summary>
    /// <param name="assemblyName">The name of the C# assembly.</param>
    /// <param name="t">The type to search for.</param>
    /// <returns>The list of types in the assebly that are a subtype of t.</returns>
    private static List<Type> GetSubtypesOfType(Type t) {
      List<Type> results = new List<Type>();

      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
        foreach (Type type in assembly.GetTypes()) {
          if (type.IsSubclassOf(t)) {
            results.Add(type);
          }
        }
      }

      return results;
    }
  }

  /// <summary>
  /// A class that allows someone to forcibly change the state of a finite state
  /// machine.
  /// </summary>
  public abstract class StateDriver {
    public static StateDriver For(string type) {
      return For(Type.GetType(type));
    }

    public static StateDriver For(Type type) {
      Type[] typeArr = new Type[] { type };
      var genericBase = typeof(StateDriver<>);
      var combined = genericBase.MakeGenericType(typeArr);
      return (StateDriver) Activator.CreateInstance(combined);
    }

    /// <summary>
    /// Start the given machine with this state driver's state. If the state
    /// does not exist on the state machine's game object, it will be added.
    /// </summary>
    /// <param name="fsm">The machine to start.</param>
    public abstract void StartMachine(FiniteStateMachine fsm);

    /// <summary>
    /// Whether or not the finite state machine is already in this State
    /// driver's state.
    /// </summary>
    /// <param name="fsm">The state machine to check this for.</param>
    /// <returns>True if the state machine is already in the state.</returns>
    public abstract bool IsInState(FiniteStateMachine fsm);

    /// <summary>
    /// Forcibly change the state of the machine.
    /// </summary>
    /// <param name="fsm">The machine to force a state change on.</param>
    public abstract void ForceStateChangeOn(FiniteStateMachine fsm);
  }

  /// <summary>
  /// A class that allows someone to forcibly change the state of a finite state
  /// machine.
  /// </summary>
  /// <typeparam name="S">The type of the state.</typeparam>
  public class StateDriver<S> : StateDriver where S : State {

    /// <summary>
    /// Start the given machine with this state driver's state. If the state
    /// does not exist on the state machine's game object, it will be added.
    /// </summary>
    /// <param name="fsm">The machine to start.</param>
    public override void StartMachine(FiniteStateMachine fsm) {
      S state = fsm.gameObject.GetComponent<S>();
      if (state == null) {
        state = fsm.gameObject.AddComponent<S>();
      }

      fsm.StartMachine(state);
    }

    /// <summary>
    /// Whether or not the finite state machine is already in this State
    /// driver's state.
    /// </summary>
    /// <param name="fsm">The state machine to check this for.</param>
    /// <returns>True if the state machine is already in the state.</returns>
    public override bool IsInState(FiniteStateMachine fsm) => fsm.IsInState<S>();
    
    /// <summary>
    /// Forcibly change the state of the machine.
    /// </summary>
    /// <param name="fsm">The machine to force a state change on.</param>
    public override void ForceStateChangeOn(FiniteStateMachine fsm) {
      fsm.ChangeState<S>();
    }
  }
}