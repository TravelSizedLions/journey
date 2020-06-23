using Storm.Attributes;
using Storm.Components;
using Storm.Flexible;
using Storm.Flexible.Interaction;
using Storm.LevelMechanics.Platforms;
using Storm.Subsystems.FSM;
using Storm.Subsystems.Transitions;
using UnityEngine;

namespace Storm.Characters.Player {

  #region Interface

  /// <summary>
  /// The player interface.
  /// </summary>
  public interface IPlayer {

    #region Properties

    IPhysicsComponent Physics { get; set; }

    ICollisionComponent CollisionSensor { get; set; }

    IInteractionComponent Interaction { get; set; }

    Carriable CarriedItem { get; set; }

    Facing Facing { get; }

    Vector2 Center { get; }
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
    /// Checks if the player has pressed the up button.
    /// </summary>
    /// <returns>True if the player pressed up in the current frame.</returns>
    bool PressedUp();

    /// <summary>
    /// Checks if the player is holding down the up button.
    /// </summary>
    /// <returns>True if the player is holding down the up button</returns>
    bool HoldingUp();

    /// <summary>
    /// Checks if the player has released the up button.
    /// </summary>
    /// <returns>True if the player has released up.</returns>
    bool ReleasedUp();

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

    bool PressedAction();

    bool HoldingAction();

    bool ReleasedAction();

    Transform GetTransform();

    void TryInteract();
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

    public IPhysicsComponent Physics { get; set; }

    /// <summary>
    /// Delegate class for collisiong/distance sensing.
    /// </summary>
    public ICollisionComponent CollisionSensor { get; set; }

    /// <summary>
    /// Delegate class for interacting with stuff.
    /// </summary>
    public IInteractionComponent Interaction { get; set; }

    /// <summary>
    /// Delegate class for indicating different player interactions.
    /// </summary>
    private PlayerIndication indicator;

    /// <summary>
    /// Script that handles coyote time for the player.
    /// </summary>
    private CoyoteTimer coyoteTimer;

    /// <summary>
    /// Wrapper class around Unity's static Input class.
    /// </summary>
    private UnityInput unityInput;

    /// <summary>
    /// Player's behavioral state machine
    /// </summary>
    private FiniteStateMachine stateMachine;
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
    /// A reference to the player's sprite.
    /// </summary>
    private SpriteRenderer sprite;

    [Header("Debug Information", order = 0)]
    [Space(5, order = 1)]

    /// <summary>
    /// Whether the player is facing left or right.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [Tooltip("Whether the player is facing left or right.")]
    private Facing facing;
    public Facing Facing { get { return facing; } }

    /// <summary>
    /// The center of the player's collider.
    /// </summary>
    public Vector2 Center { get { return playerCollider.bounds.center; } }

    /// <summary>
    /// Whether or not the player is allowed to jump.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [Tooltip("Whether or not the player is allowed to jump.")]
    private bool canJump = true;

    /// <summary>
    /// Whether or not the player is allowed to move.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [Tooltip("Whether or not the player is allowed to move.")]
    private bool canMove = true;

    /// <summary>
    /// Whether or not the player is allowed to crouch.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [Tooltip("Whether or not the player is allowed to crouch.")]
    private bool canCrouch = true;

    /// <summary>
    /// Whether or not the player is standing on a platform.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [Tooltip("Whether or not the player is standing on a platform.")]
    private bool isOnMovingPlatform;


    /// <summary>
    /// The object the player is carrying at the moment.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [Tooltip("The object the player is carrying at the moment.")]
    private Carriable carriedItem;

    /// <summary>
    /// The object the player is carrying
    /// </summary>
    /// <value>The object the player should be carrying.</value>
    public Carriable CarriedItem {
      get { return carriedItem; }
      set { carriedItem = value; }
    }
    #endregion

    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      
      sprite = GetComponent<SpriteRenderer>();
      coyoteTimer = gameObject.AddComponent<CoyoteTimer>();
      playerCollider = GetComponent<BoxCollider2D>();

      unityInput = new UnityInput();
      CollisionSensor = new CollisionComponent();

      Physics = gameObject.AddComponent<PhysicsComponent>();
      indicator = GetComponent<PlayerIndication>();

      stateMachine = gameObject.AddComponent<FiniteStateMachine>();
      State state = gameObject.AddComponent<Idle>();
      stateMachine.StartMachine(state);

      InteractionSettings interactionSettings = GetComponent<InteractionSettings>();
      Interaction = new InteractionComponent(this, stateMachine, interactionSettings);
    }

    private void Start() {

      TransitionManager.Instance.RespawnPlayer(this);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      if (collision.collider.GetComponent<MovingPlatform>() == null) {
        DisablePlatformMomentum();
        transform.SetParent(null);
      }
    }
    #endregion

