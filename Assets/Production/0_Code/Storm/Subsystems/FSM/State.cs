using System;
using UnityEngine;

namespace Storm.Subsystems.FSM {
  public class State : MonoBehaviour {

    #region Fields
    /// <summary>
    /// The animation trigger for this state.
    /// </summary>
    protected string AnimParam = "";

    /// <summary>
    /// The state machine this state belongs to.
    /// </summary>
    protected IStateMachine FSM;
    #endregion

    #region Virtual Methods
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
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public virtual void OnStateEnter() { }

    /// <summary>
    /// Fires when the state exits, before the next state is entered into.
    /// </summary>
    public virtual void OnStateExit() { }


    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public virtual void OnUpdate() { }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public virtual void OnFixedUpdate() { }

    /// <summary>
    /// Fires when code outside the state machine is trying to send information.
    /// </summary>
    /// <param name="signal">The signal sent.</param>
    public virtual void OnSignal(GameObject obj) { }

    #endregion

    #region Getters/Setters

    /// <summary>
    /// Point of injection for testing.
    /// </summary>
    /// <param name="stateMachine">The state machine.</param>
    public void Inject(IStateMachine stateMachine) {
      this.FSM = stateMachine;
    }

    /// <summary>
    /// Get the animation trigger for this state.
    /// </summary>
    /// <returns></returns>
    public string GetAnimParam() {
      return AnimParam;
    }

    #endregion

    #region Methods that interact with the State Machine.

    /// <summary>
    /// Pre-hook called by the Player Character when a player state is first added to the player.
    /// </summary>
    public void HiddenOnStateAdded(IStateMachine stateMachine) {
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
        throw new UnityException(string.Format("Please set {0}.AnimParam to the name of the animation parameter in the  behavior's Awake() method.", this.GetType()));
      }

      Debug.Log("anim param: " + AnimParam);
      FSM.SetAnimParam(AnimParam);
      OnStateEnter();
    }

    /// <summary>
    /// Pre-hook called by the Player Character when a player exits a given state.
    /// </summary>
    public void ExitState() {
      OnStateExit();
      enabled = false;
    }

    /// <summary>
    /// Change state. The old state behavior will be detached from the player after this call.
    /// </summary>
    public void ChangeToState<S>() where S : State {

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
    #endregion
  }
}