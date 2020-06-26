using System.Collections;
using System.Collections.Generic;
using Storm.Flexible;
using Storm.Flexible.Interaction;
using UnityEngine;


namespace Storm.Characters.Player {

  /// <summary>
  /// WHen the player is rising from their double jump.
  /// </summary>
  public class DoubleJumpRise : HorizontalMotion {

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
        ChangeToState<WallSlide>();
      } else if (player.IsFalling()) {
        ChangeToState<DoubleJumpFall>();
      } else if (player.PressedJump()) {
        base.TryBufferedJump();
      } else if (player.PressedAction()) {
        player.Interact();
      }
    }

    /// <summary>
    /// Fires when code outside the state machine is trying to send information.
    /// </summary>
    /// <param name="signal">The signal sent.</param>
    public override void OnSignal(GameObject obj) {
      if (CanCarry(obj)) {
        ChangeToState<CarryJumpRise>();
      }
    }
    #endregion
  }

}