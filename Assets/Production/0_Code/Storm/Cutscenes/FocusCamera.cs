using System.Collections;
using System.Collections.Generic;
using Storm.Cameras;
using UnityEngine;

namespace Storm.Cutscenes {

  /// <summary>
  /// A script that can be used during in-game cutscenes to change where the
  /// camera is focused.
  /// </summary>
  public class FocusCamera : MonoBehaviour {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// How quickly to pan the camera to the target.
    /// </summary>
    [Tooltip("How quickly to pan the camera to the target. 0 - No panning, 1 - Instantaneous")]
    [Range(0, 1)]
    public float PanSpeed;
    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Switch where the camera is focused on to this game object's transform.
    /// </summary>
    public void Focus() {
      Debug.Log("Focusing!");
      VirtualCamera camera = GetComponentInChildren<VirtualCamera>();
      camera.Activate(); 
    }

    /// <summary>
    /// Reset focus back on the player.
    /// </summary>
    public void Unfocus() {
      TargettingCamera.ClearTarget();
    }
    #endregion

  }
}