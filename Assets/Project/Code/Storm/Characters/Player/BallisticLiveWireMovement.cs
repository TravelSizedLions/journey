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
  public class BallisticLiveWireMovement : PlayerBehavior {
    
    #region Air Control Parameters
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    [Header("Air Control", order=0)]
    [Space(5, order=1)]

    // TODO: Implement if there's time.
    
    // [Tooltip("How much control the player has over Jerrod's ascent/descent. Higher means more control.")]
    // /// <summary>How much control the player has over Jerrod's ascent/descent. Higher means more control.</summary>
    // public float verticalAirControl;
    
    // [Tooltip("How much control the player has over Jerrod's mid air left/right movement. Higher means more control.")]
    // /// <summary>How much control the player has over Jerrod's mid air left/right movement. Higher means more control.</summary>
    // public float horizontalAirControl;


    /// <summary>
    /// Sets how big of a jump the player performs upon exiting LiveWire mode.
    /// </summary>
    [Tooltip("Sets how big of a jump the player performs upon exiting BallisticLiveWire mode.")]
    public float PostTransitJump;

    /// <summary>
    /// How much the player can nudge the ballistic curve left or right.
    /// </summary>
    public float HorizontalAirControl;

    /// <summary>
    /// How much the player can nudge the ballistic curve up or down.
    /// </summary>
    public float VerticalAirControl;

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

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    public override void Awake() {
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
        rb.velocity = rb.velocity*Vector2.right + JumpForce;
      }
    }


    public void GatherInputs() {
      SpacePressed = Input.GetKeyDown(KeyCode.Space);
      HorizontalAxis = Input.GetAxis("Horizontal");
      VerticalAxis = Input.GetAxis("Vertical");
    }
    
    public void FixedUpdate() {

      //float hInputDirection = Mathf.Sign(HorizontalAxis);
      //float vInputDirection = Mathf.Sign(VerticalAxis);
      
      Vector2 nudge = new Vector2(HorizontalAxis*HorizontalAirControl,VerticalAxis*VerticalAirControl);

      Debug.Log("nudge: "+nudge);
      // Nudge the player the right direction.
      rb.velocity += nudge;
    }


    public void OnCollisionEnter2D(Collision2D collision) {
      Debug.Log("Collision!");
      if (collision.gameObject.layer == LayerMask.NameToLayer("Foreground")) {
        player.SwitchBehavior(PlayerBehaviorEnum.Normal);
      }
    }

    #endregion Unity API
    
    
    
    #region PlayerMovement API
    //-------------------------------------------------------------------------
    // PlayerMovement API
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
      }
    }
    
    
    public void SetInitialVelocity(Vector2 velocity) {
      rb.velocity = velocity;
      anim.SetBool("IsFacingRight", rb.velocity.x > 0);
    }
    #endregion PlayerMovement API
  }

}