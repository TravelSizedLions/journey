using System.Collections;
using System.Collections.Generic;


using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// When the player is crouching.
  /// </summary>
  public class Crouching : PlayerState {

    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    public override string AnimParam { get { return param; } }
    #endregion

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    private string param = "crouching";
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
      } else if (player.PressedAction()) {
        player.Interact();
      }
    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      if (!player.IsTouchingGround()) {
        ChangeToState<SingleJumpFall>();
      }
    }
    
    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      physics.Velocity = Vector2.zero;
    }


    /// <summary>
    /// Fires when code outside the state machine is trying to send information.
    /// </summary>
    /// <param name="signal">The signal sent.</param>
    public override void OnSignal(GameObject obj) {
      if (CanCarry(obj)) {
        ChangeToState<CarryCrouching>();
      } else if (IsAimableFlingFlower(obj)) {
        ChangeToState<FlingFlowerAim>();
      } else if (IsDirectionalFlingFlower(obj)) {
        ChangeToState<FlingFlowerDirectedLaunch>();
      }
    }
    #endregion
  }
}