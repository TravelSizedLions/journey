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
    /// Camera settings related to the pixel perfect camera.
    /// </summary>
    public PixelPerfectCamera PixelCam;


    
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

      if (PixelCam == null) {
        PixelCam = transform.GetComponentInChildren<PixelPerfectCamera>();
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
        cam.SetTarget(cameraSettings, PixelCam);
      }
    }

    /// <summary>
    /// Removes the target from the TargettingCamera
    /// </summary>
    public void Deactivate() {
      TargettingCamera.ClearTarget();
    }

  }

}