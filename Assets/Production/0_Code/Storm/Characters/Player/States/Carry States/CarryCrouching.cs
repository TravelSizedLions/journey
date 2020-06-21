using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  /// <summary>
  /// When the player is crouching while holding an item.
  /// </summary>
  public class CarryCrouching : PlayerState {

    #region Fields
    private bool releasedAction;
    #endregion

    #region Unity API
    private void Awake() {
      AnimParam = "carry_crouch";
    }
    #endregion

    #region State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      bool holdingDown = player.HoldingDown();

      if (player.HoldingAction() && releasedAction) {
        if (!holdingDown) {
          ChangeToState<DropItem>();  
        } else {
          DropItem(player.CarriedItem);
          ChangeToState<Crouching>();
        }
        
      } else if (!player.HoldingDown()) {
        ChangeToState<CarryCrouchEnd>();
      }

      if (player.ReleasedAction()) {
        releasedAction = true;
      }
    }

    public override void OnStateEnter() {
      releasedAction = player.ReleasedAction() || !player.HoldingAction();
    }
    #endregion
  }
}