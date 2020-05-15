using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public class CrouchEnd : PlayerState {

    private void Awake() {
      AnimParam = "crouch_end";
    }


    public void OnCrouchEndFinished() {
      ChangeToState<Idle>();
    }


    public override void OnUpdate() {
      if (Input.GetAxis("Horizontal") != 0) {
        ChangeToState<Running>();
      } else if (Input.GetButton("Down")) {
        ChangeToState<CrouchStart>();
      }
    }
  }
}