using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class HorizontalMotion : PlayerState {

    private float maxSpeed;

    private float maxSqrVelocity;

    private float acceleration;

    private float accelerationFactor;

    private float deceleration;

    private Vector2 decelerationForce;

    private float agility;

    private float idleThreshold;

    private float wallJumpMuting;

    private bool isWallJumping;

    protected Rigidbody2D playerRB;


    public override void OnStateAdded() {
      playerRB = player.GetComponent<Rigidbody2D>();


      MovementSettings settings = GetComponent<MovementSettings>();

      maxSpeed = settings.MaxSpeed;
      maxSqrVelocity = maxSpeed*maxSpeed;

      acceleration = settings.Acceleration;
      accelerationFactor = maxSpeed*acceleration;

      deceleration = settings.Deceleration;
      decelerationForce = decelerationForce = new Vector2(1-deceleration, 1);

      agility = settings.Agility;

      idleThreshold = settings.IdleThreshold;

      wallJumpMuting = settings.WallJumpMuting;

    }


    public Facing MoveHorizontally() {
      float input = Input.GetAxis("Horizontal");

      if (Mathf.Abs(input) != 1 && !isWallJumping) {
        playerRB.velocity *= decelerationForce;
      }

      // factor in turn around time.
      float inputDirection = Mathf.Sign(input);
      float motionDirection = Mathf.Sign(playerRB.velocity.x);
      float adjustedInput = (inputDirection == motionDirection) ? (input) : (input*agility);

      if (isWallJumping) {
        adjustedInput *= wallJumpMuting;
      }

      float horizSpeed = Mathf.Clamp(playerRB.velocity.x + (adjustedInput*accelerationFactor), -maxSpeed, maxSpeed);
      playerRB.velocity = new Vector2(horizSpeed, playerRB.velocity.y);

      if (Mathf.Abs(playerRB.velocity.x) < idleThreshold) {
        return Facing.None;
      } else {
        return (Facing)Mathf.Sign(playerRB.velocity.x);
      }

    }



    public void WallJump() {
      isWallJumping = true;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      isWallJumping = false;
    }
  }
}