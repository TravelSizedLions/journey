using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {
  /// <summary>
  /// When the player stands up from crouch while holding an item.
  /// </summary>
  public class CarryCrouchEnd : PlayerState {

    #region Unity API
    private void Awake() {
      AnimParam = "carry_crouch_out";
    }

    #endregion

    #region State API

    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.HoldingDown()) {
        ChangeToState<CarryCrouchStart>();
      } else if (player.TryingToMove()) {
        ChangeToState<CarryRun>();
      }
    }

    /// <summary>
    /// Animation event hook
    /// </summary>
    public void OnCarryCrouchEndFinished() {
      ChangeToState<CarryIdle>();
    }
    #endregion
  }

}