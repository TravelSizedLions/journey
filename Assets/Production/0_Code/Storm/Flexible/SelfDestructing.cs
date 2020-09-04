using System.Collections;
using System.Collections.Generic;
using Storm.Subsystems.Save;
using Storm.Subsystems.Transitions;
using UnityEngine;

namespace Storm.Flexible {

  /// <summary>
  /// A behavior that allows a game object to be destroyed on command. This is useful when a game object needs to be destroyed through an event trigger.
  /// </summary>
  [RequireComponent(typeof(GuidComponent))]
  public class SelfDestructing : MonoBehaviour {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// How long to wait before self destructing (in seconds).
    /// </summary>
    [Tooltip("How long to wait before self destructing (in seconds).")]
    public float Delay = 0f;

    /// <summary>
    /// The unique ID for this game object.
    /// </summary>
    private string guid;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      GuidComponent guidComponent = gameObject.GetComponent<GuidComponent>();

      if (guidComponent != null) {
        guid = guidComponent.GetGuid().ToString();
        if (VSave.Get(StaticFolders.DESTRUCTIBLE, guid+Keys.KEEP_DESTROYED, out bool keepDestroyed)) {
          Destroy(this.gameObject, Delay);
        }
      } else {
        Debug.LogWarning("SelfDestructing object \"" + gameObject.name + "\" needs a GuidComponent!");
      }
    }
    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Destroy this object.
    /// </summary>
    public void SelfDestruct() {
      Destroy(this.gameObject, Delay);
    }

    /// <summary>
    /// Mark this object as permanently destroyed. This means the object won't
    /// respawn when you return to the scene.
    /// </summary>
    public void KeepDestroyed() {
      VSave.Set(StaticFolders.DESTRUCTIBLE, guid+Keys.KEEP_DESTROYED, true);
    }
    #endregion
  }
}