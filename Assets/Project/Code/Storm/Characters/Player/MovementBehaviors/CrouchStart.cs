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
      if (!player.HoldingDown()) {
        ChangeToState<CrouchEnd>();
      } else if (player.TryingToMove()) {
        ChangeToState<Crawling>();
      } else if (!player.IsTouchingGround()) {
        ChangeToState<SingleJumpFall>();
      }
    }


    public override void OnStateEnter() {
      physics.Velocity = Vector2.zero;
    }

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnCrouchStartFinished() {
      if (enabled) {
        ChangeToState<Crouching>();
      }
    }

    #endregion
  }
}