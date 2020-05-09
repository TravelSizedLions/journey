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
    private MovementBehavior movement;

    /// <summary>
    /// The player's conversation state.
    /// </summary>
    private ConversationBehavior conversation;

    /// <summary>
    /// The player's carry state.
    /// </summary>
    private CarryBehavior carry;
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

      gameObject.AddComponent<HorizontalMotion>();
    }

    private void Start() {
      movement = gameObject.AddComponent<Idle>();
      movement.OnStateEnter(this);
    }

    // Update is called once per frame
    private void Update() {
      movement.HandleInput();
      //conversation.HandleInput();
      //carry.HandleInput();
    }

    private void FixedUpdate() {
      movement.HandlePhysics();
      //carry.HandleInput();
    }
    #endregion


    #region State Management
    /// <summary>
    /// State change callback for movement behaviors. The old behavior will be detached from the player after this call.
    /// </summary>
    /// <param name="oldBehavior">The old movement behavior.</param>
    /// <param name="newBehavior">The new movement behavior.</param>
    public void OnStateChange(MovementBehavior oldBehavior, MovementBehavior newBehavior) {
      movement = newBehavior;
      FireStateChange(oldBehavior, newBehavior);
    }

    /// <summary>
    /// State change callback for conversation behaviors. The old behavior will be detached from the player after this call.
    /// </summary>
    /// <param name="oldBehavior">The old conversation behavior.</param>
    /// <param name="newBehavior">The new conversation behavior.</param>
    public void OnStateChange(ConversationBehavior oldBehavior, ConversationBehavior newBehavior) {
      conversation = newBehavior;
      FireStateChange(oldBehavior, newBehavior);
    }

    /// <summary>
    /// State change callback for carry behaviors. The old behavior will be detached from the player after this call.
    /// </summary>
    /// <param name="oldBehavior">The old carry behavior.</param>
    /// <param name="newBehavior">The new carry behavior.</param>
    public void OnStateChange(CarryBehavior oldBehavior, CarryBehavior newBehavior) {
      carry = newBehavior;
      FireStateChange(oldBehavior, newBehavior);
    }

    /// <summary>
    /// Performs the actions necessary to switch states for any player behavior state machine.
    /// </summary>
    /// <param name="oldBehavior"></param>
    /// <param name="newBehavior"></param>
    private void FireStateChange(PlayerBehavior oldBehavior, PlayerBehavior newBehavior) {
      oldBehavior.OnStateExit(this);
      newBehavior.OnStateEnter(this);
      Destroy(oldBehavior);
    }
    #endregion

    #region Animation

    public void SetAnimParam(string name, bool value) {
      animator.SetBool(name, value);
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