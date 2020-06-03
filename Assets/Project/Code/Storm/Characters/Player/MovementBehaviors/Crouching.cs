using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is crouching.
  /// </summary>
  public class Crouching : PlayerState {

    #region Unity API
    private void Awake() {
      AnimParam = "crouching";
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
      }
    }

    public override void OnStateEnter() {
      physics.Velocity = Vector2.zero;
    }
    #endregion
  }
}