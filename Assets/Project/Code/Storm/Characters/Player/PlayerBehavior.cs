using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {

  public abstract class PlayerBehavior : MonoBehaviour {

      /// <summary>
      /// A reference to the player character.
      /// </summary>
      protected PlayerCharacter player;

      public virtual void OnStateEnter(PlayerCharacter p) {
        this.player = p;
        this.hideFlags = HideFlags.HideInInspector;
      }

      public virtual void OnStateExit(PlayerCharacter p) {

      }

      public virtual void HandleInput() {

      }
  }

}