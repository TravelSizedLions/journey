using Storm.Flexible;
using UnityEngine;

namespace Storm.Characters.Bosses {

  /// <summary>
  /// An eye
  /// </summary>
  public class PropEye : MonoBehaviour {
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
    protected Animator animator;


    /// <summary>
    /// A component for shaking the eye.
    /// </summary>
    protected Shaking shaking;


    /// <summary>
    /// Whether or not the eye is open.
    /// </summary>
    protected bool open;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected void Awake() {
      animator = GetComponent<Animator>();
      shaking = GetComponent<Shaking>();
    }
    #endregion


    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    public virtual void Open() {
      animator.SetBool("open", true);
      open = true;
    }

    public virtual void Close() {
      animator.SetBool("open", false);
      open = false;
    }
    #endregion
  }
}