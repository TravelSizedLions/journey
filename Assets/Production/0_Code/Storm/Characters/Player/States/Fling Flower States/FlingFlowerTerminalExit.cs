
using Storm.LevelMechanics;
using UnityEngine;

namespace Storm.Characters.Player {
  public class FlingFlowerTerminalExit : PlayerState {

    private IFlingFlowerGuide guide; 

    private void Awake() {
      AnimParam = "fling_flower_terminal_exit";
    }

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
      float a = powersSettings.FlingFlowerGravitation;
      physics.Velocity = Vector3.zero;
      physics.Position = physics.Position*a + guide.CurrentFlower.transform.position*(1-a);
    }

    public void OnExitTerminalFinished() {
      if (!exited) {
        if (player.CarriedItem == null) {
          ChangeToState<Idle>();
        } else {
          ChangeToState<CarryIdle>();
        }
      }
    }

    public override void OnStateEnter() {
      guide = player.FlingFlowerGuide;
      guide.CurrentFlower.PickDirection(player);
      
      if (player.CarriedItem != null) {
        player.CarriedItem.Hide();
      }
    }

    public override void OnStateExit() {
      if (player.CarriedItem != null) {
        player.CarriedItem.Show();
      }
    }

  }
}