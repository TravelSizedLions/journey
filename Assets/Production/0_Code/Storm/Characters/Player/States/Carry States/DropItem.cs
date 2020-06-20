using System.Collections;
using System.Collections.Generic;
using Storm.Flexible;
using UnityEngine;

namespace Storm.Characters.Player {
  /// <summary>
  /// When the player is letting go of an item.
  /// </summary>
  public class DropItem : PlayerState {

    #region Unity API
    private void Awake() {
      AnimParam = "drop_item";
    }

    #endregion


    #region State API

    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      Carriable item = player.CarriedItem;
      item.OnPutdown();

      CarrySettings settings = GetComponent<CarrySettings>();
      Debug.Log(settings.DropForce);
      
      if (player.Facing == Facing.Right) {
        item.Physics.Vx = settings.DropForce.x;
      } else {
        item.Physics.Vx = -settings.DropForce.x;
      }

      item.Physics.Vy = settings.DropForce.y;
      
    }

    /// <summary>
    /// Fires when code outside the state machine is trying to send information.
    /// </summary>
    /// <param name="signal">The signal sent.</param>
    public override void OnSignal(GameObject obj) {
      Carriable carriable = obj.GetComponent<Carriable>();
      if (carriable != null) {
        ChangeToState<PickUpItem>();
      }
    }

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnDropItemFinished() {
      ChangeToState<Idle>();
    }
    #endregion
  }

}