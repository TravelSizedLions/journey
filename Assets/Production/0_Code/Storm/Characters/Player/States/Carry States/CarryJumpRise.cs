using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {
  public class CarryJumpRise : CarryMotion {

    private void Awake() {
      AnimParam = "carry_jump_rise";
    }


    public override void OnUpdate() {
      if (player.PressedAction()) {
        ChangeToState<MidAirThrowItem>();
      }
    }

    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (player.IsFalling()) {
        ChangeToState<CarryJumpFall>();
      }
    }
  }

}