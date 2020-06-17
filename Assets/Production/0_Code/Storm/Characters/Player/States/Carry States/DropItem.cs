using System.Collections;
using System.Collections.Generic;
using Storm.Flexible;
using UnityEngine;

namespace Storm.Characters.Player {
  public class DropItem : PlayerState {

    private void Awake() {
      AnimParam = "drop_item";
    }

    public void OnDropItemFinished() {
      ChangeToState<Idle>();
    }

    public override void OnStateEnter() {
      Carriable item = player.CarriedItem;
      item.OnPutdown();

      CarrySettings settings = GetComponent<CarrySettings>();
      if (player.Facing == Facing.Right) {
        item.Physics.Vx = settings.DropForce.x;
      } else {
        item.Physics.Vx = -settings.DropForce.x;
      }

      item.Physics.Vy = settings.DropForce.y;
      
    }

    public override void OnSignal(GameObject obj) {
      Carriable carriable = obj.GetComponent<Carriable>();
      if (carriable != null) {
        ChangeToState<PickUpItem>();
      }
    }
  }

}