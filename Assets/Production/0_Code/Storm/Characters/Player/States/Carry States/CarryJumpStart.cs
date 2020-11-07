using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is starting to jump while carrying an item.
  /// </summary>
  public class CarryJumpStart : CarryMotion {

    #region Unity API
    private void Awake() {
      AnimParam = "carry_jump_start";
    }
    #endregion

    #region State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {     
      if (player.CarriedItem == null) {
        ChangeToState<SingleJumpRise>();

      } else if ((player.HoldingAction() || player.HoldingAltAction()) && releasedAction) {
        ChangeToState<MidAirThrowItem>();
      } else if (player.ReleasedAction() || player.ReleasedAltAction()) {
        releasedAction = true;
      }
    }

    /// <summary>
    /// Fires when the state exits, before the next state is entered into.
    /// </summary>
    public override void OnStateExit() {
      physics.Vy = settings.CarryJumpForce;
    }

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnCarryJumpStartFinished() {
      if (!exited) {
        ChangeToState<CarryJumpRise>();
      }
    }


    #endregion
  }

}