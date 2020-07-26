using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is standing still while carrying an item.
  /// </summary>
  public class CarryIdle : PlayerState {
    
    #region Fields
    /// <summary>
    /// Whether or not the player has released the action button.
    /// </summary>
    private bool actionReleased;
    #endregion

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
      if (player.TryingToMove()) {
        ChangeToState<CarryRun>();
      } else if (player.PressedAction() && actionReleased) {
        ChangeToState<DropItem>();
      } else if (player.ReleasedAction()) {
        actionReleased = true;
      } else if (player.PressedJump()) {
        ChangeToState<CarryJumpStart>();
      } else if (player.PressedDown()) {
        ChangeToState<CarryCrouchStart>();
      }
    }

    public override void OnFixedUpdate() {
      if (!player.IsTouchingGround()) {
        ChangeToState<CarryJumpFall>();
      }
    }

    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      actionReleased = !player.HoldingAction();
    }
    #endregion
  }

}