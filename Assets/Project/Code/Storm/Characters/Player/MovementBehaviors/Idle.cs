using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {
  public class Idle : PlayerState {

    private void Awake() {
      AnimParam = "idle";
    }


    #region Movement Behavior Implementation
    //-------------------------------------------------------------------------
    // Movement Behavior Implementation
    //-------------------------------------------------------------------------


    public override void OnUpdate() {
      if (Input.GetButton("Jump")) {
        ChangeToState<Jump1Start>();
      } else if (Input.GetButton("Down")) {
        ChangeToState<CrouchStart>();
      } else if (Input.GetAxis("Horizontal") != 0) {
        ChangeToState<Running>();
      }
    }

    #endregion
  }

}