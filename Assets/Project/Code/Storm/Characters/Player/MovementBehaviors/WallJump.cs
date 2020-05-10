using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class WallJump : MovementBehavior
  {

    private HorizontalMotion motion;

    private Vector2 wallJumpLeft;

    private Vector2 wallJumpRight;

    private new Rigidbody2D rigidbody;

    private void Awake() {
      AnimParam = "wall_jump";
    }

    private void Start() {
      MovementSettings settings = GetComponent<MovementSettings>();
      wallJumpRight = settings.WallJump;
      wallJumpLeft = new Vector2(-wallJumpRight.x, wallJumpRight.y);
    }


    public void OnAnimationFinished() {
      ChangeState<Rise>();
    }


    public override void OnStateEnter(PlayerCharacter p) {
      base.OnStateEnter(p);

      motion = GetComponent<HorizontalMotion>();
    }


    public override void OnStateExit(PlayerCharacter p) {
      base.OnStateExit(p);

      rigidbody = player.rigidbody;

      if (player.IsTouchingLeftWall()) {
        rigidbody.velocity = wallJumpRight;
      } else if (player.IsTouchingRightWall()) {
        rigidbody.velocity = wallJumpLeft;
      }

      motion.WallJump();
      Push(MovementSymbol.Jumped);
    }
  }

}

