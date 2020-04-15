using System.Collections;
using System.Collections.Generic;
using Storm.UI;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// The player behavior for Non-ballistic livewire movement (being fired and redirected in straight lines).
  /// </summary>
  public class DirectedLiveWireMovement : LiveWireMovement {

    #region Movement Parameters
    [Header("Movement Parameters", order = 0)]
    [Space(5, order = 1)]

    [Tooltip("How fast the player zips from point to point.")]
    public float TopSpeed = 125f;

    /// <summary>
    /// Calculated squared velocity from topSpeed.
    /// </summary>
    protected float maxSqrVelocity;

    /// <summary>
    /// How fast the player reaches top speed. 0 - no movement, 1 - instantaneous top speed
    /// </summary>
    [Tooltip("How fast the player reaches top speed. 0 - no movement, 1 - instantaneous top speed")]
    [Range(0, 1)]
    public float Acceleration = 0.2f;

    /// <summary>
    /// The target player velocity. Calculated from direction and topSpeed.
    /// </summary>
    private Vector2 targetVelocity;

    /// <summary>
    /// Sets how big of a jump the player performs upon exiting LiveWire mode.
    /// </summary>
    [Tooltip("Sets how big of a jump the player performs upon exiting LiveWire mode.")]
    public float PostTransitJump = 24f;

    /// <summary>
    /// The jump force vector calculated from the jump variable.
    /// </summary>
    protected Vector2 jumpForce;

    [Space(15, order = 2)]
    #endregion

    #region Appearance Parameters
    [Header("Appearance Parameters", order = 3)]
    [Space(5, order = 4)]

    /// <summary>
    /// How much the spark stretches. Higher = more stretch.
    /// </summary>
    [Tooltip("How much the spark stretches. Higher = more stretch.")]
    public float Stretch = 3f;

    /// <summary>
    /// How thick the spark should be.
    /// </summary>
    [Tooltip("How thick the spark should be.")]
    public float Thickness = 0.2f;

    /// <summary>
    /// The target scale calculated from stretch and thickness.
    /// </summary>
    private Vector3 scale;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------//
    // Unity API
    //-------------------------------------------------------------------//
    protected override void Awake() {
      base.Awake();
    }

    private void Start() {
      jumpForce = new Vector2(0, PostTransitJump);
      maxSqrVelocity = TopSpeed * TopSpeed;
      rb.freezeRotation = true;
      scale = new Vector3(Stretch, Thickness, 1);
    }


    private void Update() {
      transform.localScale = Vector3.Lerp(transform.localScale, scale, Acceleration);
    }


    private void FixedUpdate() {
      rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, Acceleration);
    }

    #endregion

    #region Player Behavior API
    //-------------------------------------------------------------------//
    // Player Behavior API
    //-------------------------------------------------------------------//

    /// <summary>
    /// Called every time the player switches to Directed LiveWire Movement.
    /// </summary>
    public override void Activate() {
      if (!enabled) {
        base.Activate();
        if (rb == null) {
          rb = GetComponent<Rigidbody2D>();
        }
        rb.velocity = Vector2.zero;
        foreach (var param in anim.parameters) {
          anim.SetBool(param.name, false);
        }
        rb.gravityScale = 0;
        gameObject.layer = LayerMask.NameToLayer("LiveWire");
        anim.SetBool("LiveWire", true);
      }
    }

    /// <summary>
    /// Called every time the player switches away from Directed LiveWire Movement.
    /// </summary>
    public override void Deactivate() {
      if (enabled) {
        base.Deactivate();
        anim.SetBool("LiveWire", false);
        //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        rb.velocity = jumpForce;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        rb.gravityScale = 1;
        gameObject.layer = LayerMask.NameToLayer("Player");
      }

    }
    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Sets the direction the player will travel towards.
    /// </summary>
    /// <param name="direction">The direction to make the player travel in. Does not need to be normalized.</param>
    public override void SetDirection(Vector2 direction) {
      rb.velocity = Vector2.zero;
      targetVelocity = direction.normalized * TopSpeed;
      transform.rotation = Quaternion.identity;
      transform.localScale = new Vector3(1, Thickness, 1);

      transform.Rotate(0, 0, Vector2.SignedAngle(Vector2.right, direction));
    }
    #endregion
  }
}