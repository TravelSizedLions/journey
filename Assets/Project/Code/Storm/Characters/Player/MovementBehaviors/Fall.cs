using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {

  public class Fall : MovementBehavior {

    private Rigidbody2D playerRB;

    private HorizontalMotion motion;

    private void Awake() {
      AnimParam = "fall";

      motion = GetComponent<HorizontalMotion>();
    }

    public override void OnStateEnter(PlayerCharacter p) {
      base.OnStateEnter(p);
      
      playerRB = p.GetComponent<Rigidbody2D>();
    }


    public override void HandleInput() {
      if (Input.GetButtonDown("Jump")) {
        if (TryConsume(MovementSymbol.Jumped)) {
          ChangeState<DoubleJump>();
        } else {
          Push(MovementSymbol.Jumped);
          ChangeState<SingleJump>();
        }
      }
    }


    public override void HandlePhysics() {
      Facing facing = motion.Handle();
      player.SetFacing(facing);
    }

    private void OnCollisionStay2D(Collision2D collision) {
      if (collision.collider.CompareTag("Ground")) {
        if (player.IsTouchingGround()) {
          ChangeState<Land>();
          return;
        } else if (player.IsTouchingRightWall()) {
          player.SetFacing(Facing.Right);
          ChangeState<WallSlide>();
          return;
        } else if (player.IsTouchingLeftWall()) {
          player.SetFacing(Facing.Left);
          ChangeState<WallSlide>();
          return;
        }
      }
    }
  }
}