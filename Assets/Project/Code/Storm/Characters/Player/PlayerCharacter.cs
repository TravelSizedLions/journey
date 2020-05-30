using Storm.LevelMechanics.Platforms;
using UnityEngine;

namespace Storm.Characters.Player {


  /// <summary>
  /// The main player script.
  /// </summary>
  /// <remarks>
  /// The player is comprised of states of behavior. See the player's attached animator controller for an idea of this behavior.
  /// </remarks>
  public class PlayerCharacter : MonoBehaviour {
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
    /// A reference to the player's rigidbody component.
    /// </summary>
    public new Rigidbody2D rigidbody;

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
    #endregion

    #endregion



    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      animator = GetComponent<Animator>();
      rigidbody = GetComponent<Rigidbody2D>();
      sprite = GetComponent<SpriteRenderer>();
      coyoteTimer = gameObject.AddComponent<CoyoteTimer>();
      rigidbody.freezeRotation = true;

      playerCollider = GetComponent<BoxCollider2D>();

      groundLayerMask = LayerMask.GetMask("Foreground");
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
      return rigidbody.velocity.y > 0;
    }

    public bool IsFalling() {
      return rigidbody.velocity.y <= 0;
    }


    public void StartCoyoteTime() {
      Debug.Log("Resetting Coyote Time");
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
    /// Checks if the player is trying to jump, and whether or not they're allowed to.
    /// </summary>
    /// <param name="levelTriggered">If true, checks if the player is holding jump, rather than if they just pressed jump.</param>
    /// <returns>True if the player should perform a jump.</returns>
    public bool TryingToJump(bool levelTriggered = false) {
      if (levelTriggered) {
        return Input.GetButton("Jump") && CanJump();
      } else {
        return Input.GetButtonDown("Jump") && CanJump();
      }
    }


    public bool TryingToMove() {
      return CanMove() && Input.GetAxis("Horizontal") != 0;
    }
    #endregion
    
  }
}