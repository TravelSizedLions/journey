

using UnityEngine;

namespace Storm.Characters.Player {
  /// <summary>
  /// When the player this aiming themselves to launch from a fling flower.
  /// </summary>
  public class FlingFlowerAim : PlayerState {

    private IFlingFlowerGuide guide;

    private bool startedCharging = false;

    private void Awake() {
      AnimParam = "fling_flower_aim";
    }

    public override void OnUpdate() {
      guide.RotateGuide();

      if (player.PressedJump()) {
        // Let the player jump out of the flower.
        if (player.CarriedItem == null) {
          ChangeToState<SingleJumpStart>();
        } else {
          ChangeToState<CarryJumpStart>();
        }

      } else if (player.PressedAltAction()) {
        
        // Let the player fall from the flower.
        DropPlayer();
      } else if (player.PressedAction()) {

        // Begin charging for launch.
        startedCharging = true;

      } else if (player.ReleasedAction()) {

        // Send the player flying!
        if (guide.GetPercentCharged() > 0.25f) {
          physics.Velocity = player.GetMouseDirection()*guide.GetCharge();
          ChangeToState<FlingFlowerBallisticProjectile>();

        } else {
          // The player probably just clicked to release, so release them
          DropPlayer();
        }

      } else if (startedCharging) {

        // Do some charging.
        guide.Charge(powersSettings.AimableFlingFlowerChargeSpeed*Time.deltaTime);
      }
    }

    /// <summary>
    /// Release the player from the fling flower.
    /// </summary>
    private void DropPlayer() {
      if (player.CarriedItem == null) {
        ChangeToState<Idle>();
      } else {
        ChangeToState<CarryIdle>();
      }
    }

    public override void OnFixedUpdate() {
      float a = powersSettings.FlingFlowerGravitation;
      physics.Velocity = Vector3.zero;
      physics.Position = physics.Position*a + guide.CurrentFlower.transform.position*(1-a);
    }

    public override void OnStateEnter() {
      startedCharging = false;
      guide = player.FlingFlowerGuide;

      guide.SetMaxCharge(powersSettings.AimableFlingFlowerMaxVelocity);
      guide.Show();
      guide.ResetCharge();
      GameManager.Mouse.Swap("aim");

      if (player.CarriedItem != null) {
        player.CarriedItem.Hide();
      }
    }

    public override void OnStateExit() {
      guide.Hide();
      guide.ResetCharge();
      GameManager.Mouse.Swap("default");
      if (player.CarriedItem != null) {
        player.CarriedItem.Show();
        GameManager.Mouse.Swap("aim");
      }
    }
  }

}