    #region Getting Information About Player Context
    /// <summary>
    /// Whether or not the player is in the middle of a wall jump.
    /// </summary>
    /// <returns></returns>
    public bool IsWallJumping() {
      HorizontalMotion motion = stateMachine.GetCurrentState() as HorizontalMotion;
      if (motion != null) {
        return motion.IsWallJumping();
      } else {
        return false;
      }
    }

    #endregion


    #region Getters/Setters
    public Transform GetTransform() {
      return transform;
    }

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

    /// <summary>
    /// Disable crouching for the player.
    /// </summary>
    public void DisableCrouch() {
      canCrouch = false;
    }

    /// <summary>
    /// Enable crouching for the player.
    /// </summary>
    public void EnableCrouch() {
      canCrouch = true;
    }

    /// <summary>
    /// Whether or not crouching is enabled for the player.
    /// </summary>
    public bool CanCrouch() {
      return canCrouch;
    }

    /// <summary>
    /// Whether or not the player is crouching.
    /// </summary>
    /// <returns>True if the player is crouching or starting/ending a crouch,
    /// false otherwise.</returns>
    public bool IsCrouching() {
      return stateMachine.IsInState<CrouchStart>() ||
        stateMachine.IsInState<Crouching>() ||
        stateMachine.IsInState<CrouchEnd>();
    }

    /// <summary>
    /// Whether or not the player is crawling.
    /// </summary>
    public bool IsCrawling() {
      return stateMachine.IsInState<Crawling>();
    }

    /// <summary>
    /// Whether or not the player is diving into a crawl.
    /// </summary>
    public bool IsDiving() {
      return stateMachine.IsInState<Dive>();
    }

    /// <summary>
    /// Disables player platform momentum.
    /// </summary>
    public void DisablePlatformMomentum() {
      isOnMovingPlatform = false;
    }

    /// <summary>
    /// Enables player platform momentum.
    /// </summary>
    public void EnablePlatformMomentum() {
      isOnMovingPlatform = true;
    }

    /// <summary>
    /// Whether or not platform momentum is enabled.
    /// </summary>
    /// <returns>True if the player is on a platform, false otherwise.</returns>
    public bool IsPlatformMomentumEnabled() {
      return isOnMovingPlatform;
    }

    /// <summary>
    /// Whether or not the player is rising.
    /// </summary>
    /// <returns>True if the player's vertical velocity is above 0. False otherwise.</returns>
    public bool IsRising() {
      return Physics.Vy > 0;
    }

    /// <summary>
    /// Whether or not the player is falling.
    /// </summary>
    /// <returns>True if the player's vertical velocity is less than or equal to
    /// 0. False otherwise.</returns>
    public bool IsFalling() {
      return Physics.Vy <= 0;
    }

    /// <summary>
    /// Sets the direction that the player is facing.
    /// </summary>
    /// <param name="facing">The direction enum</param>
    public void SetFacing(Facing facing) {
      if (facing != Facing.None) {
        this.facing = facing;
      }

      if (facing == Facing.Left) {
        transform.localScale = new Vector3(-1, 1, 1);
        if (indicator.CurrentIndicator != null) {
          indicator.CurrentIndicator.transform.localScale = new Vector3(-1, 1, 1);
        }
      } else if (facing == Facing.Right) {
        transform.localScale = new Vector3(1, 1, 1);
        if (indicator.CurrentIndicator != null) {
          indicator.CurrentIndicator.transform.localScale = new Vector3(1, 1, 1);
        }
      }
    }
    #endregion

    #region Delegation for Collision/Distance Detection 

    /// <summary>
    /// How far the player is from the ground.
    /// </summary>
    /// <returns>The distance between the player's feet and the closest piece of ground.</returns>
    public float DistanceToGround() => CollisionSensor.DistanceToGround(playerCollider.bounds.center, playerCollider.bounds.extents);

    /// <summary>
    /// How far the player is from a left-hand wall.
    /// </summary>
    /// <returns>The distance between the player's left side and the closest left-hand wall.</returns>
    public float DistanceToLeftWall() => CollisionSensor.DistanceToLeftWall(playerCollider.bounds.center, playerCollider.bounds.extents);

    /// <summary>
    /// How far the player is from a right-hand wall.
    /// </summary>
    /// <returns>The distance between the player's right side and the closest right-hand wall.</returns>
    public float DistanceToRightWall() => CollisionSensor.DistanceToRightWall(playerCollider.bounds.center, playerCollider.bounds.extents);

    /// <summary>
    /// How far the player is from the closest wall.
    /// </summary>
    /// <returns>The distance between the player and the closest wall.</returns>
    public float DistanceToWall() => CollisionSensor.DistanceToWall(playerCollider.bounds.center, playerCollider.bounds.extents);

    /// <summary>
    /// Whether or not the player is touching a left-hand wall.
    /// </summary>
    public bool IsTouchingLeftWall() => CollisionSensor.IsTouchingLeftWall(playerCollider.bounds.center, playerCollider.bounds.size);

