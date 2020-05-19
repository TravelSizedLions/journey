using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  /// <summary>
  /// When the player lands from a light fall.
  /// </summary>
  public class Land : PlayerState {

    #region Unity API
    private void Awake() {
      AnimParam = "land";
    }
    #endregion

    #region Player State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (Input.GetButton("Jump")) {
        ChangeToState<Jump1Start>();
      } else if (Input.GetAxis("Horizontal") != 0) {
        ChangeToState<Running>();
      } else if (Input.GetButton("Down")) {
        ChangeToState<CrouchStart>();
      }
    }

    /// <summary>
    /// Animation pre-hook.
    /// </summary>
    public void OnLandFinished() {
      ChangeToState<Idle>();
    }
    #endregion
  }
}
