using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {

  /// <summary>
  /// The base class for player states.
  /// </summary>
  public abstract class PlayerState : MonoBehaviour {

    #region Fields
    /// <summary>
    /// The name of the trigger in the animator controller that enters this state. Set it in either the Awake() or OnStateAdded() functions.
    /// </summary>
    protected string AnimParam = "";

    /// <summary>
    /// A reference to the player character.
    /// </summary>
    protected PlayerCharacter player;

    /// <summary>
    /// The player's rigidbody component.
    /// </summary>
    protected new Rigidbody2D rigidbody;
    #endregion

    #region Player State Overrides

    /// <summary>
    /// First time initialization for the state. A reference to the player and the player's rigidbody will already have been added by this point.
    /// </summary>
    public virtual void OnStateAdded() {

    }

    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public virtual void OnStateEnter() {

    }

    /// <summary>
    /// Fires when the state exits, before the next state is entered into.
    /// </summary>
    public virtual void OnStateExit() {

    }


    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public virtual void OnUpdate() {

    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public virtual void OnFixedUpdate() {

    }

    #endregion

    #region Player Character Interfacing

    /// <summary>
    /// Pre-hook called by the Player Character when a player state is first added to the player.
    /// </summary>
    public void HiddenOnStateAdded() {
      player = GetComponent<PlayerCharacter>();
      rigidbody = player.rigidbody;

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

      player.SetAnimParam(AnimParam);
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
    protected void ChangeToState<State>() where State : PlayerState {
      Debug.Log(typeof(State));

      // Add the state if it's not already there.
      State state = player.gameObject.GetComponent<State>();

      if (state == null) {
        state = player.gameObject.AddComponent<State>();
        state.HiddenOnStateAdded();
      }

      player.OnStateChange(this, state);
    }
    
    #endregion
  }

}