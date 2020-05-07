using System.Collections;
using System.Collections.Generic;
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


    #region Collision Detection
    /// <summary>
    /// How thick overlap boxes should be when checking for collision direction.
    /// </summary>
    private float colliderWidth = 0.05f;

    /// <summary>
    /// A reference to the player's rigidbody component.
    /// </summary>
    private new Rigidbody2D rigidbody;
    #endregion

    #region Animation
    private Animator animator;
    #endregion
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      animator = GetComponent<Animator>();
      rigidbody = GetComponent<Rigidbody2D>();
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

    public void TriggerAnimation() {
      animator.SetTrigger("transition");
    }

    public void SetAnimParam(string name) {
      animator.SetTrigger(name);
    }

    public void SetAnimParam(string name, bool value) {
      animator.SetBool(name, value);
    }


    public void SetAnimParam(string name, int value) {
      animator.SetInteger(name, value);
    }

    public void SetAnimParam(string name, float value) {
      animator.SetFloat(name, value);
    }
    #endregion



    #region Collision Detection 

    public bool IsTouchingGround() {
      var playerCollider = GetComponent<BoxCollider2D>();

      Vector2 center = new Vector2(
        playerCollider.bounds.center.x,
        playerCollider.bounds.center.y - playerCollider.bounds.extents.y - this.colliderWidth/2
      );

      Vector2 size = new Vector2(
        playerCollider.size.x,
        this.colliderWidth
      );

      return Physics2D.OverlapBox(center, size, 0) != null;
    }
    #endregion
  }
}