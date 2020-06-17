using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public class ThrowItem : PlayerState {

    private void Awake() {
      AnimParam = "carry_run_throw";
    }
  }

}