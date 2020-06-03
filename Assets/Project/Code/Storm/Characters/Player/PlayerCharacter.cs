using Storm.LevelMechanics.Platforms;
using Storm.Services;

using UnityEngine;



namespace Storm.Characters.Player {

  public interface IPlayer {

    #region Properties
    IPhysics physics { get; set; }

    #endregion

    /// <summary>
    /// State change callback for player states. The old state will be detached from the player after this call.
    /// </summary>
    /// <param name="oldState">The old player state.</param>
    /// <param name="newState">The new player state.</param>
    void OnStateChange(PlayerState oldState, PlayerState newState);

    /// <summary>
    /// Sets the animation trigger parameter for a given state.
    /// </summary>
    /// <param name="name">The name of the animation parameter to set.</param>
    void SetAnimParam(string name);

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


  /// <summary>
  /// The main player script.
  /// </summary>
  /// <remarks>
  /// The player is comprised of states of behavior. See the player's attached animator controller for an idea of this behavior.
  /// </remarks>
  public class PlayerCharacter : MonoBehaviour, IPlayer {
    #region Fields
    #region Concurrent State Machines
    /// <summary>
    /// The player's movement state.
    /// </summary>
    private PlayerState state;
    #endregion

    #region Basic Information 
    /// <summary>
    /// Whether the player is facing left or right;
    /// </summary>
    public Facing Facing;


    /// <summary>
    /// Information about the player's physics.
    /// </summary>
    public IPhysics physics { get; set; }
    #endregion

    #region Animation
    /// <summary>
    /// A reference to the player's animator controller.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// A reference to the player's sprite.
    /// </summary>
    private SpriteRenderer sprite;
    #endregion

    #region Collision Testing
    /// <summary>
    /// How thick overlap boxes should be when checking for collision direction.
    /// </summary>
    private float colliderWidth = 0.25f;

    /// <summary>
    /// A reference to the player's box collider.
    /// </summary>
    private BoxCollider2D playerCollider;

    /// <summary>
    /// The box used to detect directional collisions.
    /// </summary>
    private Vector2 boxCast;

    /// <summary>
    /// The vertical & horizontal difference between the player's collider and the box cast.
    /// </summary>
    private float boxCastMargin = .5f;


    /// <summary>
    /// Layer mask that prevents collisions with anything aside from things on the ground layer.
    /// </summary>
    private LayerMask groundLayerMask;
    #endregion

    #region Other Player Information

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

    /// <summary>
    /// Script that handles coyote time for the player.
    /// </summary>
    private CoyoteTimer coyoteTimer;


    /// <summary>
    /// Wrapper class around Unity's static Input class.
    /// </summary>
    private UnityInput UnityInput;
    #endregion

    #endregion



    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      animator = GetComponent<Animator>();
      sprite = GetComponent<SpriteRenderer>();
      coyoteTimer = gameObject.AddComponent<CoyoteTimer>();

      playerCollider = GetComponent<BoxCollider2D>();

      groundLayerMask = LayerMask.GetMask("Foreground");

      UnityInput = new UnityInput();

      var rigidbody = GetComponent<Rigidbody2D>();
      rigidbody.freezeRotation = true;
      physics = gameObject.AddComponent<UnityPhysics>();
    }

    private void Start() {
      state = gameObject.AddComponent<Idle>();
      state.HiddenOnStateAdded();
      state.EnterState();
      animator.ResetTrigger("idle");
    }

    private void Update() {
      state.OnUpdate();
    }

    private void FixedUpdate() {
      state.OnFixedUpdate();
    }


    private void OnCollisionEnter2D(Collision2D collision) {
      if (collision.collider.GetComponent<MovingPlatform>() == null) {
        DisablePlatformMomentum();
        transform.SetParent(null);
      }
    }
    #endregion


    #region State Management
    /// <summary>
    /// State change callback for player states. The old state will be detached from the player after this call.
    /// </summary>
    /// <param name="oldState">The old player state.</param>
    /// <param name="newState">The new player state.</param>
    public void OnStateChange(PlayerState oldState, PlayerState newState) {

      state = newState;
      oldState.ExitState();
      newState.EnterState();
    }

    /// <summary>
    /// Sets the animation trigger parameter for a given state.
    /// </summary>
    /// <param name="name">The name of the animation parameter to set.</param>
    public void SetAnimParam(string name) {
      animator.SetTrigger(name);
    }

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
    #endregion

    #region Collision Detection 

    public float DistanceToGround() {
      Vector2 center = playerCollider.bounds.center;
      Vector2 extents = playerCollider.bounds.extents;

      Vector2 startLeft = center-new Vector2(extents.x, extents.y+0.05f);
      RaycastHit2D hitLeft = Physics2D.Raycast(startLeft, Vector2.down, float.PositiveInfinity, groundLayerMask);
      // Debug.Log("Distance Left: " + hitLeft.distance);

      Vector2 startRight = center-new Vector2(-extents.x, extents.y+0.05f);
      RaycastHit2D hitRight = Physics2D.Raycast(startRight, Vector2.down, float.PositiveInfinity, groundLayerMask);
      // Debug.Log("Distance Right: " + hitRight.distance);

      // Return the closer of the two (needed to catch when a player passes over a ledge).
      float[] distances = {
        hitRight.collider != null ? hitRight.distance : float.PositiveInfinity,
        hitLeft.collider != null ? hitLeft.distance : float.PositiveInfinity
      };
      
      return Mathf.Min(distances);
    }

