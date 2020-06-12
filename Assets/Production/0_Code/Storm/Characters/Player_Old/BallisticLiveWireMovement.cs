using Storm.Attributes;
using UnityEngine;

namespace Storm.Characters.PlayerOld {


  /// <summary>
  /// Jerrod's movement mode after being shot from a spark launcher.
  /// 
  /// Jerrod, in spark form, takes a ballistic trajectory based on directional inputs
  /// from the player. If the player presses the jump button, Jerrod reverts back to normal
  /// with a small leap. Otherwise, Jerrod returns to normal upon hitting the ground or a wall.
  ///
  /// While in the air, the player still has a little control over Jerrod's trajectory.
  /// </summary>
  public class BallisticLivewireMovement : LivewireMovement {

    #region Air Control Parameters
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    [Header("Air Control", order = 0)]
    [Space(5, order = 1)]

    /// <summary>
    /// Sets how big of a jump the player performs upon exiting LiveWire mode.
    /// </summary>
    [Tooltip("Sets how big of a jump the player performs upon exiting BallisticLiveWire mode.")]
    public float PostTransitJump = 20;

    /// <summary>
    /// How much the player can nudge the ballistic curve left or right. Higher = more sensitive to input.
    /// </summary>
    [Tooltip("How much the player can nudge the ballistic curve left or right.")]
    public float HorizontalAirControl = 0.45f;

    /// <summary>
    /// How much the player can nudge the ballistic curve up or down. Higher = more sensitive to input.
    /// </summary>
    [Tooltip("How much the player can nudge the ballistic curve up or down.")]
    public float VerticalAirControl = 0.35f;

    /// <summary>
    /// How much the player decelerates horizontally when no input is given. 0 - stop immediately, 1 - no deceleration.
    /// </summary>
    [Tooltip("How much the player decelerates horizontally when no input is given after applying air control. 0 - stop immediately, 1 - no deceleration.")]
    [Range(0, 1)]
    public float AirControlDeceleration = 0.9f;

    /// <summary>
    /// Whether or not the player has started using air control.
    /// </summary>
    [Tooltip("Whether or not the player has started using air control.")]
    [SerializeField]
    [ReadOnly]
    private bool usedHorizontalAirControl;

    /// <summary>
    /// The jump force vector calculated from the jump variable.
    /// </summary>
    protected Vector2 jumpForce;
    [Space(15, order = 2)]
    #endregion Air Control Parameters

    #region Appearance Parameters
    [Header("Appearance Parameters", order = 3)]
    [Space(5, order = 4)]

    /// <summary> 
    /// The scaling factor of the spark visual (in both x and y directions).
    /// </summary>
    [Tooltip("The scaling factor of the spark visual")]
    public float SparkSize = 0.7f;

    /// <summary> 
    /// The scaling vector calculated from sparkSize. 
    /// </summary>
    private Vector2 sparkScale;

    /// <summary> 
    /// Temp variable used to save and restore the player's BoxCollider2D dimensions. 
    /// </summary>
    private Vector2 oldColliderSize;

    /// <summary> 
    /// Temp variable used to save and restore the player's BoxCollider2D offsets. 
    /// </summary>
    private Vector2 oldColliderOffset;

    [Space(15, order = 5)]
    #endregion

    #region Input Flags

    [Header("Input", order = 6)]
    [Space(5, order = 7)]

    /// <summary> 
    /// Whether or not the space bar has been pressed. 
    /// </summary>
    [Tooltip("Whether or not the space bar has been pressed.")]
    private bool spacePressed;

    /// <summary>
    /// Input axis for left/right. If the player presses right, this value will be positive.
    /// If the player presses left, this value will be negative.
    /// </summary>
    [Tooltip("Input axis for left/right. If the player presses right, this value will be positive. If the player presses left, this value will be negative.")]
    [SerializeField]
    [ReadOnly]
    private float horizontalAxis;

    /// <summary>
    /// Input axis for left/right. If the player presses up, this value will be positive.
    /// If the player presses down, this value will be negative.
    /// </summary>
    [Tooltip("Input axis for left/right. If the player presses up, this value will be positive. If the player presses down, this value will be negative.")]
    [SerializeField]
    [ReadOnly]
    private float verticalAxis;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    protected override void Awake() {
      base.Awake();
      sparkScale = new Vector2(SparkSize, SparkSize);
    }


