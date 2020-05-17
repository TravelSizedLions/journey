using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public class Crouching : PlayerState {

    private void Awake() {
      AnimParam = "crouching";
    }

    public override void OnUpdate() {
      if (!Input.GetButton("Down")) {
        ChangeToState<CrouchEnd>();
      } else if (Input.GetAxis("Horizontal") != 0) {
        ChangeToState<Crawling>();
      }
    }

  }

}