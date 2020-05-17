using UnityEngine;

namespace Storm.Characters.Player {


  /// <summary>
  /// The main player script.
  /// </summary>
  /// <remarks>
  /// The player is 
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
    public Facing facing;
    #endregion


    #region Collision Detection
    /// <summary>
    /// How thick overlap boxes should be when checking for collision direction.
    /// </summary>
    private float colliderWidth = 0.1f;

    /// <summary>
    /// A reference to the player's rigidbody component.
    /// </summary>
    public new Rigidbody2D rigidbody;
    #endregion

    #region Animation
    private Animator animator;

    private SpriteRenderer sprite;


    private BoxCollider2D playerCollider;

    private Vector2 boxCast;
    private Vector2 hBoxCast;

    private float boxCastMargin = .5f;
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
      rigidbody.freezeRotation = true;

      playerCollider = GetComponent<BoxCollider2D>();
    }

    private void Start() {
      state = gameObject.AddComponent<Idle>();
      state.HiddenOnStateAdded();
      state.EnterState();
      animator.ResetTrigger("idle");
    }

    // Update is called once per frame
    private void Update() {
      state.OnUpdate();
    }

    private void FixedUpdate() {
      state.OnFixedUpdate();
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

    #region Animation

    public void SetAnimParam(string name) {
      animator.SetTrigger(name);
    }


    public void SetFacing(Facing facing) {
      this.facing = facing;

      if (facing == Facing.Left) {
        sprite.flipX = true;
      } else if (facing == Facing.Right) {
        sprite.flipX = false;
      }
    }
    #endregion
    #endregion


    #region Collision Detection 

    public bool IsTouchingGround() {
      boxCast = ((Vector2)playerCollider.bounds.size) - new Vector2(boxCastMargin, 0);

      RaycastHit2D[] hits = Physics2D.BoxCastAll(
        playerCollider.bounds.center,
        boxCast, 
        0,
        Vector2.down, 
        colliderWidth
      );

      return AnyHits(hits, Vector2.up);
    }

    public bool IsTouchingLeftWall() {
      boxCast = ((Vector2)playerCollider.bounds.size) - new Vector2(0, boxCastMargin); 


      RaycastHit2D[] hits = Physics2D.BoxCastAll(
        playerCollider.bounds.center, 
        boxCast, 
        0, 
        Vector2.left, 
        colliderWidth
      );

      return AnyHits(hits, Vector2.right);
    }


    public bool IsTouchingRightWall() {
      boxCast = ((Vector2)playerCollider.bounds.size) - new Vector2(0, boxCastMargin); 

      RaycastHit2D[] hits = Physics2D.BoxCastAll(
        playerCollider.bounds.center, 
        boxCast, 
        0, 
        Vector2.right, 
        colliderWidth
      );
      
      return AnyHits(hits, Vector2.left);
    }

    private bool AnyHits(RaycastHit2D[] hits, Vector2 normalCheck) {
      for (int i = 0; i < hits.Length; i++) {
        if (hits[i].collider.CompareTag("Ground") && 
            (hits[i].normal.normalized == normalCheck.normalized)) {
          return true;
        }
      }

      return false;
    }
    #endregion
  }
}