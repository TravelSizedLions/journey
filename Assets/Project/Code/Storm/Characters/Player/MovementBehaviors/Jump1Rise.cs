using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class Jump1Rise : HorizontalMotion {


    private void Awake() {
      AnimParam = "jump_1_rise";
    }

    public override void OnUpdate() {
      if (Input.GetButtonDown("Jump")) {
        ChangeToState<Jump2Start>();
      }
    }

    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();

      player.SetFacing(facing);
      
      if (playerRB.velocity.y < 0) {
        ChangeToState<Jump1Fall>();
      }
    }


    private void OnCollisionEnter2D(Collision2D collision) {
      if (enabled) {
        if (player.IsTouchingLeftWall() || player.IsTouchingRightWall()) {
          ChangeToState<WallRun>();
        }
      }
    }
  }

}
