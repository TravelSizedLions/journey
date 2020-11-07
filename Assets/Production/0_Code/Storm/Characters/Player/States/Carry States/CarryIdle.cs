using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is standing still while carrying an item.
  /// </summary>
  public class CarryIdle : PlayerState {

    #region Unity API
    private void Awake() {
      AnimParam = "carry_idle";
    }
    #endregion

    #region State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.CarriedItem == null) {
        ChangeToState<Idle>();
        
      } else if (player.TryingToMove()) {
        ChangeToState<CarryRun>();

      } else if ((player.HoldingAction() || player.HoldingAltAction()) && releasedAction) {
        ChangeToState<DropItem>();

      } else if (player.PressedJump()) {
        ChangeToState<CarryJumpStart>();

      } else if (player.PressedDown()) {
        ChangeToState<CarryCrouchStart>();

      } else if (player.ReleasedAction() || player.ReleasedAltAction()) {
        releasedAction = true;
      }
    }

    public override void OnFixedUpdate() {
      if (!player.IsTouchingGround()) {
        ChangeToState<CarryJumpFall>();
      }
    }

    #endregion
  }

}