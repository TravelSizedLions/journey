using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {

  public abstract class PlayerBehavior : MonoBehaviour {

    protected string AnimParam = "";

    /// <summary>
    /// A reference to the player character.
    /// </summary>
    protected PlayerCharacter player;

    public virtual void OnStateEnter(PlayerCharacter p) {
      this.player = p;

      if (string.IsNullOrEmpty(AnimParam)) {
        throw new UnityException(string.Format("Please set {0}.AnimParam to the name of the animation parameter in the  behavior's Awake() method.", this.GetType()));
      }

      p.SetAnimParam(AnimParam, true);
    }

    public virtual void OnStateExit(PlayerCharacter p) {
      p.SetAnimParam(AnimParam, false);
    }

    public virtual void HandleInput() {

    }
  }

}