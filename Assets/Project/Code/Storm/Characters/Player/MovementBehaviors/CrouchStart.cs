using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public class CrouchStart : PlayerState {


    private void Awake() {
      AnimParam = "crouch_start";
    }

    public override void OnUpdate() {
      bool down = Input.GetButton("Down");
      if (!down) {
        ChangeToState<CrouchEnd>();
      } else if (down && Input.GetAxis("Horizontal") != 0) {
        ChangeToState<Crawling>();
      }
    }

    public void OnCrouchStartFinished() {
      ChangeToState<Crouching>();
    }
  }

}