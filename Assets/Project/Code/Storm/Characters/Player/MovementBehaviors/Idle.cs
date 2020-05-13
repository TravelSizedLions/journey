using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {
  public class Idle : MovementBehavior {

    private void Awake() {
      AnimParam = "idle";
    }


    private void OnCollisionStay2D(Collision2D other) {
      if (!player.IsTouchingGround()) {
        if (player.IsTouchingLeftWall() || player.IsTouchingRightWall()) {
          ChangeState<WallSlide>();
        }
      }
    }


    #region Movement Behavior Implementation
    //-------------------------------------------------------------------------
    // Movement Behavior Implementation
    //-------------------------------------------------------------------------

    public override void OnStateEnter(PlayerCharacter player) {
      base.OnStateEnter(player);
    }

    public override void HandleInput() {
      if (Input.GetButton("Jump")) {
        ChangeState<SingleJump>();
      } else if (Input.GetAxis("Horizontal") != 0) {
        ChangeState<Running>();
      } else if (Input.GetButton("Down")) {
        ChangeState<StartCrouch>();
      }
    }

    #endregion
  }

}