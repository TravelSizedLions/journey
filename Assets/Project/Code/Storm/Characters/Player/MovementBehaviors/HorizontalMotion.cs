using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// Shared behavior for player states that allow the player to move left/right.
  /// </summary>
  public class HorizontalMotion : PlayerState {

    #region Fields
    /// <summary>
    /// The player's maximum horizontal movement speed.
    /// </summary>
    private float maxSpeed;

    /// <summary>
    /// The squared velocity of the player's max speed.
    /// </summary>
    private float maxSqrVelocity;

    /// <summary>
    /// How quickly the player accelerates to top speed, as a fraction of the player's top speed.
    /// </summary>
    private float acceleration;

    /// <summary>
    /// 
    /// </summary>
    private float accelerationFactor;

    /// <summary>
    /// How quickly the player decelerates.
    /// </summary>
    private float deceleration;

    /// <summary>
    /// The deceleration of the player in terms of a force vector.
    /// </summary>
    private Vector2 decelerationForce;

    /// <summary>
    /// How quickly the player turns around while running.
    /// </summary>
    private float agility;

    /// <summary>
    /// How slow the player needs to be moving to switch back to idle state.
    /// </summary>
    protected float idleThreshold;

    /// <summary>
    /// Instantaneous deceleration to facilitate wall jumping.
    /// </summary>
    private float wallJumpMuting;

    /// <summary>
    /// Whether or not the player is wall-jumping.
    /// </summary>
    protected static bool isWallJumping;

    #endregion

    #region Unity API
    private void OnCollisionStay2D(Collision2D collision) {
      if (player.IsTouchingGround()) {
        isWallJumping = false;
      }
    }
    #endregion


    #region Player State API
    /// <summary>
    /// First time initialization for the state. A reference to the player and the player's rigidbody will already have been added by this point.
    /// </summary>
    public override void OnStateAdded() {

      MovementSettings settings = GetComponent<MovementSettings>();

      maxSpeed = settings.MaxSpeed;
      maxSqrVelocity = maxSpeed*maxSpeed;

      acceleration = settings.Acceleration;
      accelerationFactor = maxSpeed*acceleration;

      deceleration = settings.Deceleration;
      decelerationForce = new Vector2(1-deceleration, 1);

      agility = settings.Agility;

      idleThreshold = settings.IdleThreshold;

      wallJumpMuting = settings.WallJumpMuting;

    }
    #endregion

    #region Other Methods
    /// <summary>
    /// Translate user input into horizontal motion.
    /// </summary>
    /// <returns>Which direction the player should be facing.</returns>
    public Facing MoveHorizontally() {
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

    /// <summary>
    /// Perform setup for a wall jump.
    /// </summary>
    public void WallJump() {
      isWallJumping = true;
    }
    #endregion
  }
}