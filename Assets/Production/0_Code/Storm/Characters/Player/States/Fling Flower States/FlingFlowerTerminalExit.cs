
using Storm.LevelMechanics;
using UnityEngine;

namespace Storm.Characters.Player {
  public class FlingFlowerTerminalExit : PlayerState {

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
    private string param = "fling_flower_terminal_exit";

    /// <summary>
    /// The GUI guide for this fling flower.
    /// </summary>
    private IFlingFlowerGuide guide; 
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