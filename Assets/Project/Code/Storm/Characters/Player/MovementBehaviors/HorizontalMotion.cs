using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {

  public class HorizontalMotion : MonoBehaviour {

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

    private new Rigidbody2D rigidbody;

    private void Awake() {

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

      rigidbody = GetComponent<Rigidbody2D>();
    }

    public Facing Handle() {
      float input = Input.GetAxis("Horizontal");

      if (Mathf.Abs(input) != 1 && !isWallJumping) {
        rigidbody.velocity *= decelerationForce;
      }

      // factor in turn around time.
      float inputDirection = Mathf.Sign(input);
      float motionDirection = Mathf.Sign(rigidbody.velocity.x);
      float adjustedInput = (inputDirection == motionDirection) ? (input) : (input*agility);

      if (isWallJumping) {
        adjustedInput *= wallJumpMuting;
      }

      float horizSpeed = Mathf.Clamp(rigidbody.velocity.x + (adjustedInput*accelerationFactor), -maxSpeed, maxSpeed);
      rigidbody.velocity = new Vector2(horizSpeed, rigidbody.velocity.y);

      if (Mathf.Abs(rigidbody.velocity.x) < idleThreshold) {
        return Facing.None;
      } else {
        return (Facing)Mathf.Sign(rigidbody.velocity.x);
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