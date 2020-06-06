using Storm.LevelMechanics.Platforms;
using Storm.Services;

using UnityEngine;
using Storm.Subsystems.FSM;

namespace Storm.Characters.Player {

  #region Interface
  public interface IPlayer {

    #region Properties
    IPhysics physics { get; set; }

    ICollisionSensor CollisionSensor { get; set; }

    #endregion

    /// <summary>
    /// Sets the direction that the player is facing.
    /// </summary>
    /// <param name="facing">The direction enum</param>
    void SetFacing(Facing facing);

    /// <summary>
    /// Get the distance to the closest piece of ground.
    /// </summary>
    /// <returns>The distance to the closest piece of ground</returns>
    float DistanceToGround();

    /// <summary>
    /// Whether or not the player is touching the ground.
    /// </summary>
    bool IsTouchingGround();

      /// <summary>
    /// How far the player is from a left-hand wall.
    /// </summary>
    /// <returns>The distance between the player's left side and the closest left-hand wall.</returns>
    float DistanceToLeftWall();

    /// <summary>
    /// How far the player is from a right-hand wall.
    /// </summary>
    /// <returns>The distance between the player's right side and the closest right-hand wall.</returns>
    float DistanceToRightWall();

    /// <summary>
    /// Gets the distance to the closest wall (left or right)
    /// </summary>
    float DistanceToWall();

    /// <summary>
    /// Whether or not the player is touching a left-hand wall.
    /// </summary>
    bool IsTouchingLeftWall();

    /// <summary>
    /// Whether or not the player is touching a right-hand wall.
    /// </summary>
    bool IsTouchingRightWall();


    /// <summary>
    /// Whether or not the player is in the middle of a wall jump.
    /// </summary>
    bool IsWallJumping();


    /// <summary>
    /// Whether or not jumping is enabled for the player.
    /// </summary>
    bool CanJump();

    /// <summary>
    /// Disable jumping for the player.
    /// </summary>
    void DisableJump();

    /// <summary>
    /// Enable jumping for the player.
    /// </summary>
    void EnableJump();


    /// <summary>
    /// Whether or not movement is enabled for the player.
    /// </summary>
    bool CanMove();

    /// <summary>
    /// Disable movement for the player.
    /// </summary>
    void DisableMove();

    /// <summary>
    /// Enable movement for the player.
    /// </summary>
    void EnableMove();

    /// <summary>
    /// Signal that the player detached from a platform.
    /// </summary>
    void DisablePlatformMomentum();

    /// <summary>
    /// Signal that the player is attached to a platform.
    /// </summary>
    void EnablePlatformMomentum();

    /// <summary>
    /// Whether or not the player is attached to a moving platform.
    /// </summary>
    bool IsPlatformMomentumEnabled();

    /// <summary>
    /// Whether or not the player is rising.
    /// </summary>
    bool IsRising();

    /// <summary>
    /// Whether or not the player is falling.
    /// </summary>
    bool IsFalling();

    /// <summary>
    /// Signal that the player has just barely run off of a ledge.
    /// </summary>
    void StartCoyoteTime();

    /// <summary>
    /// Whether or not the player has just barely run off of a ledge.
    /// </summary>
    bool InCoyoteTime();

    /// <summary>
    /// Utilize the remaining coyote time.
    /// </summary>
    void UseCoyoteTime();

    /// <summary>
    /// Checks if the player pressed the jump button.
    /// </summary>
    /// <returns>True if the player pressed the jump button.</returns>
    bool PressedJump();

    /// <summary>
    /// Checks if the player is holding the jump button.
    /// </summary>
    /// <returns>True if the player is holding the jump button.</returns>
    bool HoldingJump();

    /// <summary>
    /// Checks whether or not the player is trying to move horizontally, and whether or not they're allowed to.
    /// </summary>
    /// <returns>True if the player should move.</returns>
    bool TryingToMove();

    /// <summary>
    /// Checks if the player has pressed the down button.
    /// </summary>
    /// <returns>True if the player pressed down in the current frame.</returns>
    bool PressedDown();

    /// <summary>
    /// Checks if the player is holding down the down button.
    /// </summary>
    /// <returns>True if the player is holding down the down button</returns>
    bool HoldingDown();

    /// <summary>
    /// Checks if the player has released the down button.
    /// </summary>
    /// <returns>True if the player has released down.</returns>
    bool ReleasedDown();

    /// <summary>
    /// Gets the horizontal input for the player.
    /// </summary>
    /// <returns>The horizontal input for the player. < 0 means left, > 0 means right, 0 means no movement.</returns>
    float GetHorizontalInput();
  }
  #endregion

