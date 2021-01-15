using System;
using UnityEngine;

namespace HumanBuilders {
  public abstract class State : MonoBehaviour {

    #region Fields
    /// <summary>
    /// The animation trigger parameter for this state.
    /// </summary>
    /// <remarks> 
    /// For
    /// the sake of reducing parity errors between the animation controller's transitions
    /// and the state transitions in your code, The finite state machine
    /// system makes the assumption that your controller is set up such
    /// that:
    /// <list type="bullet">
    ///   <item>
    ///     Each state in your <seealso cref="FiniteStateMachine"/> should map
    ///     to one animation state in your animation controller (States can share an
    ///     animation, but one state shouldn't have multiple animations).
    ///   </item>
    ///   <item>
    ///     Each state can transition to any other state
    ///   </item>
    ///   <item> 
    ///     Each state has a trigger parameter that causes a transition to that state
    ///   </item>
    ///   <item> 
    ///     There are no other types of parameters
    ///   </item>
    ///   <item>
    ///     You will not make transitions through Animator.SetTrigger()
    ///   </item>
    /// </list>
    /// 
    /// When creating your own state behaviors, you should specify AnimParam as the name of
    /// the trigger parameter that causes
    /// the transition to the state (not the name of the state itself!). This should be done in the Awake() method, instead of through field initialization.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class IdleState : State {
    ///   private void Awake() {
    ///     // This will be used to automatically transition to/from the state this
    ///     // class represents.
    ///     AnimParam = "idle_trigger";
    ///   }
    /// 
    ///   ...
    /// }
    /// </code>
    /// </example>
    public abstract string AnimParam { get; }

    /// <summary>
    /// The state machine this state belongs to.
    /// </summary>
    protected IStateMachineInternal FSM;

    /// <summary>
    /// Whether or not the state has already transitioned to another state. 
    /// </summary>
    /// <remarks>
    /// This value will be false during <seealso cref="OnStateAdded()" /> and
    /// <seealso cref="OnStateAddedGeneral()" />, and
    /// will be true before <seealso cref="OnStateExit()" /> and <seealso cref="OnStateExitGeneral()"/>.
    /// 
    /// It's generally used to prevent race conditions between animation event callbacks and
    /// the <seealso cref="OnSignal(GameObject)" /> method, where the animation callback may try to
    /// transition to a new state after the state has already left.
    /// </remarks>
    protected bool exited;
    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      
    }
    #endregion

    #region Virtual Methods. Override these in your State classes.
    /// <summary>
    /// First time initialization that is common to all states that will belong to a specific implentation of a state machine.
    /// Ex. PlayerStates will always need to get a reference to the player.
    /// </summary>
    public virtual void OnStateAddedGeneral() { }

    /// <summary>
    /// First time initialization for a specific state. Dependencies common to all states should have been added by this point.
    /// </summary>
    public virtual void OnStateAdded() { }

    /// <summary>
    /// Behavior for entering a state that is common to all states that will belong to a specific
    /// implementation of a state machine. Fires right before <see cref="OnStateEnter()" />
    /// </summary>
    public virtual void OnStateEnterGeneral() { }

    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public virtual void OnStateEnter() { }

    /// <summary>
    /// Behavior for exiting a state that is common to all states that will
    /// belong to a specific implementation of a state machine. Fires right
    /// after <see cref="OnStateExit()" />.
    /// </summary>
    public virtual void OnStateExitGeneral() { }

    /// <summary>
    /// Fires when the state exits, before the next state is entered into.
    /// </summary>
    public virtual void OnStateExit() { }

    /// <summary>
    /// Behavior that fires once per frame that's common to all states belonging
    /// to a specific state machine. Fires just prior to <see cref="OnUpdate()"/>.
    /// </summary>
    public virtual void OnUpdateGeneral() { }

    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public virtual void OnUpdate() { }

    /// <summary>
    /// Behavior that fires once per physics tick that's common to all states
    /// belonging to a specific state machine. Fires just prior to <see cref="State.OnFixedUpdate"/>
    /// </summary>
    public virtual void OnFixedUpdateGeneral() { }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public virtual void OnFixedUpdate() { }

    /// <summary>
    /// Fires when code outside the state machine is trying to send information.
    /// </summary>
    /// <param name="signal">The signal sent.</param>
    /// <seealso cref="exited" />
    public virtual void OnSignal(GameObject obj) { }

    /// <summary>
    /// 
    /// Fires when code outside the state machine is trying to send information.
    /// This should contain logic that should run for all states that belong to
    /// a specific state machine. Fires just prior to <see cref="State.Signal"/>
    /// </summary>
    /// <param name="obj"></param>
    public virtual void OnSignalGeneral(GameObject obj) { }

    #endregion

    #region Getters/Setters

    /// <summary>
    /// Point of injection for testing.
    /// </summary>
    /// <param name="stateMachine">The state machine.</param>
    public void Inject(IStateMachineInternal stateMachine) {
      this.FSM = stateMachine;
    }

    /// <summary>
    /// Get the animation trigger for this state.
    /// </summary>
    public string GetAnimParam() {
      return AnimParam;
    }

    #endregion

    #region Methods that interact with the State Machine.

    /// <summary>
    /// Pre-hook called by the Player Character when a player state is first added to the player.
    /// </summary>
    public void HiddenOnStateAdded(IStateMachineInternal stateMachine) {
      FSM = stateMachine;
      OnStateAddedGeneral();
      OnStateAdded();
    }

    /// <summary>
    /// Pre-hook called by the Player Character when a player enters a given state.
    /// </summary>
    public void EnterState() {
      enabled = true;

      if (string.IsNullOrEmpty(AnimParam)) {
        Debug.LogWarning(string.Format("Please set {0}.AnimParam to the name of the animation parameter for this state in the behavior's Awake() method.", this.GetType()));
        return;
      }


      // Debug.Log("anim param: " + AnimParam);
      FSM.SetAnimParam(AnimParam);
      exited = false;
      OnStateEnterGeneral();
      OnStateEnter();
    }

    /// <summary>
    /// Pre-hook called by the Player Character when a player exits a given state.
    /// </summary>
    public void ExitState() {
      exited = true;
      OnStateExit();
      OnStateExitGeneral();
      enabled = false;
    }

    /// <summary>
    /// Change state. The old state behavior will be detached from the player after this call.
    /// </summary>
    public void ChangeToState<S>() where S : State {
      if (FSM != null && FSM.Running) {
        S state;

        // Add the state if it's not already there.
        if (FSM.ContainsState<S>()) {
          state = FSM.GetState<S>();
          
        } else {
          state = gameObject.AddComponent<S>();
          FSM.RegisterState(state);
          state.HiddenOnStateAdded(FSM);
        }

        FSM.OnStateChange(this, state);
      }
    }
    #endregion
  }
}