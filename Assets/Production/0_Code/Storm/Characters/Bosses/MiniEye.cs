using Storm.Flexible;
using Storm.Flexible.Interaction;
using UnityEngine;

namespace Storm.Characters.Bosses {
  /// <summary>
  /// A boss eye. 
  /// </summary>
  [RequireComponent(typeof(Shaking))]
  public class MiniEye : TriggerableParent {

    #region Delegates
    //-------------------------------------------------------------------------
    // Delegates
    //-------------------------------------------------------------------------

    /// <summary>
    /// Call back signature type.
    /// </summary>
    public delegate void OnEyeCloseCallback();

    /// <summary>
    /// Event fires when the eye closes;
    /// </summary>
    public event OnEyeCloseCallback OnEyeClose;
  
    #endregion

    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------

    /// <summary>
    /// Whether or no the eye is open.
    /// </summary>
    public bool IsOpen { get {return open; } }
    #endregion

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// A reference to the animator on this object.
    /// </summary>
    private Animator animator;


    /// <summary>
    /// A component for shaking the eye.
    /// </summary>
    private Shaking shaking;


    /// <summary>
    /// Whether or not the eye is open.
    /// </summary>
    private bool open;

    /// <summary>
    /// The dangerous area of the eye that's enabled while the eye is closed.
    /// </summary>
    private Deadly damageArea;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      animator = GetComponent<Animator>();
      shaking = GetComponent<Shaking>();
      damageArea = GetComponentInChildren<Deadly>();
      DisableDamageArea();
    }
    #endregion

    #region Triggerable Parent API
    public override void PullTriggerEnter2D(Collider2D col) {
      Carriable carriable = col.GetComponent<Carriable>();
      if (carriable != null && col == carriable.Collider && open) {
        carriable.Physics.Velocity = Vector2.zero;
        TakeDamage();
        if (OnEyeClose != null) {
          OnEyeClose.Invoke();
        }
      }
    }
    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    public void Open() {
      animator.SetBool("open", true);
      open = true;
      EnableDamageArea();
    }

    public void Close() {
      animator.SetBool("open", false);
      open = false;
      DisableDamageArea();
    }

    public void TakeDamage() {
      animator.SetTrigger("damage");
      shaking.Shake();
      Close();
    }
    #endregion

    #region Helper Methods
    private void EnableDamageArea() {
      damageArea.enabled = true;
      damageArea.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void DisableDamageArea() {
      damageArea.enabled = false;
      damageArea.GetComponent<SpriteRenderer>().enabled = false;
    }

    #endregion
  }
}