  /// <summary>
  /// The main player script.
  /// </summary>
  /// <remarks>
  /// The player is comprised of states of behavior. See the player's attached animator controller for an idea of this behavior.
  /// </remarks>
  public class PlayerCharacter : MonoBehaviour, IPlayer {
    #region Fields
    #region Component Classes
    /// <summary>
    /// Information about the player's physics.
    /// </summary>
    public IPhysics physics { get; set; }

    /// <summary>
    /// Delegate class for collisiong/distance sensing.
    /// </summary>
    public ICollisionSensor CollisionSensor { get; set; }

    /// <summary>
    /// Script that handles coyote time for the player.
    /// </summary>
    private CoyoteTimer CoyoteTimer;

    /// <summary>
    /// Wrapper class around Unity's static Input class.
    /// </summary>
    private UnityInput UnityInput;

    /// <summary>
    /// Player's behavioral state machine
    /// </summary>
    private FiniteStateMachine StateMachine;
    #endregion

    #region Collision Testing
    /// <summary>
    /// A reference to the player's box collider.
    /// </summary>
    private BoxCollider2D playerCollider;

    /// <summary>
    /// Layer mask that prevents collisions with anything aside from things on the ground layer.
    /// </summary>
    private LayerMask groundLayerMask;
    #endregion

    #region Other Player Information
    /// <summary>
    /// Whether the player is facing left or right;
    /// </summary>
    public Facing Facing;

    /// <summary>
    /// A reference to the player's sprite.
    /// </summary>
    private SpriteRenderer sprite;

    /// <summary>
    /// Whether or not the player can jump.
    /// </summary>
    private bool canJump = true;

    /// <summary>
    /// Whether or not the player is allowed to move.
    /// </summary>
    private bool canMove = true;

    /// <summary>
    /// Whether or not the player's momentum should be affected by a platform they're standing on.
    /// </summary>
    private bool isOnMovingPlatform;
    #endregion

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      sprite = GetComponent<SpriteRenderer>();
      CoyoteTimer = gameObject.AddComponent<CoyoteTimer>();

      playerCollider = GetComponent<BoxCollider2D>();

      groundLayerMask = LayerMask.GetMask("Foreground");

      UnityInput = new UnityInput();
      CollisionSensor = new CollisionSensor();

      var rigidbody = GetComponent<Rigidbody2D>();
      rigidbody.freezeRotation = true;
      physics = gameObject.AddComponent<UnityPhysics>();
    }

    private void Start() {
      StateMachine = gameObject.AddComponent<FiniteStateMachine>();
      State state = gameObject.AddComponent<Idle>();
      StateMachine.StartMachine(state);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      if (collision.collider.GetComponent<MovingPlatform>() == null) {
        DisablePlatformMomentum();
        transform.SetParent(null);
      }
    }
    #endregion


    /// <summary>
    /// Sets the direction that the player is facing.
    /// </summary>
    /// <param name="facing">The direction enum</param>
    public void SetFacing(Facing facing) {
      if (facing != Facing.None) {
        this.Facing = facing;
      }

      if (facing == Facing.Left) {
        sprite.flipX = true;
      } else if (facing == Facing.Right) {
        sprite.flipX = false;
      }
    }
 

    #region Collision Detection 

    /// <summary>
    /// How far the player is from the ground.
    /// </summary>
    /// <returns>The distance between the player's feet and the closest piece of ground.</returns>
    public float DistanceToGround() {
      return CollisionSensor.DistanceToGround(
        playerCollider.bounds.center,
        playerCollider.bounds.extents
      );
    }

    /// <summary>
    /// How far the player is from a left-hand wall.
    /// </summary>
    /// <returns>The distance between the player's left side and the closest left-hand wall.</returns>
    public float DistanceToLeftWall() {
      return CollisionSensor.DistanceToLeftWall(
        playerCollider.bounds.center,
        playerCollider.bounds.extents
      );
    }

    /// <summary>
    /// How far the player is from a right-hand wall.
    /// </summary>
    /// <returns>The distance between the player's right side and the closest right-hand wall.</returns>
    public float DistanceToRightWall() {
      return CollisionSensor.DistanceToRightWall(
        playerCollider.bounds.center,
        playerCollider.bounds.extents
      );
    }

    /// <summary>
    /// How far the player is from the closest wall.
    /// </summary>
    /// <returns>The distance between the player and the closest wall.</returns>
    public float DistanceToWall() {
      return CollisionSensor.DistanceToWall(
        playerCollider.bounds.center,
        playerCollider.bounds.extents
      );
    }

