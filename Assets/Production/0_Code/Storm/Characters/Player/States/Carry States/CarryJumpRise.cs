using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {
  /// <summary>
  /// When the player is rising from a jump while carrying an item.
  /// </summary>
  public class CarryJumpRise : CarryMotion {

    #region Unity API
    private void Awake() {
      AnimParam = "carry_jump_rise";
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
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (player.IsFalling()) {
        ChangeToState<CarryJumpFall>();
      }
    }


    #endregion
  }

}