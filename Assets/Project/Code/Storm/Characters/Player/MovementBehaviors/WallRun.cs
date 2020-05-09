using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {

  public class WallRun : MovementBehavior {

    private new Rigidbody2D rigidbody;

    private void Awake() {
      AnimParam = "wall_run";
    }

    public override void HandlePhysics() {
      float input = Input.GetAxis("Horizontal");
      float verticalVelocity = rigidbody.velocity.y;

      if (input > 0 && player.IsTouchingLeftWall()) {
        SwitchState(verticalVelocity);
      } else if (input < 0 && player.IsTouchingRightWall()) {
        SwitchState(verticalVelocity);
      } else {
        rigidbody.velocity = Vector2.up * rigidbody.velocity;
      }

      if (verticalVelocity < 0) {
        ChangeState<WallSlide>();
      }
    }

    private void SwitchState(float verticalVelocity) {
      if (verticalVelocity > 0) {
        ChangeState<Rise>();
      } else {
        ChangeState<Fall>();
      }
    }

    public override void OnStateEnter(PlayerCharacter p) {
      base.OnStateEnter(p);

      TryConsume(MovementSymbol.DoubleJumped);
      TryConsume(MovementSymbol.Jumped);

      rigidbody = p.rigidbody;
    }
  }

}