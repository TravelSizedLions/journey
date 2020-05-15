using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public class WallSlide : PlayerState {

    private void Awake() {
      AnimParam = "wall_slide";
    }

    public override void OnUpdate() {
      float input = Input.GetAxis("Horizontal");

      if (Input.GetButton("Jump")) {
      
      } else  {
        if (player.IsTouchingLeftWall() && input > 0) {
          ChangeToState<Jump1Fall>();
        } else if (player.IsTouchingRightWall() && input < 0) {
          ChangeToState<Jump1Fall>();
        }
      }
    }

    public override void OnFixedUpdate() {
      if (player.IsTouchingGround()) {
        ChangeToState<Idle>();
      }
    }
  }
}