using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class WallSlide : MovementBehavior {

    private float wallSlideDeceleration;

    private new Rigidbody2D rigidbody;

    private void Awake() {
      AnimParam = "wall_slide";


      MovementSettings settings = GetComponent<MovementSettings>();
      wallSlideDeceleration = 1-settings.WallSlideDeceleration;
    }

    public override void OnStateEnter(PlayerCharacter p) {
      base.OnStateEnter(p);

      rigidbody = p.rigidbody;

      stack.Clear();

    }

    public override void HandleInput() {
      if (Input.GetButtonDown("Jump")) {
        ChangeState<WallJump>();
      }
    }

    public override void HandlePhysics() {

      float input = Input.GetAxis("Horizontal");
      float verticalVelocity = rigidbody.velocity.y;

      if (input > 0 && player.IsTouchingLeftWall()) {
        rigidbody.velocity += new Vector2(1,0);
        ChangeState<Fall>();
      } else if (input < 0 && player.IsTouchingRightWall()) {
        rigidbody.velocity += new Vector2(-1,0);
        ChangeState<Fall>();
      } else {
        // Slow down the player's descent.
        rigidbody.velocity = (Vector2.up * rigidbody.velocity)*wallSlideDeceleration;
      }
    }

    private void OnCollisionStay2D(Collision2D collision) {
      if (player.IsTouchingGround()) {
        ChangeState<Idle>();
      }
    }
  }

}