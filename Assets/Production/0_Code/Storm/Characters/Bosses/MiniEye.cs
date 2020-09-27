using Storm.Flexible;
using Storm.Flexible.Interaction;
using UnityEngine;

namespace Storm.Characters.Bosses {
  /// <summary>
  /// A boss eye. 
  /// </summary>
  [RequireComponent(typeof(Shaking))]
  public class MiniEye : Eye, ITriggerableParent {

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
  
    /// <summary>
    /// A reference to the main eye.
    /// </summary>
    public MegaEye megaEye;
    #endregion


    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The dangerous area of the eye that's enabled while the eye is closed.
    /// </summary>
    private Deadly damageArea;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected new void Awake() {
      base.Awake();
      damageArea = GetComponentInChildren<Deadly>();
      megaEye = FindObjectOfType<MegaEye>();
      DisableDamageArea();
    }
    #endregion

    #region Triggerable Parent API
    public void PullTriggerEnter2D(Collider2D col) {
      Carriable carriable = col.transform.root.GetComponentInChildren<Carriable>();
      if (carriable != null && col == carriable.Collider && open) {
        carriable.Physics.Velocity = Vector2.zero;
        TakeDamage();
        if (OnEyeClose != null) {
          OnEyeClose.Invoke();
        }
      }
    }

    public void PullTriggerStay2D(Collider2D col) {}
    public void PullTriggerExit2D(Collider2D col) {}
    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    public void TakeDamage() {
      animator.SetTrigger("damage");
      shaking.Shake();
      Close();
      megaEye.UseSpeechBubble(this);
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

