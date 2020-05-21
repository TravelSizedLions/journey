using System.Collections;
using System.Collections.Generic;
using Storm.Cameras;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player starts a jump from a wall.
  /// </summary>
  public class WallJump : HorizontalMotion {


    /// <summary>
    /// The force of a wall jump to the left.
    /// </summary>
    private Vector2 wallJumpLeft;

    /// <summary>
    /// The force of a wall jump to the right.
    /// </summary>
    private Vector2 wallJumpRight;

    #region Unity API
    private void Awake() {
      AnimParam = "wall_jump";
    }
    #endregion

    #region  Player State API
    /// <summary>
    /// First time initialization for the state. A reference to the player and the player's rigidbody will already have been added by this point.
    /// </summary>
    public override void OnStateAdded() {
      base.OnStateAdded();
      
      MovementSettings settings = GetComponent<MovementSettings>();

      wallJumpRight = settings.WallJump;
      wallJumpLeft = new Vector2(-wallJumpRight.x, wallJumpRight.y);
    }

    /// <summary>
    /// Fires when the state exits, before the next state is entered into.
    /// </summary>
    public override void OnStateExit() {
      WallJump();
      if (player.IsTouchingLeftWall()) {
        rigidbody.velocity = wallJumpRight;
      } else {
        rigidbody.velocity = wallJumpLeft;
      }

      TargettingCamera cam = FindObjectOfType<TargettingCamera>();
      cam.ResetTracking(false, true);
    }

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnWallJumpFinished() {
      ChangeToState<Jump1Rise>();
    }
    #endregion
  }
}