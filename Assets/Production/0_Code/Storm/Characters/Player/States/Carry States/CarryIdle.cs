using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public class CarryIdle : PlayerState {
    
    private void Awake() {
      AnimParam = "carry_idle";
    }


    public override void OnUpdate() {
      if (player.TryingToMove()) {
        ChangeToState<CarryRun>();
      } else if (player.PressedAction()) {
        ChangeToState<DropItem>();
      } else if (player.PressedJump()) {
        ChangeToState<CarryJumpStart>();
      } else if (player.PressedDown()) {
        ChangeToState<CarryCrouchStart>();
      }
    }
  }

}