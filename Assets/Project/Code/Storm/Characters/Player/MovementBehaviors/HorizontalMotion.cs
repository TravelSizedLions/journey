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
    /// The player acceleration in terms of units/sec^2
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
    /// Whether or not the player is jumping from a wall.
    /// </summary>
    private static bool isWallJumping;

    /// <summary>
    /// Instantaneous deceleration to facilitate wall jumping.
    /// </summary>
    private float wallJumpMuting;

    /// <summary>
    /// How close the player has to be to the ground in order to register another jump.
    /// </summary>
    private float groundJumpBuffer;

    /// <summary>
    /// How close the player has to be to a wall in order to register another wall jump.
    /// </summary>
    private float wallJumpBuffer;

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

      groundJumpBuffer = settings.GroundJumpBuffer;

      wallJumpBuffer = settings.WallJumpBuffer;
    }
    #endregion

    #region Other Methods
    /// <summary>
    /// Translate user input into horizontal motion.
    /// </summary>
    /// <returns>Which direction the player should be facing.</returns>
    public Facing MoveHorizontally() {
      float input = player.GetHorizontalInput();
      bool movingEnabled = player.CanMove();

      TryDecelerate(input, isWallJumping, movingEnabled, player.IsTouchingGround());

      if (!movingEnabled) {
        return GetFacing();
      }

      TryUnparentTransform(player.IsPlatformMomentumEnabled(), input);

      // factor in turn around time.
      float inputDirection = Mathf.Sign(input);
      float motionDirection = Mathf.Sign(physics.Vx);
      float adjustedInput = (inputDirection == motionDirection) ? (input) : (input*agility);

      if (isWallJumping) {
        adjustedInput *= wallJumpMuting;
      }

      float horizSpeed = Mathf.Clamp(physics.Vx + (adjustedInput*accelerationFactor), -maxSpeed, maxSpeed);
      physics.Vx = horizSpeed;
      
      return GetFacing();
    }

    public bool TryUnparentTransform(bool platformMomentumEnabled, float input) {
      // Prevents the player from being dragged around by a platform they 
      // may have been parented to.
      if (!platformMomentumEnabled && input != 0) {
        transform.SetParent(null);
        return true;
      }

      return false;
    }

    public bool TryDecelerate(float input, bool wallJumping, bool movingEnabled, bool touchingGround) {      
      if (Mathf.Abs(input) != 1 && !wallJumping || (!movingEnabled && touchingGround)) {
        physics.Velocity *= decelerationForce;
        return true;
      }

      return false;
    }

    /// <summary>
    /// Tries to perform either a single jump or wall jump based on how close the player is to ground or wall.
    /// </summary>
    public bool TryBufferedJump() {
      float distToFloor = player.DistanceToGround();
      float distToWall = player.DistanceToWall();

      if (distToFloor <= distToWall) {
        if (distToFloor < groundJumpBuffer) {
          ChangeToState<Jump1Start>();
          return true;
        }
      } else {
        if (distToWall < wallJumpBuffer && player.GetHorizontalInput() != 0) {
          ChangeToState<WallJump>();
          return true;
        }
      }

      return false;
    }


    public Facing GetFacing() {
      if (Mathf.Abs(physics.Vx) < idleThreshold) {
        return Facing.None;
      } else {
        return (Facing)Mathf.Sign(physics.Vx);
      }
    }

    /// <summary>
    /// Perform setup for a wall jump.
    /// </summary>
    public void WallJump() {
      isWallJumping = true;
    }

    public bool IsWallJumping() {
      return isWallJumping;
    }
    #endregion
  }
}