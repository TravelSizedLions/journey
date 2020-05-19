using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  /// <summary>
  /// When the player is running up a wall.
  /// </summary>
  public class WallRun : HorizontalMotion {

    #region Fields
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
    private float ascensionThreshold;
    #endregion

    #region Unity API
    private void Awake() {
      AnimParam = "wall_run";
    }
    #endregion

    #region Player State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (Input.GetButtonDown("Jump")) {
        ChangeToState<WallJump>();
      }
    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      MoveHorizontally();

      bool leftWall = player.IsTouchingLeftWall();
      bool rightWall = player.IsTouchingRightWall();

      // Only keep wall running if you're touching a wall.
      float yVel = rigidbody.velocity.y;

      if (!(leftWall || rightWall)) {
        SwitchState(yVel);
      }
      
      if (yVel < 0) {
        ChangeToState<WallSlide>();
      } else if (yVel < wallRunSpeed) {
        Ascend();
      }
    }

    /// <summary>
    /// Move up the wall a fair amount.
    /// </summary>
    private void Ascend() {

      // You can only keep wall running for so long.
      if (ascensionTimer > 0) {
        ascensionTimer -= Time.fixedDeltaTime;

        // You can only keep wall running while you hold down the jump button.
        if (ascending && Input.GetButton("Jump")) {
          rigidbody.velocity = new Vector2(rigidbody.velocity.x, wallRunSpeed);
          
        } else {
          ascending = false;
        }
      } else {
        if (Mathf.Abs(rigidbody.velocity.x) > 0) {
          rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        }
      }
      
    }

    /// <summary>
    /// Decide whether or not to switch to a rise or a fall.
    /// </summary>
    /// <param name="verticalVelocity">The player's vertical velocity</param>
    private void SwitchState(float verticalVelocity) {
      if (verticalVelocity > 0) {
        ChangeToState<Jump1Rise>();
      } else {
        ChangeToState<Jump1Fall>();
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
      ascensionThreshold = settings.AscensionThreshold;
      rigidbody = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      BoxCollider2D collider = GetComponent<BoxCollider2D>();

      RaycastHit2D hit = Physics2D.Linecast(((Vector2)collider.bounds.center)-new Vector2(0, collider.bounds.extents.y), ((Vector2)collider.bounds.center-new Vector2(0, 10000)));

      float dist = hit.distance;
      if (hit.distance < ascensionThreshold) {
        ascending = true;
        ascensionTimer = ascensionTime;
        rigidbody.velocity = new Vector2(rigidbody.velocity.x, wallRunBoost);
      }
    }

    #endregion
  }

}