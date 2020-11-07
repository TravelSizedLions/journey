using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player dives into a crouch/crawl.
  /// </summary>
  [RequireComponent(typeof(MovementSettings))]
  public class Dive : PlayerState {

    #region Fields
    /// <summary>
    /// The vector force for diving to the right.
    /// </summary>
    private Vector2 rightDiveHop;

    /// <summary>
    /// The vector force for diving to the left.
    /// </summary>
    private Vector2 leftDiveHop;

    /// <summary>
    /// Whether or not the animation has finished.
    /// </summary>
    private bool animFinished;
    #endregion

    #region Unity API
    private void Awake() {
      AnimParam = "dive";
    }
    #endregion

    #region Player State API

    public override void OnUpdate() {
      if (player.PressedJump() && !player.IsTouchingGround()) {
        ChangeToState<SingleJumpStart>();
      }
    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      if (animFinished && player.CanMove() && player.IsTouchingGround()) {
        ChangeToState<Crawling>();
      } else if (animFinished && !player.CanMove() && player.IsTouchingGround()) {
        if (player.HoldingDown()) {
          ChangeToState<Crouching>();
        } else {
          ChangeToState<Idle>();
        }
      }
    }

    /// <summary>
    /// First time initialization for the state. A reference to the player and the player's rigidbody will already have been added by this point.
    /// </summary>
    public override void OnStateAdded() {
      MovementSettings settings = GetComponent<MovementSettings>();

      rightDiveHop = settings.DiveHop;
      leftDiveHop = new Vector2(-rightDiveHop.x, rightDiveHop.y);
    }

    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      animFinished = false;
      if (physics.Vx > 0) {
        physics.Velocity += rightDiveHop;
      } else {
        physics.Velocity += leftDiveHop;
      }
    }

    public override void OnSignal(GameObject obj) {
      if (IsAimableFlingFlower(obj)) {
        ChangeToState<FlingFlowerAim>();
      }
    }
    #endregion

    #region Getters/Setters

    /// <summary>
    /// Set the component forces of the dive that will be performed.
    /// </summary>
    /// <param name="horizontal">The horizontal component</param>
    /// <param name="vertical"></param>
    public void SetDiveHop(float horizontal, float vertical) {
      if (horizontal >= 0) {
        rightDiveHop = new Vector2(horizontal, vertical);
        leftDiveHop = new Vector2(-horizontal, vertical);
      } else {
        rightDiveHop = new Vector2(-horizontal, vertical);
        leftDiveHop = new Vector2(horizontal, vertical);
      }
    }

    /// <summary>
    /// Animation pre-hook
    /// </summary>
    public void OnDiveFinished() {
      if (!exited) {
        animFinished = true;
      }
    }

    #endregion
  }
}