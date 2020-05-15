using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


using Storm.Characters;

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
    private float colliderWidth = 0.25f;

    /// <summary>
    /// A reference to the player's rigidbody component.
    /// </summary>
    public new Rigidbody2D rigidbody;
    #endregion

    #region Animation
    private Animator animator;

    private SpriteRenderer sprite;
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
      BoxCollider2D playerCollider = GetComponent<BoxCollider2D>();

      RaycastHit2D[] hitArr = Physics2D.BoxCastAll(playerCollider.bounds.center, playerCollider.bounds.size, 0, Vector2.down, colliderWidth);
      List<RaycastHit2D> hits = new List<RaycastHit2D>(hitArr);

      return hits.Any(hit => hit.collider.CompareTag("Ground"));
    }

    public bool IsTouchingLeftWall() {
      BoxCollider2D playerCollider = GetComponent<BoxCollider2D>();

      RaycastHit2D[] hitArr = Physics2D.BoxCastAll(
        playerCollider.bounds.center, 
        playerCollider.bounds.size, 
        0, 
        Vector2.left, 
        colliderWidth
      );

      List<RaycastHit2D> hits = new List<RaycastHit2D>(hitArr);

      return hits.Any(hit => hit.collider.CompareTag("Ground"));
    }


    public bool IsTouchingRightWall() {
      BoxCollider2D playerCollider = GetComponent<BoxCollider2D>();

      RaycastHit2D[] hitArr = Physics2D.BoxCastAll(
        playerCollider.bounds.center, 
        playerCollider.bounds.size, 
        0, 
        Vector2.right, 
        colliderWidth
      );
      
      List<RaycastHit2D> hits = new List<RaycastHit2D>(hitArr);

      return hits.Any(hit => hit.collider.CompareTag("Ground"));
    }
    #endregion
  }
}