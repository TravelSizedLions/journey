using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class EndCrouch : MovementBehavior {


    public void OnAnimationFinished() {
      ChangeState<Idle>();
    }


    private void Awake() {
      AnimParam = "crouch_out";
    }

    public override void HandleInput() {
      if (Input.GetButtonDown("Down")) {
        ChangeState<StartCrouch>();
      } else if (Input.GetButton("Jump")) {
        ChangeState<SingleJump>();
      } else if (Input.GetAxis("Horizontal") != 0) {
        ChangeState<Running>();
      }
    }

  }

}