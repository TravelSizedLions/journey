using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {
  public class CarryCrouchEnd : PlayerState {
    private void Awake() {
      AnimParam = "carry_crouch_out";
    }

    public override void OnUpdate() {
      if (player.HoldingDown()) {
        ChangeToState<CarryCrouchStart>();
      } else if (player.TryingToMove()) {
        ChangeToState<CarryRun>();
      }
    }

    public void OnCarryCrouchEndFinished() {
      ChangeToState<CarryIdle>();
    }
  }

}