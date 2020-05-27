using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player runs left/right.
  /// </summary>
  public class Running : HorizontalMotion {

    #region Unity API
    private void Awake() {
      AnimParam = "running";
    }
    #endregion


    #region Player State API

    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (Input.GetButtonDown("Jump")) {
        if (player.IsTouchingLeftWall() || player.IsTouchingRightWall()) {
          ChangeToState<WallRun>();
        } else if (player.CanJump()) {
          ChangeToState<Jump1Start>();
        }
      } else if (Input.GetButton("Down")) {
        ChangeToState<Dive>();
      }
    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (facing == Facing.None) {
        ChangeToState<Idle>();
      } else if (!player.IsTouchingGround() && rigidbody.velocity.y < 0) {
        ChangeToState<Jump1Fall>();
      }
    }
    #endregion
  }
}