using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Services;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is crawling on the floor.
  /// </summary>
  public class Crawling : PlayerState {

    #region Fields
    /// <summary>
    /// The speed at which the player crawls.
    /// </summary>
    private float crawlSpeed;
    #endregion

    #region Unity API
    private void Awake() {
      AnimParam = "crawling";
    }
    #endregion

    #region Player State API

    /// <summary>
    /// First time initialization for the state. A reference to the player and the player's rigidbody will already have been added by this point.
    /// </summary>
    public override void OnStateAdded() {
      MovementSettings settings = GetComponent<MovementSettings>();
      crawlSpeed = settings.CrawlSpeed;
    }

    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (!player.HoldingDown() && player.TryingToMove()) {
        ChangeToState<Running>();
      }
    }
    
    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {

      float input = player.GetHorizontalInput();

      if (!player.IsTouchingGround()) {
        player.StartCoyoteTime();
        ChangeToState<Jump1Fall>();
      }

      if (!player.CanMove()) {
        return;
      }

      if (input != 0 && player.CanMove()) {
        float inputDirection = Mathf.Sign(input);
        float playerMovement = inputDirection*crawlSpeed;

        physics.Vx = playerMovement;

        Facing facing = (Facing)inputDirection;
        player.SetFacing(facing);
      } else {
        physics.Vx = 0;
        ChangeToState<Crouching>();
      }

    }

    #endregion
  }
}