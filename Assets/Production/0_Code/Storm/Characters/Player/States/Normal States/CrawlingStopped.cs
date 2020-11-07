using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Components;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is crawling on the floor.
  /// </summary>
  [RequireComponent(typeof(MovementSettings))]
  public class CrawlingStopped : PlayerState {


    #region Unity API
    private void Awake() {
      AnimParam = "crawling_stopped";
    }
    #endregion

    #region Player State API


    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.TryingToMove()) {
        ChangeToState<Crawling>();
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
      }
    }
    #endregion
  }
}