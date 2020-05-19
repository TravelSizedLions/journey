using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player enters into a crouch.
  /// </summary>
  public class CrouchStart : PlayerState {


    #region Unity API
    private void Awake() {
      AnimParam = "crouch_start";
    }
    #endregion

    #region Player State API

    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      bool down = Input.GetButton("Down");
      if (!down) {
        ChangeToState<CrouchEnd>();
      } else if (down && Input.GetAxis("Horizontal") != 0) {
        ChangeToState<Crawling>();
      }
    }

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnCrouchStartFinished() {
      ChangeToState<Crouching>();
    }
    #endregion
  }
}