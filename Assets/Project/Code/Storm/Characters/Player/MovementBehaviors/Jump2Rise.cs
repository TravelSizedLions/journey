using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {

  /// <summary>
  /// WHen the player is rising from their double jump.
  /// </summary>
  public class Jump2Rise : HorizontalMotion {

    #region Unity API
    private void Awake() {
      AnimParam = "jump_2_rise";
    }
    #endregion

    #region Player State API

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (player.IsTouchingRightWall() || player.IsTouchingLeftWall()) {
        ChangeToState<WallRun>();
      } else if (player.IsFalling()) {
        ChangeToState<Jump2Fall>();
      } else if (player.TryingToJump()) {
        base.TryBufferedJump();
      }
    }
    #endregion
  }

}