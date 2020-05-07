using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {
  public class Idle : MovementBehavior {

    #region Movement Behavior Implementation
    //-------------------------------------------------------------------------
    // Movement Behavior Implementation
    //-------------------------------------------------------------------------

    public override void OnStateEnter(PlayerCharacter player) {
      base.OnStateEnter(player);
    }

    public override void HandleInput() {
      if (Input.GetButtonDown("Jump")) {
        player.TriggerAnimation();
        ChangeState<Jump>();
      }
    }

    #endregion
  }

}