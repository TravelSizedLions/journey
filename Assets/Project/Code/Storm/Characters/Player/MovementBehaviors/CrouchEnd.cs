using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player stands up from crouch.
  /// </summary>
  public class CrouchEnd : PlayerState {

    #region Unity API
    private void Awake() {
      AnimParam = "crouch_end";
    }

    #endregion

    #region Player State API

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnCrouchEndFinished() {
      ChangeToState<Idle>();
    }

    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (Input.GetAxis("Horizontal") != 0) {
        ChangeToState<Running>();
      } else if (Input.GetButton("Down")) {
        ChangeToState<CrouchStart>();
      } else if (Input.GetButton("Jump") && player.CanJump()) {
        ChangeToState<Jump1Start>();
      }
    }
    #endregion

  }
}