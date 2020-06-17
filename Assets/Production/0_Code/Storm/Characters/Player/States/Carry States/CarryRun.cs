using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public class CarryRun : CarryMotion {

    private void Awake() {
      AnimParam = "carry_run";
    }


    public override void OnUpdate() {
      if (player.PressedJump()) {
        ChangeToState<CarryJumpStart>();
      }
    }

    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (facing == Facing.None) {
        ChangeToState<CarryIdle>();
      } else if (!player.IsTouchingGround() && player.IsFalling()) {
        player.StartCoyoteTime();
        ChangeToState<CarryJumpFall>();
      }
    }

  }

}