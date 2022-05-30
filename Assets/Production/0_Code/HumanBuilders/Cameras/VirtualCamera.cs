using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace HumanBuilders {

  /// <summary>
  /// A virtual camera used to frame sections of a level. 
  /// Makes the camera "stick" to certain locations if the player walks into the appropriate area.
  /// </summary>
  public class VirtualCamera : MonoBehaviour, ITriggerableParent {

    /// <summary>
    /// The camera settings that should be applied to the TargettingCamera when this vCam is activated.
    /// </summary>
    private Camera cameraSettings;


    /// <summary>
    /// A reference to the TargettingCamera.
    /// </summary>
    private TargettingCamera cam;

    /// <summary>
    /// The speed at which the camera pans
    /// </summary>
    [Range(0, 1)]
    public float PanSpeed = 0.33f;

    public float ZoomSpeed = 0.98f;
    
    //---------------------------------------------------------------------
    // Unity API
    //---------------------------------------------------------------------

    /// <summary>
    /// Grabs references to the vCam's settings and to the TargettingCamera
    /// </summary>
    private void Awake() {
      cameraSettings = transform.GetComponentInChildren<Camera>();

      if (cam == null) {
        cam = FindObjectOfType<TargettingCamera>();
      }
    }


    /// <summary>
    /// Activate the vCam if the player moves into it's trigger collider.
    /// </summary>
    /// <param name="col">The collider that's intersecting the vCam collider</param>
    public void PullTriggerEnter2D(Collider2D col) {
      if (col.gameObject.CompareTag("Player")) {
        if (TargettingCamera.target == null || 
            TargettingCamera.target.transform.position != transform.position) {
          Activate();
        }
      }
    }

    /// <summary>
    /// Activate the vCam the player is standing completely within over a partial collision.
    /// </summary>
    /// <param name="col">The collider that's intersecting the vCam collider</param>
    public void PullTriggerStay2D(Collider2D col) {
      if (col.gameObject.CompareTag("Player")) {

        if (TargettingCamera.target == null) {
          if (cam == null) {
            cam = FindObjectOfType<TargettingCamera>();
          }
          Debug.Log("Clearing target before set");
          TargettingCamera.ClearTarget();
        }

        if (TargettingCamera.target.transform.position != transform.position) {
          Activate();
        }
      }
    }


    /// <summary>
    /// Deactivate the vCam if the player leaves its trigger collider.
    /// </summary>
    /// <param name="col">The collider that's intersecting the vCam collider</param>
    public void PullTriggerExit2D(Collider2D col) {
      if (col.gameObject.CompareTag("Player")) {
        if (TargettingCamera.target == cameraSettings.transform) {

          // Don't reset if you're turning the vCam off.
          // TODO: this is breaking either the boss battle OR the start of
          // scenes the player's in.
          if (gameObject.activeInHierarchy) {
            if (!GameManager.Player.IsDead()) {
              ResetManager.Reset();
            }
          }
          
          if (!GameManager.Player.IsDead()) {
            Debug.Log("Deactivating");
            Deactivate();
          }
        }
      }
    }



    //---------------------------------------------------------------------
    // Public Interface
    //---------------------------------------------------------------------

    /// <summary>
    /// Makes this virtual camera the target of the TargettingCamera
    /// </summary>
    public void Activate() {
      if (cam == null) {
        cam = FindObjectOfType<TargettingCamera>();
      }

      if (cam != null) {
        cam.SetTarget(cameraSettings, PanSpeed, ZoomSpeed);
      }
    }

    /// <summary>
    /// Removes the target from the TargettingCamera
    /// </summary>
    public void Deactivate() {
      TargettingCamera.ClearTarget();
    }

    private void OnDrawGizmos() {
      Camera cam = transform.GetComponentInChildren<Camera>(true);
      Vector3 position = cam.transform.position;

      Color color = new Color(0.75f, .75f, 0.75f, 1f);
      CameraUtils.DrawCameraBox(position, cam.orthographicSize, cam.aspect, color, 5);
      CameraUtils.DrawCameraBox(position, cam.orthographicSize, 16f/9f, color, 3);
      CameraUtils.DrawCameraBox(position, cam.orthographicSize, 16f/10f, color, 2);
    }
  }
}