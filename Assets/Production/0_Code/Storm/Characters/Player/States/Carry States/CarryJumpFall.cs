using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public class CarryJumpFall : CarryMotion {

    private bool releasedAction;

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
      } else if (player.PressedAction() && releasedAction) {
        ChangeToState<MidAirThrowItem>();
      } else if (player.ReleasedAction()) {
        releasedAction = true;
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

    public override void OnStateEnter() {
      releasedAction = !player.HoldingAction();
    }
  }

}