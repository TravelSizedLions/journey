using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// A script that can be used during in-game cutscenes to change where the
  /// camera is focused.
  /// </summary>
  public class FocusCamera : MonoBehaviour {

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    private TargettingCamera targettingCamera;

    [Tooltip("The virtual camera view to focus in on.")]
    public VirtualCamera vCam;

    [Tooltip("How quickly to pan the camera to the target. 0 - No panning, 1 - Instantaneous")]
    [Range(0, 1)]
    public float PanSpeed;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      targettingCamera = FindObjectOfType<TargettingCamera>();
    }

    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    public void Focus() {
      vCam.Activate(); 
    }

    public void Snap() {
      vCam.Activate(); 
      targettingCamera.SnapToTarget();
    }

    public void Unfocus() {
      TargettingCamera.ClearTarget();
    }
  }
}