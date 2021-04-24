
using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// An eye that can be opened and closed
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
    /// The gameobject containing the lids of the prop eyes
    /// </summary>
    public GameObject Lids;


    /// <summary>
    /// Whether or not the eye is open.
    /// </summary>
    protected bool open;
    #endregion


    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    public virtual void Open() {
      if (Lids != null) {
        Lids.SetActive(false);
      }

      open = true;
    }

    public virtual void Close() {
      if (Lids != null) {
        Lids.SetActive(true);
      }

      open = false; 
    }
    #endregion
  }
}