    /// <summary>
    /// Whether or not the player is touching the ground.
    /// </summary>
    public bool IsTouchingGround() {
      boxCast = ((Vector2)playerCollider.bounds.size) - new Vector2(boxCastMargin, 0);

      RaycastHit2D[] hits = Physics2D.BoxCastAll(
        playerCollider.bounds.center,
        boxCast, 
        0,
        Vector2.down, 
        colliderWidth,
        groundLayerMask
      );

      return AnyHits(hits, Vector2.up);
    }


    public float DistanceToWall() {
      Vector2 center = playerCollider.bounds.center;
      Vector2 extents = playerCollider.bounds.extents;

      float buffer = 0.05f;
      Vector2 horizontalDistance = new Vector2(10000, 0);

      Vector2 startTopLeft = center+new Vector2(-(extents.x+buffer), extents.y);
      RaycastHit2D hitTopLeft = Physics2D.Raycast(startTopLeft, Vector2.left, float.PositiveInfinity, groundLayerMask);
      //Debug.Log("Top Left: " + hitTopLeft.distance);


      Vector2 startTopRight = center+new Vector2(extents.x+buffer, extents.y);
      RaycastHit2D hitTopRight = Physics2D.Raycast(startTopRight, Vector2.right, float.PositiveInfinity, groundLayerMask);
      //Debug.Log("Top Right: " + hitTopRight.distance);


      Vector2 startBottomLeft = center+new Vector2(-(extents.x+buffer), -extents.y);
      RaycastHit2D hitBottomLeft = Physics2D.Raycast(startBottomLeft, Vector2.left, float.PositiveInfinity, groundLayerMask);
      //Debug.Log("Bottom Left: " + hitBottomLeft.distance);


      Vector2 startBottomRight = center+new Vector2(extents.x+buffer, -extents.y);
      RaycastHit2D hitBottomRight = Physics2D.Raycast(startBottomRight, Vector2.right, float.PositiveInfinity, groundLayerMask);
      //Debug.Log("Bottom Right: " + hitBottomRight.distance);

      //float min = Mathf.Min(new float[] {hitTopLeft.distance, hitTopRight.distance, hitBottomLeft.distance, hitBottomRight.distance});
      //Debug.Log("Min: " + min);

      float[] distances = {
        hitTopLeft.collider != null ? hitTopLeft.distance : float.PositiveInfinity,
        hitTopRight.collider != null ? hitTopRight.distance : float.PositiveInfinity,
        hitBottomLeft.collider != null ? hitBottomLeft.distance : float.PositiveInfinity,
        hitBottomRight.collider != null ? hitBottomRight.distance : float.PositiveInfinity,
      };

      return Mathf.Min(distances);
    }

    /// <summary>
    /// Whether or not the player is touching a left-hand wall.
    /// </summary>
    public bool IsTouchingLeftWall() {
      boxCast = ((Vector2)playerCollider.bounds.size) - new Vector2(0, boxCastMargin); 


      RaycastHit2D[] hits = Physics2D.BoxCastAll(
        playerCollider.bounds.center, 
        boxCast, 
        0, 
        Vector2.left, 
        colliderWidth,
        groundLayerMask
      );

      return AnyHits(hits, Vector2.right);
    }

    /// <summary>
    /// Whether or not the player is touching a right-hand wall.
    /// </summary>
    public bool IsTouchingRightWall() {
      boxCast = ((Vector2)playerCollider.bounds.size) - new Vector2(0, boxCastMargin); 

      RaycastHit2D[] hits = Physics2D.BoxCastAll(
        playerCollider.bounds.center, 
        boxCast, 
        0, 
        Vector2.right, 
        colliderWidth,
        groundLayerMask
      );
      
      return AnyHits(hits, Vector2.left);
    }

    /// <summary>
    /// Whether or not a list of raycast hits is in the desired direction.
    /// </summary>
    /// <param name="hits">The list of RaycastHits</param>
    /// <param name="normalDirection">The normal of the direction to check hits against.</param>
    /// <returns>Whether or not there are any ground contacts in the desired direction.</returns>
    private bool AnyHits(RaycastHit2D[] hits, Vector2 normalDirection) {
      for (int i = 0; i < hits.Length; i++) {
        if (hits[i].collider.CompareTag("Ground") && 
            (hits[i].normal.normalized == normalDirection.normalized)) {

          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Whether or not the player is in the middle of a wall jump.
    /// </summary>
    /// <returns></returns>
    public bool IsWallJumping() {
      HorizontalMotion motion = state as HorizontalMotion;
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
      coyoteTimer.Reset();
    }

    public bool InCoyoteTime() {
      return coyoteTimer.InCoyoteTime();
    }

    public void UseCoyoteTime() {
      coyoteTimer.UseCoyoteTime();
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