using System.Collections;
using System.Collections.Generic;
using Storm.Flexible;
using UnityEngine;

namespace Storm.Characters.Player {

  public class PickUpItem : PlayerState {


    private bool canDrop;

    private void Awake() {
      AnimParam = "pick_up_item";
    }

    public override void OnUpdate() {
      if (player.PressedJump()) {
        ChangeToState<CarryJumpStart>();
      } if (player.ReleasedAction()) {
        canDrop = true;
      } if (player.PressedAction() && canDrop) {
        ChangeToState<DropItem>();
      }
    }

    public void OnPickupItemFinished() {
      ChangeToState<CarryIdle>();
    }

    public override void OnStateEnter() {
      Carriable carriable = player.CarriedItem;
      carriable.OnPickup();
      canDrop = false;
    }
  }

}