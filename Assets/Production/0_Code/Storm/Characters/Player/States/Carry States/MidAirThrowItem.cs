using System.Collections;
using System.Collections.Generic;
using Storm.Flexible;
using UnityEngine;


namespace Storm.Characters.Player {
  public class MidAirThrowItem : HorizontalMotion {

    private void Awake() {
      AnimParam = "carry_jump_throw";
    }


    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (player.IsTouchingGround()) {
        if (player.TryingToMove()) {
          ChangeToState<Running>();
        } else {
          ChangeToState<Land>();
        }
      } else if (player.IsTouchingLeftWall() || player.IsTouchingRightWall()) {
        if (player.IsRising()) {
          ChangeToState<WallRun>();
        } else  {
          ChangeToState<WallSlide>();
        }
      }
    }

    public void OnMidAirThrowItemFinished() {
      if (player.IsRising()) {
        ChangeToState<SingleJumpRise>();
      } else {
        ChangeToState<SingleJumpFall>();
      }
    }


    public override void OnStateEnter() {
      Carriable item = player.CarriedItem;
      item.OnThrow();

      CarrySettings settings = GetComponent<CarrySettings>();
      if (player.HoldingUp()) {
        item.Physics.Vy = settings.VerticalThrowForce;
      } else {
        if (player.Facing == Facing.Right) {
          item.Physics.Vx = settings.ThrowForce.x;
        } else {
          item.Physics.Vx = -settings.ThrowForce.x;
        }
        item.Physics.Vy = settings.ThrowForce.y;
      }
    }
  }

}