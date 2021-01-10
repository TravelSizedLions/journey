using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HumanBuilders {

  /// <summary>
  /// When the player is crawling on the floor.
  /// </summary>
  [RequireComponent(typeof(MovementSettings))]
  public class CrawlingStopped : PlayerState {
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
    private string param = "crawling_stopped";
    #endregion


    #region Player State API

    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.TryingToMove()) {
        ChangeToState<Crawling>();
      } else if (player.PressedAction() || player.PressedAltAction()) {
        player.Interact();
      }
    }
    
    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      if (!player.IsTouchingGround()) {
        player.StartCoyoteTime();
        ChangeToState<SingleJumpFall>();
      }
    }

    public override void OnSignal(GameObject obj) {
      if (IsAimableFlingFlower(obj)) {
        ChangeToState<FlingFlowerAim>();
      } else if (IsDirectionalFlingFlower(obj)) {
        ChangeToState<FlingFlowerDirectedLaunch>();
      }
    }
    #endregion
  }
}