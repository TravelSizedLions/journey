using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public class CarryJumpFall : CarryMotion {

    private void Awake() {
      AnimParam = "carry_jump_fall";
    }

    public override void OnUpdate() {
      if (player.PressedJump()) {
        if (player.InCoyoteTime()) {
          player.UseCoyoteTime();
          ChangeToState<CarryJumpStart>();
        } else {
          base.TryBufferedJump();
        }
      } else if (player.PressedAction()) {
        ChangeToState<MidAirThrowItem>();
      }
    }

    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (player.IsTouchingGround()) {
        if (player.TryingToMove()) {
          ChangeToState<CarryRun>();
        } else if (player.HoldingDown()) {
          ChangeToState<CarryCrouchStart>();
        } else {
          ChangeToState<CarryLand>();
        }
      }
    }
  }

}