using Storm.LevelMechanics;
using UnityEngine;

namespace Storm.Characters.Player {
  public class FlingFlowerDirectedProjectile : PlayerState {

    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    public override string AnimParam { get { return param; } }
    #endregion

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    private string param = "fling_flower_directed_projectile";
    #endregion

    public override void OnUpdate() {
      if (player.PressedJump()) {
        if (player.CarriedItem == null) {
          ChangeToState<DoubleJumpStart>();
        } else {
          ChangeToState<CarryJumpStart>();
        }
      }
    }

    public override void OnFixedUpdate() {
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

    public override void OnStateEnter() {
      physics.GravityScale = 0;

      if (player.CarriedItem != null) {
        player.CarriedItem.Hide();
      }
    }

    public override void OnStateExit() {
      physics.GravityScale = 1;

      if (player.CarriedItem != null) {
        player.CarriedItem.Show();
      }
    }

    public override void OnSignal(GameObject obj) {
      if (IsAimableFlingFlower(obj)) {
        ChangeToState<FlingFlowerAim>();

      } else if (IsDirectionalFlingFlower(obj)) {
        DirectionalFlingFlower flower = obj.GetComponent<DirectionalFlingFlower>();

        if (flower.IsTerminal()) {
          ChangeToState<FlingFlowerTerminalExit>();
        } else {
          ChangeToState<FlingFlowerDirectedLaunch>();
        }      
      }
    }

  }
}