    /// <summary>
    /// Whether or not the player is touching a right-hand wall.
    /// </summary>
    public bool IsTouchingRightWall() => CollisionSensor.IsTouchingRightWall(playerCollider.bounds.center, playerCollider.bounds.size);

    /// <summary>
    /// Whether or not the player is touching the ground.
    /// </summary>
    public bool IsTouchingGround() => CollisionSensor.IsTouchingGround(playerCollider.bounds.center, playerCollider.bounds.size);
    #endregion

    #region Input Checking Delegation

    /// <summary>
    /// Checks if the player pressed the jump button.
    /// </summary>
    /// <returns>True if the player pressed the jump button.</returns>
    public bool PressedJump() => unityInput.GetButtonDown("Jump") && CanJump();

    /// <summary>
    /// Checks if the player is holding the jump button.
    /// </summary>
    /// <returns>True if the player is holding the jump button.</returns>
    public bool HoldingJump() => unityInput.GetButton("Jump") && CanJump();

    /// <summary>
    /// Checks whether or not the player is trying to move horizontally, and whether or not they're allowed to.
    /// </summary>
    /// <returns>True if the player should move.</returns>
    public bool TryingToMove() => CanMove() && unityInput.GetHorizontalInput() != 0;

    /// <summary>
    /// Checks if the player has pressed the up button.
    /// </summary>
    /// <returns>True if the player pressed up in the current frame.</returns>
    public bool PressedUp() => unityInput.GetButtonDown("Up") && CanCrouch();

    /// <summary>
    /// Checks if the player is holding down the up button.
    /// </summary>
    /// <returns>True if the player is holding down the up button</returns>
    public bool HoldingUp() => unityInput.GetButton("Up") && CanCrouch();

    /// <summary>
    /// Checks if the player has released the up button.
    /// </summary>
    /// <returns>True if the player has released up.</returns>
    public bool ReleasedUp() => unityInput.GetButtonUp("Up");


    /// <summary>
    /// Checks if the player has pressed the down button.
    /// </summary>
    /// <returns>True if the player pressed down in the current frame.</returns>
    public bool PressedDown() => unityInput.GetButtonDown("Down") && CanCrouch();

    /// <summary>
    /// Checks if the player is holding down the down button.
    /// </summary>
    /// <returns>True if the player is holding down the down button</returns>
    public bool HoldingDown() => unityInput.GetButton("Down") && CanCrouch();

    /// <summary>
    /// Checks if the player has released the down button.
    /// </summary>
    /// <returns>True if the player has released down.</returns>
    public bool ReleasedDown() => unityInput.GetButtonUp("Down");

    /// <summary>
    /// Gets the horizontal input axis for the player.
    /// </summary>
    /// <returns>The horizontal input of the player from -1 (left) to 1 (right)</returns>
    public float GetHorizontalInput() => unityInput.GetHorizontalInput();

    /// <summary>
    /// Whether or not the player has pressed the action button.
    /// </summary>
    /// <returns>True if the player has press the action button. False otherwise.</returns>
    public bool PressedAction() => unityInput.GetButtonDown("Action");

    /// <summary>
    /// Whether or not the player is holding the action button down.
    /// </summary>
    /// <returns>True if the player is holding action button down. False otherwise.</returns>
    public bool HoldingAction() => unityInput.GetButton("Action");

    /// <summary>
    /// Whether or not the player has released the action button.
    /// </summary>
    /// <returns>True if the palyer has released the action button. False otherwise.</returns>
    public bool ReleasedAction() => unityInput.GetButtonUp("Action");

    #endregion

    #region Coyote Time Delegation
    /// <summary>
    /// Starts coyote time for the player. After leaving a ledge, the player will still have a fraction of a
    /// second to input a jump.
    /// </summary>
    public void StartCoyoteTime() => coyoteTimer.Reset();


    /// <summary>
    /// Whether or not the player still has time to input a jump after leaving a
    /// ledge.
    /// </summary>
    /// <returns>True if the player still has time to jump. False otherwise.</returns>
    public bool InCoyoteTime() => coyoteTimer.InCoyoteTime();


    /// <summary>
    /// Use up the remaining coyote time. This should be called after the player
    /// performs a jump just after walking off a ledge.
    /// </summary>
    public void UseCoyoteTime() => coyoteTimer.UseCoyoteTime();

    #endregion

    #region Indicator Delegation
    /// <summary>
    /// Add a question mark indicator over the player's head.
    /// </summary>
    public void AddIndicator(string name) => indicator.AddIndicator(name);

    /// <summary>
    /// Remove the question mark indicator over the player's head.
    /// </summary>
    public void RemoveIndicator() => indicator.RemoveIndicator();

    /// <summary>
    /// Whether or not the player has an indicator over the player's head.
    /// </summary>
    /// <returns>True if the player has an indicator over the player's head.</returns>
    public bool HasIndicator() => indicator.HasIndicator();

    #endregion

    #region Interaction
    
    public void TryInteract() => Interaction.TryInteract();
    #endregion
  }
}