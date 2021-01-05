using System;
using Storm.Extensions;
using UnityEditor.Animations;
using UnityEngine;

namespace Storm.Subsystems.FSM {

  /// <summary>
  /// A class that allows someone to forcibly change the state of a finite state
  /// machine.
  /// </summary>
  public abstract class StateDriver {

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    /// <summary>
    /// Get the state driver for the given state.
    /// </summary>
    /// <param name="state">The state.</param>
    /// <returns>A driver for the given state that allows special operations on
    /// finite state machines</return>
    public static StateDriver For(State state) => state != null ? For(state.GetType()) : null;

    /// <summary>
    /// Get the state driver for the given state.
    /// </summary>
    /// <param name="type">The fully qualified type name of the state to get a
    /// state driver for.</param>
    /// <returns>A driver for the given state that allows special operations on
    /// finite state machines</return>
    public static StateDriver For(string type) => !string.IsNullOrEmpty(type) ? For(Type.GetType(type)) : null;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">The type of the state to get a driver for.</param>
    /// <returns>A driver for the given state that allows special operations on
    /// finite state machines</return>
    public static StateDriver For(Type type) {
      if (type != null) {
        Type[] typeArr = new Type[] { type };
        var genericBase = typeof(StateDriver<>);
        var combined = genericBase.MakeGenericType(typeArr);
        return (StateDriver) Activator.CreateInstance(combined);
      }

      return null;
    }
    #endregion


    #region Abstract Interface
    //-------------------------------------------------------------------------
    // Abstract Interface
    //-------------------------------------------------------------------------
 
 
    /// <summary>
    /// Start the given machine with this state driver's state. If the state
    /// does not exist on the state machine's game object, it will be added.
    /// </summary>
    /// <param name="fsm">The machine to start.</param>
    /// <seealso cref="StateDriver{S}.StartMachine" />
    public abstract void StartMachine(FiniteStateMachine fsm);

    /// <summary>
    /// Whether or not the finite state machine is already in this State
    /// driver's state.
    /// </summary>
    /// <param name="fsm">The state machine to check this for.</param>
    /// <returns>True if the state machine is already in the state.</returns>
    /// <seealso cref="StateDriver{S}.IsInState" />
    public abstract bool IsInState(FiniteStateMachine fsm);

    /// <summary>
    /// Forcibly change the state of the machine.
    /// </summary>
    /// <param name="fsm">The machine to force a state change on.</param>
    /// <seealso cref="StateDriver{S}.ForceStateChangeOn" />
    public abstract void ForceStateChangeOn(FiniteStateMachine fsm);

    /// <summary>
    /// Sample from the animation associated with this state.
    /// </summary>
    /// <param name="fsm">The machine to sample this clip on.</param>
    /// <param name="time">The timestamp of the clip to sample</param>
    /// <seealso cref="StateDriver{S}.SampleClip" />
    public abstract void SampleClip(FiniteStateMachine fsm, float time);
    #endregion
   
  }

  /// <summary>
  /// A class that allows someone to forcibly change the state of a finite state
  /// machine.
  /// </summary>
  /// <typeparam name="S">The type of the state.</typeparam>
  public class StateDriver<S> : StateDriver where S : State, new() {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The animation clip associated with this state.
    /// </summary>
    private AnimationClip cachedClip;
    #endregion


    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

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
      #if UNITY_EDITOR
      if (Application.isPlaying) {
      #endif

        fsm.ChangeState<S>();

      #if UNITY_EDITOR
      }
      #endif
    }

    /// <summary>
    /// Sample from the animation associated with this state.
    /// </summary>
    /// <param name="fsm">The machine to sample this clip on.</param>
    /// <param name="time">The timestamp of the clip to sample</param>
    public override void SampleClip(FiniteStateMachine fsm, float time) {
      if (cachedClip == null) {
        cachedClip = FindClip(fsm);
      }

      cachedClip.SampleAnimation(fsm.gameObject, time);
    }

    /// <summary>
    /// Searches the finite state machine to find the animation clip associated
    /// with this state.
    /// </summary>
    /// <param name="fsm">The finite state machine to search.</param>
    /// <returns>The clip.</returns>
    private AnimationClip FindClip(FiniteStateMachine fsm) {
      // Get the animator controller asset bound to our animator component.
      S state = new S();
      AnimatorController controller = AnimationTools.GetController(fsm.Animator);

      foreach (AnimatorStateTransition t in controller.layers[0].stateMachine.anyStateTransitions) {
        if (t.conditions[0].parameter == state.AnimParam) {
          AnimatorState animState = t.destinationState;
          return (AnimationClip) animState.motion;
        }
      }

      return null;
    }
  }
    #endregion

}