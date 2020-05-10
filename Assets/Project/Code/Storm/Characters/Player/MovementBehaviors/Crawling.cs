using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class Crawling : MovementBehavior {

    private float crawlSpeed;

    private float idleThreshold;

    private new Rigidbody2D rigidbody;

    private void Awake() {
      AnimParam = "crawling";
    }

    private void Start() {
      MovementSettings settings = GetComponent<MovementSettings>();
      crawlSpeed = settings.CrawlSpeed;
      idleThreshold = settings.IdleThreshold;

      rigidbody = player.rigidbody;
    }

    public override void HandleInput() {
      if (!Input.GetButton("Down")) {
        ChangeState<Running>();
      }
    }

    public override void HandlePhysics() {
      float input = Input.GetAxis("Horizontal");

      if (Mathf.Abs(input) == 0) {
        rigidbody.velocity = Vector2.zero;
        ChangeState<Crouching>();
        return;
      }

      float inputDirection = Mathf.Sign(input);
      rigidbody.velocity = new Vector2(inputDirection*crawlSpeed, rigidbody.velocity.y);

      if (Mathf.Abs(rigidbody.velocity.x) < idleThreshold) {
        player.SetFacing(Facing.None);
      } else {
        player.SetFacing((Facing)inputDirection);
      }
    }
    private void OnCollisionExit2D(Collision2D collision) {
      if (collision.collider.CompareTag("Ground")) {
        if (!player.IsTouchingGround()) {
          ChangeState<Fall>();
        }
      }
    }


  }
}