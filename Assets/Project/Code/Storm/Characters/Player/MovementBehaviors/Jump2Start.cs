using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {
  public class Jump2Start : HorizontalMotion {

    private void Awake() {
      AnimParam = "jump_2_start";
    }


    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      if (player.IsTouchingGround()) {
        float xVel = playerRB.velocity.x;
        if (Mathf.Abs(xVel) > idleThreshold) {
          ChangeToState<RollStart>();
        } else {
          ChangeToState<Land>();
        }
      } else if (player.IsTouchingLeftWall() || player.IsTouchingRightWall()) {
        if (playerRB.velocity.y > 0) {
          ChangeToState<WallRun>();
        } else  {
          ChangeToState<WallSlide>();
        }
      }
    }

    public void OnDoubleJumpFinished() {
      bool touchingWall = player.IsTouchingLeftWall() || player.IsTouchingRightWall();
      if (playerRB.velocity.y > 0) {
        ChangeToState<Jump2Rise>();
      } else {  
        ChangeToState<Jump2Fall>();
      }
    }

    public override void OnStateEnter() {
      MovementSettings settings = GetComponent<MovementSettings>();

      playerRB.velocity = (playerRB.velocity*Vector2.right) + (Vector2.up*settings.DoubleJumpForce);
    }

  }

}