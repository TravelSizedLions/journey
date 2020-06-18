using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {
  public class CarryJumpRise : CarryMotion {

    private bool releasedAction;

    private void Awake() {
      AnimParam = "carry_jump_rise";
    }


    public override void OnUpdate() {
      if (player.PressedAction() && releasedAction) {
        ChangeToState<MidAirThrowItem>();
      } else if (player.ReleasedAction()) {
        releasedAction = true;
      }
    }

    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (player.IsFalling()) {
        ChangeToState<CarryJumpFall>();
      }
    }

    public override void OnStateEnter() {
      releasedAction = !player.HoldingAction();
    }
  }

}