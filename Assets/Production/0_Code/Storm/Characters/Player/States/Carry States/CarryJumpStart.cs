using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is starting to jump while carrying an item.
  /// </summary>
  public class CarryJumpStart : CarryMotion {

    #region Fields
    private bool releasedAction;
    #endregion

    #region Unity API
    private void Awake() {
      AnimParam = "carry_jump_start";
    }
    #endregion

    #region State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (releasedAction && player.HoldingAction()) {
        ChangeToState<MidAirThrowItem>();
      }
    }

    /// <summary>
    /// Fires when the state exits, before the next state is entered into.
    /// </summary>
    public override void OnStateExit() {
      physics.Vy = settings.CarryJumpForce;
    }

    public override void OnStateEnter() {
      releasedAction = player.ReleasedAction() || !player.HoldingAction();
    }

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnCarryJumpStartFinished() {
      if (enabled) {
        ChangeToState<CarryJumpRise>();
      }
    }

    #endregion
  }

}