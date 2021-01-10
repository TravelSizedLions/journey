using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// When the player is standing still while carrying an item.
  /// </summary>
  public class CarryIdle : PlayerState {
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
    private string param = "carry_idle";
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