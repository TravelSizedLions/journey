using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// A script that can be used during in-game cutscenes to change where the
  /// camera is focused.
  /// </summary>
  public class FocusCamera : MonoBehaviour {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    private TargettingCamera targettingCamera;

    /// <summary>
    /// How quickly to pan the camera to the target.
    /// </summary>
    [Tooltip("How quickly to pan the camera to the target. 0 - No panning, 1 - Instantaneous")]
    [Range(0, 1)]
    public float PanSpeed;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      targettingCamera = FindObjectOfType<TargettingCamera>();
    }
    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Switch where the camera is focused on to this game object's transform.
    /// </summary>
    public void Focus() {
      VirtualCamera camera = GetComponentInChildren<VirtualCamera>();
      camera.Activate(); 
    }

    /// <summary>
    /// Switch where the camera is focused by snapping to this game object's position.
    /// </summary>
    public void Snap() {
      VirtualCamera camera = GetComponentInChildren<VirtualCamera>();
      camera.Activate(); 
      targettingCamera.SnapToTarget();
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