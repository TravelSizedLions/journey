using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public class CrouchStart : PlayerState {

    private void Awake() {
      AnimParam = "crouch_start";
    }

    public override void OnUpdate() {
      if (!Input.GetButton("Down")) {
        ChangeToState<CrouchEnd>();
      }
    }

    public void OnCrouchStartFinished() {
      ChangeToState<Crouching>();
    }
  }

}