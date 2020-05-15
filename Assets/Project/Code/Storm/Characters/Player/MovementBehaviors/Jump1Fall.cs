using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class Jump1Fall : HorizontalMotion {

    private void Awake() {
      AnimParam = "jump_1_fall";
    }

    public override void OnUpdate() {
      if (Input.GetButtonDown("Jump")) {
        ChangeToState<Jump2Start>();
      }
    }

    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (player.IsTouchingGround()) {
        ChangeToState<Land>();
      }
    }

  }
}