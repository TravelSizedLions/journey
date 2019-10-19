using UnityEngine;

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
    
    [Tooltip("How much control the player has over Jerrod's ascent/descent. Higher means more control.")]
    /// <summary>How much control the player has over Jerrod's ascent/descent. Higher means more control.</summary>
    public float verticalAirControl;
    
    [Tooltip("How much control the player has over Jerrod's mid air left/right movement. Higher means more control.")]
    /// <summary>How much control the player has over Jerrod's mid air left/right movement. Higher means more control.</summary>
    public float horizontalAirControl;


    /// <summary>
    /// Sets how big of a jump the player performs upon exiting LiveWire mode.
    /// </summary>
    [Tooltip("Sets how big of a jump the player performs upon exiting LiveWire mode.")]
    public float postTransitJump;

    /// <summary>
    /// The jump force vector calculated from the jump variable.
    /// </summary>
    protected Vector2 jumpForce;

    /// <summary>
    /// The size of Jerrod's LiveWire Spark.
    /// </summary>
    public float sparkSize;

    private Vector2 sparkScale;
    private Vector2 oldColliderSize;
    private Vector2 oldColliderOffset;

    #endregion Air Control Parameters
    
    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    public override void Awake() {
      base.Awake();
      sparkScale = new Vector2(sparkSize, sparkSize);
    }
    
    
    public void Start() {
      oldColliderSize = collider.size;
      oldColliderOffset = collider.offset;

      jumpForce = new Vector2(0, postTransitJump);
    }
    
    
    public void Update() {
      if (Input.GetKeyDown(KeyCode.Space)) {
        player.SwitchBehavior(PlayerBehaviorEnum.Normal);
        player.normalMovement.hasJumped = true;
        player.normalMovement.hasDoubleJumped = true;
      }
    }
    
    public void FixedUpdate() {
    
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

        transform.localScale = sparkScale;
        oldColliderOffset = collider.offset;
        oldColliderSize = collider.size;
        collider.offset = Vector2.zero;
        collider.size = Vector2.one*2.4f;
      }
    }
    
    
    public override void Deactivate() {
      if (enabled) {
        base.Deactivate();
        anim.SetBool("LiveWire", false);

        collider.size = oldColliderSize;
        collider.offset = oldColliderOffset;

        // Retain horizontal velocity, but cancel gravity for jump.
        if (!sensor.isTouchingLeftWall() && !sensor.isTouchingRightWall()) {
          rb.velocity = rb.velocity*Vector2.right + jumpForce;
        }

        transform.localScale = Vector2.one;
        collider.offset = oldColliderOffset;
        collider.size = oldColliderSize;
      }
    }
    
    
    public void SetInitialVelocity(Vector2 velocity) {
      rb.velocity = velocity;
    }
    #endregion PlayerMovement API
  }

}