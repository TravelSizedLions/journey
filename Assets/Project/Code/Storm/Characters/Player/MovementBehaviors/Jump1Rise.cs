using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is rising during their first jump.
  /// </summary>
  public class Jump1Rise : HorizontalMotion {

    #region Unity API
    private void Awake() {
      AnimParam = "jump_1_rise";
    }
    #endregion

    #region Player State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.TryingToJump()) {
        if (!base.TryBufferedJump()) {
          ChangeToState<Jump2Start>();
        }
      } 
    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();

      player.SetFacing(facing);
      
      if (player.IsFalling()) {
        ChangeToState<Jump1Fall>();
      } else {
        bool leftWall = player.IsTouchingLeftWall();
        bool rightWall =player.IsTouchingRightWall();
        if ((leftWall || rightWall) && !IsWallJumping()) {
          ChangeToState<WallRun>();
        }
      }
    }

    #endregion
  }
}
