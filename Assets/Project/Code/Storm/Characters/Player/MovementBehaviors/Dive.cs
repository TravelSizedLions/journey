using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class Dive : MovementBehavior {

    private void Awake() {
      AnimParam = "dive";
    }


    public void OnAnimationFinished() {
      ChangeState<Crawling>();
    }

  }
}