    /// <summary>
    /// Whether or not the player is touching a left-hand wall.
    /// </summary>
    public bool IsTouchingLeftWall() {
      return CollisionSensor.IsTouchingLeftWall(
        playerCollider.bounds.center,
        playerCollider.bounds.size
      );
    }

    /// <summary>
    /// Whether or not the player is touching a right-hand wall.
    /// </summary>
    public bool IsTouchingRightWall() {
      return CollisionSensor.IsTouchingRightWall(
        playerCollider.bounds.center,
        playerCollider.bounds.size
      );
    }

    /// <summary>
    /// Whether or not the player is touching the ground.
    /// </summary>
    public bool IsTouchingGround() {
      return CollisionSensor.IsTouchingGround(
        playerCollider.bounds.center,
        playerCollider.bounds.size
      );
    }

    /// <summary>
    /// Whether or not the player is in the middle of a wall jump.
    /// </summary>
    /// <returns></returns>
    public bool IsWallJumping() {
      HorizontalMotion motion = StateMachine.GetState() as HorizontalMotion;
      if (motion != null) {
        return motion.IsWallJumping();
      } else {
        return false;
      }
    }
    #endregion

    #region Getters/Setters

    /// <summary>
    /// Whether or not jumping is enabled for the player.
    /// </summary>
    /// <returns>True if the player is allowed to jump. False otherwise.</returns>
    public bool CanJump() {
      return canJump;
    }

    /// <summary>
    /// Disable jumping for the player.
    /// </summary>
    public void DisableJump() {
      canJump = false;
    }

    /// <summary>
    /// Enable jumping for the player.
    /// </summary>
    public void EnableJump() {
      canJump = true;
    }

    /// <summary>
    /// Whether or not movement is enabled for the player.
    /// </summary>
    /// <returns>True if the player is allowed to move. False otherwise.</returns>
    public bool CanMove() {
      return canMove;
    }

    /// <summary>
    /// Disable movement for the player.
    /// </summary>
    public void DisableMove() {
      canMove = false;
    }

    /// <summary>
    /// Enable movement for the player.
    /// </summary>
    public void EnableMove() {
      canMove = true;
    }

    public void DisablePlatformMomentum() {
      isOnMovingPlatform = false;
    }

    public void EnablePlatformMomentum() {
      isOnMovingPlatform = true;
    }

    public bool IsPlatformMomentumEnabled() {
      return isOnMovingPlatform;
    }


    public bool IsRising() {
      return physics.Vy > 0;
    }

    public bool IsFalling() {
      return physics.Vy <= 0;
    }


    public void StartCoyoteTime() {
      CoyoteTimer.Reset();
    }

    public bool InCoyoteTime() {
      return CoyoteTimer.InCoyoteTime();
    }

    public void UseCoyoteTime() {
      CoyoteTimer.UseCoyoteTime();
    }

    #endregion


    #region Input Checking

    /// <summary>
    /// Checks if the player pressed the jump button.
    /// </summary>
    /// <returns>True if the player pressed the jump button.</returns>
    public bool PressedJump() {
      return UnityInput.GetButtonDown("Jump") && CanJump();
    }

    /// <summary>
    /// Checks if the player is holding the jump button.
    /// </summary>
    /// <returns>True if the player is holding the jump button.</returns>
    public bool HoldingJump() {
      return UnityInput.GetButton("Jump") && CanJump();
    }

    /// <summary>
    /// Checks whether or not the player is trying to move horizontally, and whether or not they're allowed to.
    /// </summary>
    /// <returns>True if the player should move.</returns>
    public bool TryingToMove() {
      return CanMove() && UnityInput.GetHorizontalInput() != 0;
    }

    /// <summary>
    /// Checks if the player has pressed the down button.
    /// </summary>
    /// <returns>True if the player pressed down in the current frame.</returns>
    public bool PressedDown() {
      return UnityInput.GetButtonDown("Down");
    }

    /// <summary>
    /// Checks if the player is holding down the down button.
    /// </summary>
    /// <returns>True if the player is holding down the down button</returns>
    public bool HoldingDown() {
      return UnityInput.GetButton("Down");
    }

    /// <summary>
    /// Checks if the player has released the down button.
    /// </summary>
    /// <returns>True if the player has released down.</returns>
    public bool ReleasedDown() {
      return UnityInput.GetButtonUp("Down");
    }

    public float GetHorizontalInput() {
      return UnityInput.GetHorizontalInput();
    }

    #endregion
    
  }
}