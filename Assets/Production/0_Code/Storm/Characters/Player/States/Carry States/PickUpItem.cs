using System.Collections;
using System.Collections.Generic;
using Storm.Flexible;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is picking up an item.
  /// </summary>
  public class PickUpItem : CarryMotion {

    #region Fields
    /// <summary>
    /// Whether or not the player can drop the item in this state.
    /// </summary>
    private bool canDrop;
    #endregion

    #region Unity API
    private void Awake() {
      AnimParam = "pick_up_item";
    }
    #endregion

    #region State API

    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.PressedJump()) {
        ChangeToState<CarryJumpStart>();
      } else if (player.PressedAction() && canDrop) {
        ChangeToState<DropItem>();
      } else if (player.TryingToMove()) {
        ChangeToState<CarryRun>();
      } else if (player.HoldingDown()) {
        ChangeToState<CarryCrouching>();
      } 

      if (player.ReleasedAction()) {
        canDrop = true;
      }
    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);
    }

    // /// <summary>
    // ///  Fires whenever the state is entered into, after the previous state exits.
    // /// </summary>
    // public override void OnStateEnter() {
    //   Carriable carriable = player.CarriedItem;
    //   carriable.OnPickup();
    //   canDrop = !player.HoldingAction();
    // }

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnPickupItemFinished() {
      if (enabled) {
        if (player.TryingToMove()) {
          ChangeToState<CarryRun>();
        } else {
          ChangeToState<CarryIdle>();
        }
      }
    }
    #endregion
  }
}