using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {

  /// <summary>
  /// When the player prepares to do a double jump.
  /// </summary>
  public class DoubleJumpStart : HorizontalMotion {

    private void Awake() {
      AnimParam = "jump_2_start";
    }


    public override void OnUpdate() {
      if (player.PressedJump()) {
        if (player.InCoyoteTime()) {
          player.UseCoyoteTime();
          ChangeToState<SingleJumpStart>();
        } else {
          base.TryBufferedJump();
        }
      }
    }

    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      if (player.IsTouchingGround()) {

        if (Mathf.Abs(physics.Vx) > idleThreshold) {
          ChangeToState<RollStart>();
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

    public void OnDoubleJumpFinished() {
      if (player.IsRising()) {
        ChangeToState<DoubleJumpRise>();
      } else {  
        ChangeToState<DoubleJumpFall>();
      }
    }

    public override void OnStateEnter() {
      MovementSettings settings = GetComponent<MovementSettings>();
      physics.Vy = settings.DoubleJumpForce;

      player.DisablePlatformMomentum();
    }

  }

}