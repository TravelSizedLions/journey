using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is sliding down a wall.
  /// </summary>
  public class WallSlide : HorizontalMotion {

    #region Fields
    /// <summary>
    /// How much the player is slowed by sliding down the wall.
    /// </summary>
    private float wallSlideDeceleration;
    #endregion

    #region Unity API
    private void Awake() {
      AnimParam = "wall_slide";
    }
    #endregion


    #region Player State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (Input.GetButtonDown("Jump") && player.CanJump()) {
        ChangeToState<WallJump>();
      }
    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {  
      Facing facing = MoveHorizontally();
      
      bool leftWall = player.IsTouchingLeftWall();
      bool rightWall = player.IsTouchingRightWall();

      if (!(leftWall || rightWall)) {
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y-0.2f, player.transform.position.z);
        ChangeToState<Jump1Fall>();
        return;
      } else if (player.IsTouchingGround()) {
        ChangeToState<Idle>();
        return;
      } else {
        float input = Input.GetAxis("Horizontal");
        if ((leftWall && input < 0) || (rightWall && input > 0)) {
          rigidbody.velocity =  new Vector2(0, rigidbody.velocity.y*wallSlideDeceleration); 
        } else {
          rigidbody.velocity =  new Vector2(rigidbody.velocity.x, rigidbody.velocity.y*wallSlideDeceleration); 
        }
      }

      player.SetFacing(facing);
    }
    
    /// <summary>
    /// First time initialization for the state. A reference to the player and the player's rigidbody will already have been added by this point.
    /// </summary>
    public override void OnStateAdded() {
      base.OnStateAdded();

      rigidbody = GetComponent<Rigidbody2D>();

      MovementSettings settings = GetComponent<MovementSettings>();
      wallSlideDeceleration = 1-settings.WallSlideDeceleration;
    }
    #endregion
  }
}