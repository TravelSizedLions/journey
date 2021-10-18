using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// When the player runs left/right.
  /// </summary>
  public class Running : HorizontalMotion {
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    public override string AnimParam { get { return param; } }

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    private string param = "running";

    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.PressedJump()) {
        if (player.DistanceToWall() < settings.WallRunHorzBuffer) {
          ChangeToState<WallRun>();
        } else {
          ChangeToState<SingleJumpStart>();
        }
      } else if (player.CanDiveLeft() || player.CanDiveRight()) {
        ChangeToState<Dive>();
      } else if (player.PressedAction() || player.PressedAltAction()) {
        player.Interact();
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
    /// Fires when code outside the state machine is trying to send information.
    /// </summary>
    /// <param name="signal">The signal sent.</param>
    public override void OnSignal(GameObject obj) {
      if (CanCarry(obj)) {
        ChangeToState<PickUpItem>();
      } else if (IsAimableFlingFlower(obj)) {
        ChangeToState<FlingFlowerAim>();
      } else if (IsDirectionalFlingFlower(obj)) {
        ChangeToState<FlingFlowerDirectedLaunch>();
      }
    }
  }
}