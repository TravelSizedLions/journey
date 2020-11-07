﻿using System.Collections;
using System.Collections.Generic;
using Storm.Flexible;
using Storm.Flexible.Interaction;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is rising during their first jump.
  /// </summary>
  public class SingleJumpRise : HorizontalMotion {

    #region Unity API
    private void Awake() {
      AnimParam = "jump_1_rise";
    }
    #endregion

    #region Player State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.PressedJump()) {
        if (!base.TryBufferedJump()) {
          ChangeToState<DoubleJumpStart>();
        }
      } else if (player.PressedAction() || player.PressedAltAction()) {
        player.Interact();
      }
    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();

      player.SetFacing(facing);
      
      if (player.IsFalling()) {
        ChangeToState<SingleJumpFall>();
      } else {
        bool leftWall = player.IsTouchingLeftWall();
        bool rightWall =player.IsTouchingRightWall();
        if ((leftWall || rightWall) && !player.IsWallJumping()) {
          ChangeToState<WallSlide>();
        }
      }
    }

    /// <summary>
    /// Fires when code outside the state machine is trying to send information.
    /// </summary>
    /// <param name="signal">The signal sent.</param>
    public override void OnSignal(GameObject obj) {
      if (CanCarry(obj)) {
        Carriable item = obj.GetComponent<Carriable>();
        item.OnPickup();
        ChangeToState<CarryJumpRise>();
      } else if (IsAimableFlingFlower(obj)) {
        ChangeToState<FlingFlowerAim>();
      }
    }
    #endregion
  }
}
