
using UnityEngine;

namespace HumanBuilders {

  public class FlingFlowerBallisticProjectile : PlayerState {
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    public override string AnimParam { get { return "fling_flower_ballistic_projectile"; } }

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The player's horizontal axis input.
    /// </summary>
    private float horizontalInput;

    /// <summary>
    /// The player's vertical axis input.
    /// </summary>
    private float verticalInput;

    /// <summary>
    /// Whether or not the player has tried to use mid-air control.
    /// </summary>
    private bool usedHorizontalAirControl;

    public override void OnUpdate() {
      horizontalInput = player.GetHorizontalInput();
      verticalInput = player.GetVerticalInput();

      if (player.PressedJump()) {

        // Make sure the player is facing the right way on exit.
        player.SetFacing(physics.Vx > 0 ? Facing.Right : Facing.Left);

        if (player.CarriedItem == null) {
          ChangeToState<DoubleJumpStart>();
        } else {
          ChangeToState<CarryJumpStart>();
        }
      }
    }

    public override void OnFixedUpdate() {
      if (horizontalInput != 0 || verticalInput != 0) {
        if (horizontalInput != 0) {
          usedHorizontalAirControl = true;
        }

        Vector2 nudge = new Vector2(
          horizontalInput*powersSettings.FlingFlowerAirControl.x,
          verticalInput*powersSettings.FlingFlowerAirControl.y
        );

        physics.Velocity += nudge;
      } else if (usedHorizontalAirControl) {
        physics.Vx *= powersSettings.FlingFlowerAirControlDeceleration;
      }

      // Don't fall faster than the maximum fall velocity.
      physics.Vy = Mathf.Max(-settings.MaxFallSpeed, physics.Vy);

      bool ground = player.IsTouchingGround();
      bool left = player.IsTouchingLeftWall();
      bool right = player.IsTouchingRightWall();
      bool ceil = player.IsTouchingCeiling();

      if (ground || left || right || ceil) {
        physics.Vy = powersSettings.FlingFlowerLandingHopForce;
        if (player.CarriedItem == null) {
          ChangeToState<SingleJumpRise>();
        } else {
          ChangeToState<CarryJumpRise>();
        }
      }
    }

    public override void OnSignal(GameObject obj) {
      if (IsAimableFlingFlower(obj)) {
        ChangeToState<FlingFlowerAim>();
      } else if (IsDirectionalFlingFlower(obj)) {
        ChangeToState<FlingFlowerDirectedLaunch>();
      }
    }

    public override void OnStateEnter() {
      // player.gameObject.layer = LayerMask.NameToLayer(Layers.FLING_FLOWER);
      if (player.CarriedItem != null) {
        player.CarriedItem.Hide();
      }
      usedHorizontalAirControl = false;
    }

    public override void OnStateExit() {
      // player.gameObject.layer = LayerMask.NameToLayer(Layers.PLAYER);
      if (player.CarriedItem != null) {
        player.CarriedItem.Show();
        GameManager.Mouse.Swap("aim");
      }
    }
  }
}