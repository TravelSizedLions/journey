using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player lands from a jump while carrying an item.
  /// </summary>
  public class CarryLand : CarryMotion {

    #region Unity API
    private void Awake() {
      AnimParam = "carry_land";
    }

    #endregion

    #region State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.CarriedItem == null) {
        ChangeToState<Land>();
        
      } else if (player.PressedJump()) {
        ChangeToState<CarryJumpStart>();

      } else if ((player.HoldingAction() || player.HoldingAltAction()) && releasedAction) {
        ChangeToState<DropItem>();
      } else if (player.ReleasedAction() || player.ReleasedAltAction()) {
        releasedAction = true;
      }
    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      if (player.TryingToMove()) {
        ChangeToState<CarryRun>();
      }
    }

    /// <summary>
    /// Animation event hook
    /// </summary>
    public void OnCarryLandFinished() {
      if (!exited) {
        ChangeToState<CarryIdle>();
      }
    }
    #endregion
  }

}