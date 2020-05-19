using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
      if (!Input.GetButton("Down")) {
        if (Input.GetAxis("Horizontal") != 0) {
          ChangeToState<Running>();
        } 
      }
    }
    
    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      float input = Input.GetAxis("Horizontal");

      if (!player.IsTouchingGround()) {
        ChangeToState<Jump1Fall>();
      }

      if (input != 0) {
        float inputDirection = Mathf.Sign(input);
        float playerMovement = inputDirection*crawlSpeed;

        rigidbody.velocity = new Vector2(playerMovement, rigidbody.velocity.y);

        Facing facing = (Facing)inputDirection;
        player.SetFacing(facing);
      } else {
        ChangeToState<Crouching>();
      }
    }

    #endregion
  }
}