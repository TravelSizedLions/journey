using System.Collections;
using System.Collections.Generic;
using Storm.Flexible;
using UnityEngine;

namespace Storm.Characters.Player {

  public class ThrowItem : HorizontalMotion {
    private Vector2 throwForce;
    private float verticalThrowForce;

    private void Awake() {
      AnimParam = "carry_run_throw";
    }

    public override void OnUpdate() {
      if (player.PressedJump()) {
        if (player.IsTouchingLeftWall() || player.IsTouchingRightWall()) {
          ChangeToState<WallRun>();
        } else {
          ChangeToState<SingleJumpStart>();
        }
      } else if (player.HoldingDown()) {
        ChangeToState<Dive>();
      }
    }

    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (facing == Facing.None) {
        ChangeToState<Idle>();
      } else if (!player.IsTouchingGround() && player.IsFalling()) {
        player.StartCoyoteTime();
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
        item.Physics.Vy = settings.ThrowForce.y;
      }

      if (player.Facing == Facing.Right) {
        item.Physics.Vx = settings.ThrowForce.x;
      } else {
        item.Physics.Vx = -settings.ThrowForce.x;
      }
      
      item.Physics.Vx += player.Physics.Vx;
    }

    public void OnThrowItemFinished() {
      ChangeToState<Running>();
    }

  }
}