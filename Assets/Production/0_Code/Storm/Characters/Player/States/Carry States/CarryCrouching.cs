using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public class CarryCrouching : PlayerState {

    private void Awake() {
      AnimParam = "carry_crouch";
    }

    public override void OnUpdate() {
      if (!player.HoldingDown()) {
        ChangeToState<CarryCrouchEnd>();
      }
    }
  }
}