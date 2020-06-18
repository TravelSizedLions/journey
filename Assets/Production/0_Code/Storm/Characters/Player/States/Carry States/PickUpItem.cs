using System.Collections;
using System.Collections.Generic;
using Storm.Flexible;
using UnityEngine;

namespace Storm.Characters.Player {

  public class PickUpItem : CarryMotion {


    private bool canDrop;

    private void Awake() {
      AnimParam = "pick_up_item";
    }

    public override void OnUpdate() {
      if (player.PressedJump()) {
        ChangeToState<CarryJumpStart>();
      } else if (player.ReleasedAction()) {
        canDrop = true;
      } else if (player.PressedAction() && canDrop) {
        ChangeToState<DropItem>();
      } else if (player.TryingToMove()) {
        Debug.Log("RUNNING!");
        ChangeToState<CarryRun>();
      } 
    }

    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);


    }

    public void OnPickupItemFinished() {
      if (player.TryingToMove()) {
        ChangeToState<CarryRun>();
      } else {
        ChangeToState<CarryIdle>();
      }
    }

    public override void OnStateEnter() {
      Carriable carriable = player.CarriedItem;
      carriable.OnPickup();
      canDrop = !player.HoldingAction();
    }
  }

}