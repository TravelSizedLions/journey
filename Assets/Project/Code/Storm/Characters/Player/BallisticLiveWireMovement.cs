using UnityEngine;

using Storm.Attributes;

namespace Storm.Characters.Player {


  /// <summary>
  /// Jerrod's movement mode after being shot from a spark launcher.
  /// 
  /// Jerrod, in spark form, takes a ballistic trajectory based on directional inputs
  /// from the player. If the player presses the jump button, Jerrod reverts back to normal
  /// with a small leap. Otherwise, Jerrod returns to normal upon hitting the ground or a wall.
  ///
  /// While in the air, the player still has a little control over Jerrod's trajectory.
  /// </summary>
  public class BallisticLiveWireMovement : LiveWireMovement {
    
    #region Air Control Parameters
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    [Header("Air Control", order=0)]
    [Space(5, order=1)]

    /// <summary>
    /// Sets how big of a jump the player performs upon exiting LiveWire mode.
    /// </summary>
    [Tooltip("Sets how big of a jump the player performs upon exiting BallisticLiveWire mode.")]
    public float PostTransitJump;

    /// <summary>
    /// How much the player can nudge the ballistic curve left or right. Higher = more sensitive to input.
    /// </summary>
    [Tooltip("How much the player can nudge the ballistic curve left or right.")]
    public float HorizontalAirControl;

    /// <summary>
    /// How much the player can nudge the ballistic curve up or down. Higher = more sensitive to input.
    /// </summary>
    [Tooltip("How much the player can nudge the ballistic curve up or down.")]
    public float VerticalAirControl;

    /// <summary>
    /// How much the player decelerates horizontally when no input is given. 0 - stop immediately, 1 - no deceleration.
    /// </summary>
    [Tooltip("How much the player decelerates horizontally when no input is given after applying air control. 0 - stop immediately, 1 - no deceleration.")]
    [Range(0, 1)]
    public float airControlDeceleration;

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
    protected Vector2 JumpForce;
    [Space(15, order=2)]
    #endregion Air Control Parameters

    #region Appearance Parameters
    [Header("Appearance Parameters", order=3)]
    [Space(5, order=4)]

    /// <summary> The scaling factor of the spark visual (in both x and y directions).false </summary>
    [Tooltip("The scaling factor of the spark visual")]
    public float SparkSize;

    /// <summary> The scaling vector calculated from sparkSize. </summary>
    private Vector2 SparkScale;

    /// <summary> Temp variable used to save and restore the player's BoxCollider2D dimensions. </summary>
    private Vector2 OldColliderSize;

    /// <summary> Temp variable used to save and restore the player's BoxCollider2D offsets. </summary>
    private Vector2 OldColliderOffset;

    [Space(15, order=5)]
    #endregion

    #region Input Flags

    [Header("Input", order=6)]
    [Space(5, order=7)]

    /// <summary> Whether or not the space bar has been pressed. </summary>
    [Tooltip("Whether or not the space bar has been pressed.")]
    private bool SpacePressed;

    /// <summary>
    /// Input axis for left/right. If the player presses right, this value will be positive.
    /// If the player presses left, this value will be negative.
    /// </summary>
    [Tooltip("Input axis for left/right. If the player presses right, this value will be positive. If the player presses left, this value will be negative.")]
    [SerializeField]
    [ReadOnly]
    private float HorizontalAxis;

    /// <summary>
    /// Input axis for left/right. If the player presses up, this value will be positive.
    /// If the player presses down, this value will be negative.
    /// </summary>
    [Tooltip("Input axis for left/right. If the player presses up, this value will be positive. If the player presses down, this value will be negative.")]
    [SerializeField]
    [ReadOnly]
    private float VerticalAxis;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    protected override void Awake() {
      base.Awake();
      SparkScale = new Vector2(SparkSize, SparkSize);
    }
    
    
    public void Start() {
      OldColliderSize = collider.size;
      OldColliderOffset = collider.offset;

      JumpForce = new Vector2(0, PostTransitJump);
    }
    
    
    public void Update() {
      GatherInputs();
      if (SpacePressed) {
        player.SwitchBehavior(PlayerBehaviorEnum.Normal);
        player.normalMovement.hasJumped = true;
        player.normalMovement.hasDoubleJumped = true;

        
      }
    }

    /// <summary>
    /// Collect user inputs.
    /// </summary>
    public void GatherInputs() {
      SpacePressed = Input.GetKeyDown(KeyCode.Space);
      HorizontalAxis = Input.GetAxis("Horizontal");
      VerticalAxis = Input.GetAxis("Vertical");
    }
    
    public void FixedUpdate() {
      if (HorizontalAxis != 0 || VerticalAxis != 0) {

        if (HorizontalAxis != 0) {
          usedHorizontalAirControl = true;
        }
        
        Vector2 nudge = new Vector2(HorizontalAxis*HorizontalAirControl,VerticalAxis*VerticalAirControl);
        rb.velocity += nudge;

      } else if (usedHorizontalAirControl) {
        // Decelerate Horizontal movement.
        rb.velocity = rb.velocity*Vector2.up + rb.velocity*Vector2.right*airControlDeceleration;
      }

    }


    public void OnCollisionEnter2D(Collision2D collision) {
      Debug.Log("Collision!");
      if (collision.gameObject.layer == LayerMask.NameToLayer("Foreground")) {
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
        foreach(var param in anim.parameters) {
          anim.SetBool(param.name, false);
        }
        anim.SetBool("LiveWire", true);

        transform.localScale = SparkScale;
        OldColliderOffset = collider.offset;
        OldColliderSize = collider.size;
        collider.offset = Vector2.zero;
        collider.size = Vector2.one;

        usedHorizontalAirControl = false;
        HorizontalAxis = 0;
        VerticalAxis = 0;
      }
    }
    
    
    public override void Deactivate() {
      if (enabled) {
        base.Deactivate();
        anim.SetBool("LiveWire", false);

        collider.size = OldColliderSize;
        collider.offset = OldColliderOffset;

        transform.localScale = Vector2.one;
        collider.offset = OldColliderOffset;
        collider.size = OldColliderSize;

        usedHorizontalAirControl = false;
        HorizontalAxis = 0;
        VerticalAxis = 0;

        if (rb.velocity.y < 0) {
          rb.velocity = rb.velocity*Vector2.right + JumpForce;
        } else {
          rb.velocity += JumpForce;
        }
      }
    }
    
    #endregion Player Behavior API

    #region External Interface

    public void SetInitialVelocity(Vector2 velocity) {
      rb.velocity = velocity;
      anim.SetBool("IsFacingRight", rb.velocity.x > 0);
    }

    public override void SetDirection(Vector2 direction) {
      direction = direction.normalized;
      rb.velocity = direction*rb.velocity.magnitude;
    }
    #endregion
  }

}