using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public class CarryLand : CarryMotion {

    private void Awake() {
      AnimParam = "carry_land";
    }

    public override void OnUpdate() {
      if (player.PressedJump()) {
        ChangeToState<CarryJumpStart>();
      } else if (player.HoldingAction()) {
        ChangeToState<DropItem>();
      }
    }

    public override void OnFixedUpdate() {
      if (player.TryingToMove()) {
        ChangeToState<CarryRun>();
      }
    }

    public void OnCarryLandFinished() {
      ChangeToState<CarryIdle>();
    }
  }

}