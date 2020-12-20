using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  /// <summary>
  /// When the player is crouching while holding an item.
  /// </summary>
  public class CarryCrouching : PlayerState {

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
    private string param = "carry_crouch";
    #endregion


    #region State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      bool holdingDown = player.HoldingDown();

      if (player.CarriedItem == null) {
        ChangeToState<Crouching>();
        
      } else if ((player.HoldingAction() || player.HoldingAltAction()) && releasedAction) {
        if (!holdingDown) {
          ChangeToState<DropItem>();  
        } else {
          ChangeToState<Crouching>();
        }
        
      } else if (!player.HoldingDown()) {
        ChangeToState<CarryCrouchEnd>();

      } else if (!player.ReleasedAction() || player.ReleasedAltAction()) {
        releasedAction = true;
      }
    }

    public override void OnStateExit() {
      if (player.HoldingAltAction()) {
        player.Drop(player.CarriedItem);
      } else if (player.HoldingAction()) {
        player.Throw(player.CarriedItem);
      }
    }

    public override void OnSignal(GameObject obj) {
      if (IsAimableFlingFlower(obj)) {
        ChangeToState<FlingFlowerAim>();
      } else if (IsDirectionalFlingFlower(obj)) {
        ChangeToState<FlingFlowerDirectedLaunch>();
      }
    }
    #endregion
  }
}