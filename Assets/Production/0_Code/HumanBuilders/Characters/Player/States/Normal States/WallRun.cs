﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// When the player is running up a wall.
  /// </summary>
  public class WallRun : HorizontalMotion {
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
    private string param = "wall_run";

    /// <summary>
    /// How long the player can hold jump to ascend the wall.
    /// </summary>
    private float ascensionTime;

    /// <summary>
    /// Times how long the player has been ascending a wall.
    /// </summary>
    private float ascensionTimer;

    /// <summary>
    /// How quickly the player runs up a wall.
    /// </summary>
    private float wallRunSpeed;

    /// <summary>
    /// The initial upward velocity the player starts with when ascending a wall from the ground.
    /// </summary>
    private float wallRunBoost;

    /// <summary>
    /// Whether or not the player is ascending the wall.
    /// </summary>
    private bool ascending;

    /// <summary>
    /// How close the player needs to be to the ground to start ascending the wall.
    /// </summary>
    private float wallRunBuffer;
    #endregion

    #region Player State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.PressedJump()) {
        ChangeToState<WallJump>();
      }
    }

    #region OnFixedUpdate logic stack.
    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      MoveHorizontally();

      bool leftWall = player.IsTouchingLeftWall();
      bool rightWall = player.IsTouchingRightWall();

      if (!(leftWall || rightWall)) {
        SwitchState(physics.Vy);
      } else if (player.IsFalling()) {
        ChangeToState<WallSlide>();
      } else if (physics.Vy < wallRunSpeed) {
        Ascend();
      }
    }

    /// <summary>
    /// Move up the wall a fair amount.
    /// </summary>
    /// <returns> Whether or not the player is still ascending. </returns>
    public bool Ascend() {
      // You can only keep wall running for so long.
      if (ascensionTimer > 0) {
        ascensionTimer -= Time.fixedDeltaTime;

        // You can only keep wall running while you hold down the jump button.
        if (ascending && player.HoldingJump()) {
          physics.Vy = wallRunSpeed;    
          return true;      
        } else {
          ascending = false;
        }
      } else {
        if (Mathf.Abs(physics.Vx) > 0) {
          physics.Vx = 0;
        }
      }

      return false;
    }

    /// <summary>
    /// Decide whether or not to switch to a rise or a fall.
    /// </summary>
    /// <param name="verticalVelocity">The player's vertical velocity</param>
    public void SwitchState(float verticalVelocity) {
      if (verticalVelocity > 0) {
        ChangeToState<SingleJumpRise>();
      } else {
        ChangeToState<SingleJumpFall>();
      }
    }
    #endregion

    public override void OnSignal(GameObject obj) {
      if (IsAimableFlingFlower(obj)) {
        ChangeToState<FlingFlowerAim>();
      } else if (IsDirectionalFlingFlower(obj)) {
        ChangeToState<FlingFlowerDirectedLaunch>();
      }
    }

    /// <summary>
    /// First time initialization for the state. A reference to the player and the player's rigidbody will already have been added by this point.
    /// </summary>
    public override void OnStateAdded() {
      base.OnStateAdded();

      MovementSettings settings = GetComponent<MovementSettings>();
      wallRunSpeed = settings.WallRunSpeed;
      wallRunBoost = settings.WallRunBoost;
      ascensionTime = settings.WallRunAscensionTime;
      wallRunBuffer = settings.WallRunVertBuffer;
    }

    #region OnStateEnter() logic stack.
    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      BoxCollider2D collider = GetComponent<BoxCollider2D>();

      // Adjust player facing to gaurantee its correct.
      // Project the player's position to exactly the wall's position if the
      // player isn't actually touching tbe wall. 
      // (Looks better and produces more predictable physics)
      Facing whichWall = ProjectToWall();
      player.SetFacing(whichWall);

      if (!player.IsTouchingGround()) {
        float dist = player.DistanceToGround();

        if (dist < wallRunBuffer) {
          StartAscension();
        }
      } else {
        StartAscension();
      }

    }

    /// <summary>
    /// Make the player start running up the wall.
    /// </summary>
    private void StartAscension() {
      ascending = true;
      ascensionTimer = ascensionTime;
      physics.Vy = wallRunBoost;
    }
    #endregion

    #endregion
  }

}