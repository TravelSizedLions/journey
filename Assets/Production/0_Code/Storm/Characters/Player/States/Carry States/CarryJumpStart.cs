using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public class CarryJumpStart : CarryMotion {


    private void Awake() {
      AnimParam = "carry_jump_start";
    }

    public override void OnUpdate() {
      if (player.PressedAction()) {
        ChangeToState<MidAirThrowItem>();
      }
    }

    public void OnCarryJumpStartFinished() {
      ChangeToState<CarryJumpRise>();
    }

    public override void OnStateExit() {
      CarrySettings settings = GetComponent<CarrySettings>();
      physics.Vy = settings.JumpForce;
    }
  }

}