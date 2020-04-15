using System;
using System.Collections;
using System.Collections.Generic;
using Storm.Attributes;
using Storm.LevelMechanics.Platforms;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Storm.Characters.Player {

  /// <summary>
  /// The player's normal run and jump movement.
  /// 
  /// This player behavior handles the following:
  /// - running
  /// - jumping and double jumping
  /// - wall jumping and double jumping from a wall jump
  /// - indefinite coyote time (being able to initiate a jump after running off the side of a cliff)
  /// </summary>
  /// <seealso cref="PlayerBehavior" />
  public class NormalMovement : PlayerBehavior {


    #region Player Horizontal Movement Variables
    //---------------------------------------------------------------------
    // Horizontal Movement Variables
    //---------------------------------------------------------------------

    [Header("Running Parameters", order = 0)]
    [Space(5, order = 1)]

    /// <summary>
    /// The player's top horizontal speed.
    /// </summary>
    [Tooltip("The player's top horizontal speed.")]
    public float maxSpeed = 18f;

    /// <summary>
    /// The square of maxSpeed.
    /// </summary>
    protected float maxSqrVelocity;

    /// <summary>
    /// How quickly the player speeds up.
    /// </summary>
    [Tooltip("How quickly the player speeds up to the max speed. 0 means no acceleration. 1 means instantaneously.")]
    [Range(0, 1)]
    public float acceleration = 0.25f;

    /// <summary> 
    /// The calculated acceleration value in units/second^2. 
    /// </summary>
    private float accelerationFactor;

    /// <summary> 
    /// How quickly the player slows down. 
    /// </summary>
    [Tooltip("How quickly the player slows down. 0 means no deceleration. 1 means instantaneous stopping.")]
    [Range(0, 1)]
    public float deceleration = 0.2f;

    /// <summary> 
    /// The calculated deceleration force, (1-deceleration, 1). 
    /// </summary>
    private Vector2 decelerationForce;

    /// <summary>
    /// Whether or not left/right deceleration is enabled.
    /// </summary>
    private bool fastDecelerationEnabled;

    /// <summary>
    /// How quickly the player turns around during movement.
    /// </summary>
    [Tooltip("How quickly the player turns around during movement.")]
    public float reboundMultiplier = 4.0f;

    /// <summary>
    /// Whether or not the player is allowed to move. 
    /// </summary>
    [Tooltip("Whether or not the player is allowed to move.")]
    [SerializeField]
    [ReadOnly]
    private bool isMovingEnabled;

    /// <summary> 
    /// Whether or not the player is currently on the ground.
    /// </summary>
    [Tooltip("Whether or not the player is currently on the ground.")]
    [SerializeField]
    [ReadOnly]
    private bool isOnGround;

    /// <summary>
    /// Whether or not the player is on a moving platform.
    /// </summary>
    [Tooltip("Whether or not the player is on a moving platform.")]
    [SerializeField]
    [ReadOnly]
    private bool isOnMovingPlatform;

    /// <summary> 
    /// Whether or not the player is moving. 
    /// </summary>
    [Tooltip("Whether or not the player is moving.")]
    [ReadOnly]
    private bool isMoving;

    [Space(15, order = 2)]
    #endregion

    #region Jumping Parameters
    //---------------------------------------------------------------------------------------//
    // Jump Variables
    //---------------------------------------------------------------------------------------//
    [Header("Jumping Parameters", order = 3)]
    [Space(5, order = 4)]

    /// <summary> 
    /// The minimum vertical force to apply to a jump from the ground (tapping jump) 
    /// </summary>
    [Tooltip("The minimum force to apply to a jump when jumping from the ground (tapping jump)")]
    public float groundShortHop = 24.0f;

    /// <summary> 
    /// The force variable calculated from groundShortHop. 
    /// </summary>
    private Vector2 groundShortHopForce;


    /// <summary> 
    /// The minimum force to apply to a jump when double jumping (tapping jump) 
    /// </summary>
    [Tooltip("The minimum force to apply to a jump when double jumping (tapping jump)")]
    public float doubleJumpShortHop = 24.0f;

    /// <summary> 
    /// The force variable calculated from doubleJumpShortHop 
    /// </summary>
    private Vector2 doubleJumpShortHopForce;


    /// <summary>
    /// Used to prevent full hop from being applied to a jump more than once.
    /// Calculated to be shortly after fullHopTime
    /// </summary>
    private float jumpTimeMax;

    /// <summary>
    /// A timer used to prevent multiple repeated jump inputs.
    /// </summary>
    private float jumpTimer;

    /// <summary>
    /// Whether or not the player is allowed to jump.
    /// </summary>
    [Tooltip("Whether or not the player is allowed to jump.")]
    [ReadOnly]
    public bool isJumpingEnabled;


    /// <summary>
    /// Whether or not the player has performed their first jump.
    /// </summary>
    [Tooltip("Whether or not the player has performed their first jump.")]
    [ReadOnly]
    public bool hasJumped;

    /// <summary>
    /// Whether or not the player is in a position to double jump.
    /// </summary>
    [Tooltip("Whether or not the player is in a position to double jump.")]
    [ReadOnly]
    public bool canDoubleJump;

    /// <summary>
    /// Whether or not the player has performed their second jump.
    /// </summary>
    [Tooltip("Whether or not the player has performed their second jump.")]
    [ReadOnly]
    public bool hasDoubleJumped;

    /// <summary>
    /// Whether the jump input has been pressed (edge-triggered).
    /// </summary>
    [Tooltip("Whether the jump input has been pressed (edge-triggered).")]
    [SerializeField]
    [ReadOnly]
    private bool jumpInputPressed;

    /// <summary>
    /// Whether the jump input has been pressed (level-triggered).
    /// </summary>
    [Tooltip("Whether the jump input has been pressed (level-triggered).")]
    [SerializeField]
    [ReadOnly]
    private bool jumpInputHeld;

    /// <summary>
    /// Whether the jump input has been released (edge triggered).
    /// </summary>
    [Tooltip("Whether the jump input has been released (edge triggered).")]
    [SerializeField]
    [ReadOnly]
    private bool jumpInputReleased;


    [Space(15, order = 5)]
    #endregion

    #region Wall Interactions
    [Header("Wall Action Parameters", order = 6)]
    [Space(5, order = 7)]

    /// <summary>
    /// How quickly the player slides down the wall. 0 - no friction, 1 - player sticks to wall
    /// </summary>
    [Tooltip("How quickly the player slides down the wall. 0 - no friction, 1 - player sticks to wall")]
    [SerializeField]
    [Range(0, 1)]
    private float wallFriction = 0.07f;


    /// <summary>
    /// How quickly the character loses inertia will against a wall. 0 - immediately, 1 - never.
    /// </summary>
    [Tooltip("How quickly the character loses inertia. \n  0 - immediately \n  1 - never")]
    [SerializeField]
    [Range(0, 1)]
    private float intertialDecay = 0.95f;

    /// <summary>
    /// Whether or not the character has inertia to use.
    /// </summary>
    private bool canUseInertia;

    /// <summary>
    /// The player's inertia after hitting a wall.
    /// </summary>
    [Tooltip("The player's intertia after hitting a wall.")]
    [SerializeField]
    [ReadOnly]
    private Vector2 inertia;

    /// <summary>
    /// Vertical force applied to a wall jump.
    /// </summary>
    [Tooltip("Vertical force applied to a wall jump.")]
    [SerializeField]
    private float wallJump = 16.0f;

    /// <summary>
    /// Force vector calculated from wallJump
    /// </summary>
    private Vector2 wallJumpForce;

    /// <summary>
    /// How easy it is for the player to get back to the wall after a
    /// wall jump. Higher is easier.
    /// </summary>
    [Tooltip("How easy it is for the player to get back to the wall after a wall jump. Higher is easier.")]
    [SerializeField]
    private float wallJumpMuting = 0.08f;

    /// <summary>
    /// Whether or not the player is touching a left-hand wall.
    /// </summary>
    [Tooltip("Whether or not the player is touching a left-hand wall.")]
    [SerializeField]
    [ReadOnly]
    private bool isOnLeftWall;

    /// <summary>
    /// Whether or not the player is touching a right-hand wall.
    /// </summary>
    [Tooltip("Whether or not the player is touching a right-hand wall.")]
    [ReadOnly]
    public bool isOnRightWall;

    /// <summary>
    /// Whether or not the player is in the middle of a wall jump.
    /// </summary>
    [Tooltip("Whether or not the player is in the middle of a wall jump.")]
    [SerializeField]
    [ReadOnly]
    private bool isWallJumping;

    /// <summary>
    /// Whether or not the player is jumping from wall to wall.
    /// </summary>
    [Tooltip("Whether or not the player is jumping from wall to wall.")]
    [SerializeField]
    [ReadOnly]
    private bool isInWallJumpCombo;

    #endregion

    #region Unity API
    //---------------------------------------------------------------------
    // Unity API
    //---------------------------------------------------------------------

    protected override void Awake() {
      base.Awake();
    }

    public void Start() {

      player.isFacingRight = GameManager.Instance.transitions.GetCurrentSpawnFacing();
      anim.SetBool("IsFacingRight", player.isFacingRight);

      rb.freezeRotation = true;

      transform.position = GameManager.Instance.transitions.GetCurrentSpawnPosition();

      wallFriction = 1 - wallFriction;

      accelerationFactor = maxSpeed * acceleration;
      decelerationForce = new Vector2(1 - deceleration, 1);
      maxSqrVelocity = maxSpeed * maxSpeed;

      wallJumpForce = new Vector2(wallJump, 0);
      groundShortHopForce = new Vector2(0, groundShortHop);
      doubleJumpShortHopForce = new Vector2(0, doubleJumpShortHop);
    }


    protected void Update() {
      GatherInput();
    }

    protected void FixedUpdate() {
      touchSensor.sense();
      approachSensor.sense();
      UpdateAnimator();
      MoveCalculations();
      JumpCalculation();
    }


    /// <summary>
    /// Gather player input!
    /// </summary>
    protected void GatherInput() {
      jumpInputPressed = Input.GetKeyDown(KeyCode.Space) || jumpInputPressed;
      jumpInputHeld = Input.GetKey(KeyCode.Space);
      jumpInputReleased = Input.GetKeyUp(KeyCode.Space) || jumpInputReleased;
    }


    /// <summary>
    /// Determine the player's situation for animation purposes.
    /// 
    /// TODO: Refactor Player animator to see if this can be simplified.
    /// </summary>
    protected void UpdateAnimator() {

      // Check movement
      float motion = Mathf.Abs(rb.velocity.x);
      isMoving = motion > 0.3f;
      anim.SetBool("IsMoving", isMoving);


      // Check whether facing left or right
      if (!anim.GetBool("IsFacingRight") && rb.velocity.x > 0.1) {
        anim.SetBool("IsFacingRight", true);
      } else if (rb.velocity.x < -0.1) {
        anim.SetBool("IsFacingRight", false);
      }

      // Update player facing information for camera
      if (isOnGround) {
        if (rb.velocity.x < -0.1) {
          player.isFacingRight = false;
        } else if (rb.velocity.x > 0.1) {
          player.isFacingRight = true;
        }
        // zero case: leave boolean as is
      }

      // Check if the character is touching a wall.
      isOnGround = touchSensor.IsTouchingFloor();
      isOnLeftWall = touchSensor.IsTouchingLeftWall() && !touchSensor.IsTouchingCeiling();
      isOnRightWall = touchSensor.IsTouchingRightWall() && !touchSensor.IsTouchingCeiling();
      anim.SetBool("IsTouchingLeftWall", isOnLeftWall);
      anim.SetBool("IsTouchingRightWall", isOnRightWall);

      if (isOnRightWall || isOnLeftWall) {
        anim.SetBool("IsFalling", false);
        anim.SetBool("IsJumping", false);
        anim.SetBool("IsDoubleJumping", false);
      }

      if (isOnGround) {

        // The character is on the ground.
        anim.SetBool("IsJumping", false);
        anim.SetBool("IsDoubleJumping", false);
        anim.SetBool("IsFalling", false);
        anim.SetBool("IsOnGround", true);

        if (isOnLeftWall || isOnRightWall) {
          if (rb.velocity.y < 0) {
            anim.SetBool("IsTouchingLeftWall", isOnLeftWall);
            anim.SetBool("IsTouchingRightWall", isOnRightWall);
          }
        }
      } else {

        // The character is in the air.

        // Check Whether the character is rising or falling.  
        if (isInWallJumpCombo && (isOnLeftWall || isOnRightWall)) {
          anim.SetBool("IsTouchingLeftWall", isOnLeftWall);
          anim.SetBool("IsTouchingRightWall", isOnRightWall);
        } else if (isOnLeftWall || isOnRightWall) {
          if (rb.velocity.y > -0.75f) {
            if (hasDoubleJumped) {
              anim.SetBool("IsDoubleJumping", true);
            } else {
              anim.SetBool("IsJumping", true);
            }
            anim.SetBool("IsFalling", false);
            anim.SetBool("IsOnGround", false);
          } else {
            anim.SetBool("IsTouchingLeftWall", isOnLeftWall);
            anim.SetBool("IsTouchingRightWall", isOnRightWall);
          }
        } else {
          if (rb.velocity.y > 0) {
            if (hasDoubleJumped) {
              anim.SetBool("IsDoubleJumping", true);
            } else {
              anim.SetBool("IsJumping", true);
            }
            anim.SetBool("IsFalling", false);
            anim.SetBool("IsOnGround", false);
          } else if (rb.velocity.y < 0) {
            anim.SetBool("IsJumping", false);
            anim.SetBool("IsDoubleJumping", false);
            anim.SetBool("IsFalling", true);
            anim.SetBool("IsOnGround", false);
          }
        }

      }

      if (isOnGround && (isOnLeftWall || isOnRightWall) && rb.velocity.y < 0) {
        isOnGround = false;
        anim.SetBool("IsOnGround", false);
        anim.SetBool("IsTouchingLeftWall", isOnLeftWall);
        anim.SetBool("IsTouchingRightWall", isOnRightWall);
      }


    }

    /// <summary>
    /// Perform the horizontal movement of the player.
    /// </summary>
    private void MoveCalculations() {
      float input = Input.GetAxis("Horizontal");

      calculateInertia();

      // Deceleration cases.
      if (fastDecelerationEnabled) {
        if (!isMovingEnabled || (Mathf.Abs(input) != 1 && !isWallJumping)) {
          rb.velocity *= decelerationForce;
          return;
        }
      }

      // Enable fast deceleration if the player is actively moving.
      if (!fastDecelerationEnabled && input != 0) {
        EnableFastDeceleration();
      }

      // Keeps the player from being dragged by 
      // Any moving platforms he may have been on.
      if (!isOnMovingPlatform && input != 0) {
        transform.SetParent(null);
      }

      // If the player is turning around,
      // apply more force so the turn happens faster.
      float inputDirection = Mathf.Sign(input);
      float motionDirection = Mathf.Sign(rb.velocity.x);
      float adjustedInput = inputDirection == motionDirection ? input : input * reboundMultiplier;

      // Wall jump gracefully through the air!
      if (isWallJumping) {
        adjustedInput *= wallJumpMuting;
      }

      if (inputDirection == -1 && isOnLeftWall && !touchSensor.IsTouchingDeadlyObject()) {
        rb.velocity = Vector2.up * rb.velocity;
      } else if (inputDirection == 1 && isOnRightWall && !touchSensor.IsTouchingDeadlyObject()) {
        rb.velocity = Vector2.up * rb.velocity;
      } else {
        float horizSpeed = Mathf.Clamp(rb.velocity.x + adjustedInput * accelerationFactor, -maxSpeed, maxSpeed);
        rb.velocity = new Vector2(horizSpeed, rb.velocity.y);
      }
    }


    /// <summary>
    /// Handles inertia, which allows the character to slide over the top of walls.
    /// </summary>
    private void calculateInertia() {
      if (isOnLeftWall || isOnRightWall) {
        inertia *= intertialDecay;
        canUseInertia = Mathf.Abs(inertia.magnitude) > 0.01;
      } else {
        if (Mathf.Abs(rb.velocity.x) > 0.01) {
          inertia = rb.velocity;
        } else if (canUseInertia && Mathf.Abs(rb.velocity.x) < 0.01) {
          canUseInertia = false;
          if (isInWallJumpCombo) {
            rb.velocity = inertia;
            isWallJumping = true;
          }

          Debug.Log("isWallJumping (Inertia): " + isWallJumping);
        }
      }
    }


    /// <summary>
    /// Handle the jumping related movment of the player.
    /// </summary>
    private void JumpCalculation() {
      if (!isJumpingEnabled) {
        jumpInputPressed = false;
        jumpInputHeld = false;
        jumpInputReleased = false;
        return;
      }

      if (isOnGround) {
        isInWallJumpCombo = false;
        ResetJumpLogic();
      } else {
        bool isOnWall = false;
        Vector2 moveForce = Vector2.zero;
        float movement = rb.velocity.x;

        // Keeps the character from moving into the wall continually
        // (manifests itself as sticking to the wall).
        if (isOnLeftWall) {
          isOnWall = true;
          moveForce = new Vector2(movement > 0 ? movement : 0, 0);
        } else if (isOnRightWall) {
          isOnWall = true;
          moveForce = new Vector2(movement < 0 ? movement : 0, 0);
        }

        if (isOnWall && rb.velocity.y < 0) {
          ResetJumpLogic();
          rb.velocity = rb.velocity * Vector2.up * wallFriction + moveForce;
        }
      }

      if (jumpInputPressed) {
        jumpInputPressed = false;
        handleJumpInputPressed();
      }

      jumpTimer += Time.fixedDeltaTime;


      if (jumpInputReleased) {
        handleJumpInputReleased();
      }

    }

    /// <summary>
    /// Handle the character's jumping ability.
    /// </summary>
    private void handleJumpInputPressed() {

      if (!isOnGround && (isOnLeftWall || isOnRightWall)) {
        performSingleJump();
        handleWallJump();
      } else {
        if (!hasJumped) {
          performSingleJump();
        } else if (canDoubleJump) {
          performDoubleJump();
        }
      }
    }


    private void handleWallJump() {
      if (isOnLeftWall || approachSensor.IsTouchingLeftWall()) {
        performWallJump("left");
      } else if (isOnRightWall || approachSensor.IsTouchingRightWall()) {
        performWallJump("right");
      }
    }

    /// <summary>
    /// Returns true if the character is close by a wall in mid-air.
    /// </summary>
    /// <returns></returns>
    private bool isApproachingWallFromAir() {
      if (!isOnGround) {
        if (approachSensor.IsTouchingLeftWall() && rb.velocity.x < 0) {
          return true;
        } else if (approachSensor.IsTouchingRightWall() && rb.velocity.x > 0) {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Handle releasing the jump button.
    /// </summary>
    private void handleJumpInputReleased() {
      if (isOnLeftWall || isOnRightWall) {
        ResetJumpLogic();
      } else if (!hasDoubleJumped) {
        canDoubleJump = true;
      }
    }

    /// <summary>
    /// Perform a single jump.
    /// </summary>
    private void performSingleJump() {
      hasJumped = true;
      isWallJumping = false;
      jumpTimer = 0;
      rb.velocity = rb.velocity * Vector2.right + groundShortHopForce;
    }

    /// <summary>
    /// Performs a wall jump either left or right depending 
    /// on the input direction.
    /// </summary>
    /// <param name="direction">Either "left" or "right" (case insensitive).</param>
    private void performWallJump(string direction) {
      isWallJumping = true;
      Debug.Log("Performing Wall Jump!");
      isInWallJumpCombo = true;
      hasJumped = true;
      jumpTimer = 0;

      if (direction.ToLower() == "left") {
        rb.velocity = rb.velocity * Vector3.up + wallJumpForce;
        isOnLeftWall = false;
      } else if (direction.ToLower() == "right") {
        rb.velocity = rb.velocity * Vector3.up - wallJumpForce;
        isOnRightWall = false;
      }
    }

    private void performDoubleJump() {
      jumpTimer = 0;
      hasJumped = true;
      canDoubleJump = false;
      hasDoubleJumped = true;
      isWallJumping = false;
      isInWallJumpCombo = false;

      rb.velocity = rb.velocity * Vector2.right + doubleJumpShortHopForce;
    }

    #endregion

    #region Collisions/Triggering
    public void OnCollisionEnter2D(Collision2D collision) {

      // Catches the case where the player lands on solid ground from a 
      // moving platform without directional input.
      EnableFastDeceleration();
      if (collision.collider.GetComponent<MovingPlatform>() == null) {
        DisablePlatformMomentum();
        transform.SetParent(null);
      }

      if (player.touchSensor.IsTouchingFloor()) {
        isWallJumping = false;
      }
    }
    #endregion

    #region Player Behavior API
    //-------------------------------------------------------------------//
    // Player Behavior API
    //-------------------------------------------------------------------//

    public override void Activate() {
      base.Activate();
      ResetJumpLogic();
    }

    public override void Deactivate() {
      base.Deactivate();
    }

    #endregion

    #region External API
    //-------------------------------------------------------------------//
    // External API
    //-------------------------------------------------------------------//

    /// <summary>
    /// Enable the player to move.
    /// </summary>
    public void EnableMoving() {
      isMovingEnabled = true;
    }

    /// <summary>
    /// Keep player from moving.
    /// </summary>
    public void DisableMoving() {
      isMovingEnabled = false;
    }


    /// <summary>
    /// Keep the player from jumping.
    /// </summary>
    public void DisableJump() {
      isJumpingEnabled = false;
    }

    /// <summary>
    /// Enable the player to jump.
    /// </summary>
    public void EnableJump() {
      isJumpingEnabled = true;
    }

    /// <summary>
    /// Disable left/right deceleration for the player until the next time they collide with something.
    /// </summary>
    public void DisableFastDeceleration() {
      fastDecelerationEnabled = false;
    }

    /// <summary>
    /// Enable left/right deceleration for the player.
    /// </summary>
    public void EnableFastDeceleration() {
      fastDecelerationEnabled = true;
    }


    /// <summary>
    /// Resets all logic and timers relating to jumps.
    /// </summary>
    public void ResetJumpLogic() {
      // Allow the character 
      jumpTimer = 0;
      hasJumped = false;
      hasDoubleJumped = false;
      canDoubleJump = false;
      isWallJumping = false;
    }


    ///<summary> Keeps a player's movement tethered to a moving platform. </summary>
    public void EnablePlatformMomentum() {
      isOnMovingPlatform = true;
    }

    ///<summary> Removes player association with a moving platform. </summary>
    public void DisablePlatformMomentum() {
      isOnMovingPlatform = false;
    }
    #endregion

  }
}