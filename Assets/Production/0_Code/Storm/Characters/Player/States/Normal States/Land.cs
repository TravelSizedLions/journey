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
      if (player.HoldingDown()) {
        ChangeToState<CrouchStart>();
      } else if (player.PressedJump()) {
        ChangeToState<SingleJumpStart>();
      } else if (player.TryingToMove()) {
        ChangeToState<Running>();
      } else if (player.PressedAction()) {
        player.Interact();
      }
    }

    public override void OnSignal(GameObject obj) {
      if (CanCarry(obj)) {
        ChangeToState<PickUpItem>();
      }
    }

    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      physics.Vy = 0;
    }

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnLandFinished() {
      ChangeToState<Idle>();
    }
    #endregion
  }
}
