using System.Collections;
using System.Collections.Generic;
using Storm.Flexible;
using Storm.Flexible.Interaction;
using UnityEngine;

namespace Storm.Characters.Player {
  /// <summary>
  /// When the player is letting go of an item.
  /// </summary>
  public class DropItem : PlayerState {

    #region Fields
    private bool releasedAction;
    #endregion

    #region Unity API
    private void Awake() {
      AnimParam = "drop_item";
    }

    #endregion


    #region State API

    public override void OnUpdate() {
      if (player.ReleasedAction()) {
        releasedAction = true;
      }
      
      if (player.HoldingDown()) {
        ChangeToState<Crouching>();
      } else if (releasedAction && player.HoldingAction()) {
        player.Interact();
      }
    }

    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      releasedAction = player.ReleasedAction() || !player.HoldingAction();
      DropItem(player.CarriedItem);
    }

    /// <summary>
    /// Fires when code outside the state machine is trying to send information.
    /// </summary>
    /// <param name="signal">The signal sent.</param>
    public override void OnSignal(GameObject obj) {
      if (CanCarry(obj)) {
        ChangeToState<PickUpItem>();
      }
    }
    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnDropItemFinished() {
      if (!exited) {
        ChangeToState<Idle>();
      }
      
    }
    #endregion
  }

}