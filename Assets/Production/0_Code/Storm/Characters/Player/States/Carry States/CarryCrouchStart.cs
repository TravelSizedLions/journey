using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is starting to crouch while carrying an item.
  /// </summary>
  public class CarryCrouchStart : PlayerState {

    #region Fields
    private bool releasedAction;
    #endregion

    #region Unity API
    private void Awake() {
      AnimParam = "carry_crouch_in";
    }
    #endregion

    #region State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (!player.HoldingDown()) {
        ChangeToState<CarryCrouchEnd>();
      } else if (releasedAction && player.HoldingAction()) {
        ChangeToState<DropItem>();
      }

      if (player.ReleasedAction()) {
        releasedAction = true;
      }
    }


    public override void OnStateEnter() {
      releasedAction = player.ReleasedAction() || !player.HoldingAction();
    }

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnCarryCrouchStartFinished() {
      if (!exited) {
        ChangeToState<CarryCrouching>();
      }
    }

    #endregion
  }

}