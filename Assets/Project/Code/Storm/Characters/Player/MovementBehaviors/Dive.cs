using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player dives into a crouch/crawl.
  /// </summary>
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

    /// <summary>
    /// Animation pre-hook
    /// </summary>
    public void OnDiveFinished() {
      animFinished = true;
    }


    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      if (animFinished && player.CanMove() && player.IsTouchingGround()) {
        ChangeToState<Crawling>();
      }
    }

    /// <summary>
    /// First time initialization for the state. A reference to the player and the player's rigidbody will already have been added by this point.
    /// </summary>
    public override void OnStateAdded() {
      rigidbody = GetComponent<Rigidbody2D>();

      MovementSettings settings = GetComponent<MovementSettings>();

      rightDiveHop = settings.DiveHop;
      leftDiveHop = new Vector2(-rightDiveHop.x, rightDiveHop.y);
    }

    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      animFinished = false;
      if (rigidbody.velocity.x > 0) {
        rigidbody.velocity += rightDiveHop;
      } else {
        rigidbody.velocity += leftDiveHop;
      }
    }
    #endregion
  }
}