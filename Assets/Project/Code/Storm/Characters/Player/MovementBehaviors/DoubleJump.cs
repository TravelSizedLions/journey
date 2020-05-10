using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class DoubleJump : MovementBehavior {

    private float jumpForce = 48f;

    private HorizontalMotion motion;

    private Rigidbody2D playerRB;

    private void Awake() {
      AnimParam = "double_jump";
      motion = GetComponent<HorizontalMotion>();
    }

    public void OnAnimationFinished() {
      ChangeState<Rise>();
    }

    public override void HandlePhysics() {
      Facing facing = motion.Handle();
      player.SetFacing(facing);
    }

    public override void OnStateEnter(PlayerCharacter p) {
      base.OnStateEnter(p);

      Push(MovementSymbol.DoubleJumped);
      playerRB = p.GetComponent<Rigidbody2D>();

      // Zero out gravity, then apply jump.
      playerRB.velocity *= Vector2.right;
      playerRB.velocity += new Vector2(0, jumpForce);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      if (player.IsTouchingLeftWall() || player.IsTouchingRightWall()) {
        if (playerRB.velocity.y > 0) {
          ChangeState<WallRun>();
        } else {
          ChangeState<WallSlide>();
        }
      } else if (player.IsTouchingGround()) {
        ChangeState<Land>();
      } 
    }
  }
}