using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class Land : PlayerState {

    private void Awake() {
      AnimParam = "land";
    }


    public override void OnUpdate() {
      if (Input.GetButton("Jump")) {
        ChangeToState<Jump1Start>();
      } else if (Input.GetAxis("Horizontal") != 0) {
        ChangeToState<Running>();
      } else if (Input.GetButton("Down")) {
        ChangeToState<CrouchStart>();
      }
    }

    public void OnLandFinished() {
      ChangeToState<Idle>();
    }
  }
}
