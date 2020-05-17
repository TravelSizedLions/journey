using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {
  public class Jump2Rise : HorizontalMotion {


    private void Awake() {
      AnimParam = "jump_2_rise";
    }

    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (player.IsTouchingRightWall() || player.IsTouchingLeftWall()) {
        ChangeToState<WallRun>();
      } else if (playerRB.velocity.y < 0) {
        ChangeToState<Jump2Fall>();
      }
    }


  }

}