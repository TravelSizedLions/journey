using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {
  public class Crouching : MovementBehavior {

    private void Awake() {
      AnimParam = "crouching";
    }


    public override void HandleInput() {
      if (!Input.GetButton("Down")) {
        ChangeState<EndCrouch>();
      }
    }

    public override void HandlePhysics() {
      float input = Input.GetAxis("Horizontal");

      if (input != 0 && Input.GetButton("Down")) {
        ChangeState<Crawling>();
      }
    }
  }

}