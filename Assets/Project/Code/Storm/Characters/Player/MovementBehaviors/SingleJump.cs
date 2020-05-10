using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class SingleJump : MovementBehavior {

    private float jumpForce = 48f;

    private HorizontalMotion motion;

    private void Awake() {
      AnimParam = "single_jump";
      motion = GetComponent<HorizontalMotion>();
    }

    public void OnAnimationFinished() {
      Push(MovementSymbol.Jumped);
      if (player.IsTouchingLeftWall() || player.IsTouchingRightWall()) {
        ChangeState<WallRun>();
      } else {
        ChangeState<Rise>();
      }
      
    }

    public override void HandleInput() {
      if (Input.GetButtonDown("Jump")) {
        if (TryConsume(MovementSymbol.Jumped)) {
          Push(MovementSymbol.DoubleJumped);
          ChangeState<DoubleJump>();
        }
      }
    }

    public override void HandlePhysics() {
      Facing facing = motion.Handle();
      player.SetFacing(facing);
    }

    public override void OnStateEnter(PlayerCharacter p) {
      base.OnStateEnter(p);
    }

    public override void OnStateExit(PlayerCharacter p) {
      base.OnStateExit(p);

      Rigidbody2D playerRB = p.GetComponent<Rigidbody2D>();
      playerRB.velocity = (playerRB.velocity * Vector2.right) + new Vector2(0, jumpForce);

    }
  }
}