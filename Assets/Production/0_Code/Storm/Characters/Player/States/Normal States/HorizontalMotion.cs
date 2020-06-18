using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// Shared behavior for player states that allow the player to move left/right.
  /// </summary>
  public class HorizontalMotion : MotionState {

    #region Fields
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
      base.OnStateAdded();

      MovementSettings settings = GetComponent<MovementSettings>();
      wallJumpMuting = settings.WallJumpMuting;
      groundJumpBuffer = settings.GroundJumpBuffer;
      wallJumpBuffer = settings.WallJumpBuffer;
    }
    #endregion

    #region Motion State API
    /// <summary>
    /// Translate user input into horizontal motion.
    /// </summary>
    /// <returns>Which direction the player should be facing.</returns>
    public override Facing MoveHorizontally() {
      float input = player.GetHorizontalInput();
      bool movingEnabled = player.CanMove();

      TryDecelerate(input, isWallJumping, movingEnabled, player.IsTouchingGround());

      if (!movingEnabled) {
        return GetFacing();
      }

      TryUnparentPlayerTransform(player.IsPlatformMomentumEnabled(), input);

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

    /// <summary>
    /// Tries to perform either a single jump or wall jump based on how close the player is to ground or wall.
    /// </summary>
    public override bool TryBufferedJump() {
      float distToFloor = player.DistanceToGround();
      float distToWall = player.DistanceToWall();

      if (distToFloor <= distToWall) {
        if (distToFloor < groundJumpBuffer) {
          ChangeToState<SingleJumpStart>();
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
    #endregion

    #region Other Public Methods
    /// <summary>
    /// Perform setup for a wall jump.
    /// </summary>
    public void WallJump() {
      isWallJumping = true;
    }

    /// <summary>
    /// Whether or not the player is currently in the air from a wall jump.
    /// </summary>
    /// <returns>True if the player is wall jumping, false otherwise.</returns>
    public bool IsWallJumping() {
      return isWallJumping;
    }
    #endregion
  }
}