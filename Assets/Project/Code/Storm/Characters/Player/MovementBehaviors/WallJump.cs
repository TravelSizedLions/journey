using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class WallJump : HorizontalMotion {


    private Vector2 wallJumpLeft;

    private Vector2 wallJumpRight;


    private void Awake() {
      AnimParam = "wall_jump";
    }

    public void OnWallJumpFinished() {
      ChangeToState<Jump1Rise>();
    }

    public override void OnStateAdded() {
      base.OnStateAdded();
      
      MovementSettings settings = GetComponent<MovementSettings>();

      wallJumpRight = settings.WallJump;
      wallJumpLeft = new Vector2(-wallJumpRight.x, wallJumpRight.y);
    }

    public override void OnStateExit() {
      WallJump();
      if (player.IsTouchingLeftWall()) {
        playerRB.velocity = wallJumpRight;
      } else {
        playerRB.velocity = wallJumpLeft;
      }
    }
  }
}