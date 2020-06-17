using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public class CarryCrouchStart : PlayerState {

    private void Awake() {
      AnimParam = "carry_crouch_in";
    }

    public override void OnUpdate() {
      if (!player.HoldingDown()) {
        ChangeToState<CarryCrouchEnd>();
      }
    }

    public void OnCarryCrouchStartFinished() {
      ChangeToState<CarryCrouching>();
    }
  }

}