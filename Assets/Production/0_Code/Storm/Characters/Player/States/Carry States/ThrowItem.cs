using System.Collections;
using System.Collections.Generic;
using Storm.Flexible;
using Storm.Flexible.Interaction;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player tosses an item (while on the ground).
  /// </summary>
  public class ThrowItem : HorizontalMotion {

    #region Fields
    /// <summary>
    /// The force that the item will be thrown with.
    /// </summary>
    private Vector2 throwForce;

    /// <summary>
    /// The vertical force applied to a throw when the player is holding up.
    /// </summary>
    private float verticalThrowForce;
    #endregion

    #region Unity API
    private void Awake() {
      AnimParam = "carry_run_throw";
    }
    #endregion

    #region State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.PressedJump()) {
        if (player.IsTouchingLeftWall() || player.IsTouchingRightWall()) {
          ChangeToState<WallRun>();
        } else {
          ChangeToState<SingleJumpStart>();
        }
      } else if (player.HoldingDown()) {
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
      } else if (!player.IsTouchingGround() && player.IsFalling()) {
        player.StartCoyoteTime();
        ChangeToState<SingleJumpFall>();
      }
    }

    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      if (player.HoldingAltAction()) {
        player.Drop(player.CarriedItem);
      } else if (player.HoldingAction()) {
        player.Throw(player.CarriedItem);
      }
    }

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnThrowItemFinished() {
      if (!exited) {
        ChangeToState<Running>();
      }
    }
    #endregion
  }
}