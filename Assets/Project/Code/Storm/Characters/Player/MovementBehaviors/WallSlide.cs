using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class WallSlide : MovementBehavior {

    private new Rigidbody2D rigidbody;

    private void Awake() {
      AnimParam = "wall_slide";
    }

    public override void OnStateEnter(PlayerCharacter p) {
      base.OnStateEnter(p);

      rigidbody = p.rigidbody;

      TryConsume(MovementSymbol.DoubleJumped);
      TryConsume(MovementSymbol.Jumped);

    }

    public override void HandlePhysics() {
      float input = Input.GetAxis("Horizontal");
      float verticalVelocity = rigidbody.velocity.y;

      if (input > 0 && player.IsTouchingLeftWall()) {
        ChangeState<Fall>();
      } else if (input < 0 && player.IsTouchingRightWall()) {
        ChangeState<Fall>();
      } else {
        rigidbody.velocity = Vector2.up * rigidbody.velocity;
      }
    }

    private void OnCollisionStay2D(Collision2D collision) {
      if (collision.collider.CompareTag("Ground")) {
        if (player.IsTouchingGround()) {
          ChangeState<Idle>();
        }
      }
    }

  }

}