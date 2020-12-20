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
    private string param = "drop_item";
    #endregion


    #region State API
    public override void OnUpdate() {
      if (player.ReleasedAction() || player.ReleasedAltAction()) {
        releasedAction = true;
      }
      
      if (player.CarriedItem == null) {
        ChangeToState<Idle>();
      } else if (player.HoldingDown()) {
        ChangeToState<Crouching>();
      } else if (releasedAction && (player.HoldingAction() || player.HoldingAltAction())) {
        player.Interact();
      }
    }

    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      player.Interact();
      
      if (player.HoldingAltAction()) {
        player.Drop(player.CarriedItem);
      } else if (player.HoldingAction()) {
        player.Throw(player.CarriedItem);
      } else {
        // Default action. :P Keep this separate from the other case, because
        // A.) it's easier to reason about written like this, and B.) this may
        // or may not change in the future. 
        player.Drop(player.CarriedItem);
      }
    }

    /// <summary>
    /// Fires when code outside the state machine is trying to send information.
    /// </summary>
    /// <param name="signal">The signal sent.</param>
    public override void OnSignal(GameObject obj) {
      if (CanCarry(obj)) {
        ChangeToState<PickUpItem>();
      } else if (IsDirectionalFlingFlower(obj)) {
        ChangeToState<FlingFlowerDirectedLaunch>();
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