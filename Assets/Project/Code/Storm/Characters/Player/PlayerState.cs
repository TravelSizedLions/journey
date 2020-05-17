using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {

  public abstract class PlayerState : MonoBehaviour {

    protected string AnimParam = "";

    /// <summary>
    /// A reference to the player character.
    /// </summary>
    protected PlayerCharacter player;


    public virtual void HiddenOnStateAdded() {
      player = GetComponent<PlayerCharacter>();
      OnStateAdded();
    }


    /// <summary>
    /// First time initialization for the state.
    /// </summary>
    public virtual void OnStateAdded() {

    }

    public virtual void EnterState() {
      enabled = true;

      if (string.IsNullOrEmpty(AnimParam)) {
        throw new UnityException(string.Format("Please set {0}.AnimParam to the name of the animation parameter in the  behavior's Awake() method.", this.GetType()));
      }

      player.SetAnimParam(AnimParam);
      OnStateEnter();
    }


    public virtual void OnStateEnter() {

    }

    public virtual void ExitState() {
      OnStateExit();
      enabled = false;
    }

    public virtual void OnStateExit() {

    }



    public virtual void OnUpdate() {

    }

    public virtual void OnFixedUpdate() {

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
  }

}