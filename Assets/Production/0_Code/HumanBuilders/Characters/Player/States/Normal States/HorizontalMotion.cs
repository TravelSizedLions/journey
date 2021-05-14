using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// Shared behavior for player states that allow the player to move left/right.
  /// </summary>
  public abstract class HorizontalMotion : MotionState {

    #region Fields
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
      if (GameManager.Player.IsTouchingGround()) {
        GameManager.Player.StopWallJumpMuting();
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
      groundJumpBuffer = settings.GroundJumpBuffer;
      wallJumpBuffer = settings.WallJumpBuffer;
    }
    #endregion

    #region Motion State API

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
  }
}