    private void Start() {
      oldColliderSize = collider.size;
      oldColliderOffset = collider.offset;

      jumpForce = new Vector2(0, PostTransitJump);
    }


    private void Update() {
      GatherInputs();

      // Return back to normal player movement mid-air
      // if the user presses space.
      if (spacePressed) {
        player.SwitchBehavior(PlayerBehaviorEnum.Normal);
        player.NormalMovement.ResetDoubleJump();
      }
    }

    /// <summary>
    /// Collect user inputs.
    /// </summary>
    private void GatherInputs() {
      spacePressed = Input.GetKeyDown(KeyCode.Space);
      horizontalAxis = Input.GetAxis("Horizontal");
      verticalAxis = Input.GetAxis("Vertical");
    }

    private void FixedUpdate() {
      if (horizontalAxis != 0 || verticalAxis != 0) {

        if (horizontalAxis != 0) {
          usedHorizontalAirControl = true;
        }

        Vector2 nudge = new Vector2(horizontalAxis * HorizontalAirControl, verticalAxis * VerticalAirControl);
        rigidbody.velocity += nudge;

      } else if (usedHorizontalAirControl) {
        // Decelerate Horizontal movement.
        rigidbody.velocity = rigidbody.velocity * Vector2.up + rigidbody.velocity * Vector2.right * AirControlDeceleration;
      }

    }

    private void OnCollisionEnter2D(Collision2D collision) {
      if (player.BallisticLiveWireMovement.enabled && collision.gameObject.layer == LayerMask.NameToLayer("Foreground")) {
        player.SwitchBehavior(PlayerBehaviorEnum.Normal);
      }
    }

    #endregion Unity API

    #region Player Behavior API
    //-------------------------------------------------------------------------
    // Player Behavior API
    //-------------------------------------------------------------------------
    public override void Activate() {
      if (!enabled) {
        base.Activate();

        // Reset animator
        foreach (var param in anim.parameters) {
          anim.SetBool(param.name, false);
        }
        anim.SetBool("LiveWire", true);

        transform.localScale = sparkScale;
        oldColliderOffset = collider.offset;
        oldColliderSize = collider.size;
        collider.offset = Vector2.zero;
        collider.size = Vector2.one;

        usedHorizontalAirControl = false;
        horizontalAxis = 0;
        verticalAxis = 0;
      }
    }


    public override void Deactivate() {
      if (enabled) {
        base.Deactivate();
        anim.SetBool("LiveWire", false);


        player.TouchSensor.Sense();
        if (player.TouchSensor.IsTouchingFloor()) {
            if (rigidbody.velocity.y < 0) {
            rigidbody.velocity = rigidbody.velocity * Vector2.right + jumpForce;
          } else {
            rigidbody.velocity += jumpForce;
          }
        } else if (Input.GetKey(KeyCode.Space)) {
          if (rigidbody.velocity.y < 0) {
            rigidbody.velocity = rigidbody.velocity * Vector2.right + jumpForce;
          } else {
            rigidbody.velocity += jumpForce;
          }
        }

        float verticalAdjust = collider.bounds.extents.y;

        transform.localScale = Vector2.one;
        collider.offset = oldColliderOffset;
        collider.size = oldColliderSize;

        usedHorizontalAirControl = false;
        horizontalAxis = 0;
        verticalAxis = 0;

        verticalAdjust = transform.position.y + collider.bounds.extents.y - verticalAdjust;
        transform.position = new Vector3(transform.position.x, verticalAdjust, transform.position.z);




      }
    }

    #endregion Player Behavior API

    #region Public Interface

    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    public void SetInitialVelocity(Vector2 velocity) {
      rigidbody.velocity = velocity;
      anim.SetBool("IsFacingRight", rigidbody.velocity.x > 0);
    }

    public override void SetDirection(Vector2 direction) {
      direction = direction.normalized;
      rigidbody.velocity = direction * rigidbody.velocity.magnitude;
    }
    #endregion
  }

}