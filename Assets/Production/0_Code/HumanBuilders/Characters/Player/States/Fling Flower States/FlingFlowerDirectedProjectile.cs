
using UnityEngine;

namespace HumanBuilders {
  public class FlingFlowerDirectedProjectile : PlayerState {

    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    public override string AnimParam { get { return "fling_flower_directed_projectile"; } }

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

      player.gameObject.layer = LayerMask.NameToLayer(Layers.FLING_FLOWER);

      if (player.CarriedItem != null) {
        player.CarriedItem.Hide();
      }
    }

    public override void OnStateExit() {
      physics.GravityScale = 1;

      if (player.CarriedItem != null) {
        player.CarriedItem.Show();
      }

      player.gameObject.layer = LayerMask.NameToLayer(Layers.PLAYER);
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