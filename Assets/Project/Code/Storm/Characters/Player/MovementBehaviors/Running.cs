using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {


  public class Running : HorizontalMotion {


    private void Awake() {
      AnimParam = "running";
    }

    public override void OnUpdate() {
      if (Input.GetButtonDown("Jump")) {
        if (player.IsTouchingLeftWall() || player.IsTouchingRightWall()) {
          ChangeToState<WallRun>();
        } else {
          ChangeToState<Jump1Start>();
        }
      } else if (Input.GetButton("Down")) {
        ChangeToState<Dive>();
      }
    }

    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (facing == Facing.None) {
        ChangeToState<Idle>();
      } else if (!player.IsTouchingGround()) {
        ChangeToState<Jump1Fall>();
      }
    }

  }
}