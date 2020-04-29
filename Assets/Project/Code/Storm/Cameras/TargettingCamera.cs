using System.Collections;
using System.Collections.Generic;
using Storm.Attributes;
using Storm.Characters.Player;
using Storm.Subsystems.Transitions;
using UnityEngine;

namespace Storm.Cameras {

  [RequireComponent(typeof(Camera))]
  public class TargettingCamera : MonoBehaviour {
    #region Offset Parameters
    [Header("Offsets", order = 0)]
    [Space(5, order = 1)]

    /// <summary>
    /// How much the camera should be offset from the player vertically.
    /// </summary>
    [Tooltip("How much the camera should be offset from the player vertically.")]
    public float VerticalOffset;

    /// <summary>
    /// How much the camera should be offset from the player horizontally.
    /// </summary>
    [Tooltip("How much the camera should be offset from the player horizontally.")]
    public float HorizontalOffset;

    /// <summary>
    /// The calculated offset vector when centering on a target.
    /// </summary>
    private Vector3 centeredOffset;

    /// <summary>
    /// The calculated offset vector when offsetting to the left of a target.
    /// </summary>
    private Vector3 leftOffset;

    /// <summary>
    /// The calculated offset vector when offsetting to the right of a target.
    /// </summary>
    private Vector3 rightOffset;


    /// <summary>
    /// Whether or not the camera should center on the target instead of offsetting.
    /// </summary>
    [Tooltip("Whether or not the camera should center on the target instead of offsetting.")]
    [SerializeField]
    [ReadOnly]
    private bool isCentered;

    [Space(15, order = 2)]
    #endregion


    #region Targetting Speed Parameters
    [Header("Targetting Speed", order = 3)]
    [Space(5, order = 4)]

    /// <summary>
    /// How quickly to pan the camera to a vCam target.
    /// </summary>
    [Tooltip("How quickly to pan the camera to a vCam target. 0 - No panning, 1 - Instantaneous")]
    [Range(0, 1)]
    public float VCamPanSpeed;

    /// <summary>
    /// How quickly to pan the camera to the player if they're the target.
    /// </summary>
    [Tooltip("How quickly to pan the camera to the player if they're the target. 0 - No panning, 1 - Instantaneous")]
    [Range(0, 1)]
    public float PlayerPanSpeed;

    /// <summary>
    /// How quickly to zoom the camera in and out.
    /// </summary>
    [Tooltip("How quickly to zoom the camera in and out. 0 - No panning, 1 - Instantaneous")]
    [Range(0, 1)]
    public float ZoomSpeed;

    #endregion


    #region Static Variables

    /// <summary>
    /// The transform that the Camera is actually tracking.
    /// </summary>
    public static Transform target;

    /// <summary>
    /// The name of a virtual camera to snap to at the beginning of a scene, if applicable.
    /// </summary>
    private static string virtualCameraName;

    /// <summary>
    /// A reference to the player
    /// </summary>
    public static PlayerCharacter player;

    /// <summary>
    /// The target's camera settings, such as orthographic size (zoom).
    /// </summary>
    private static Camera targetSettings;

    /// <summary>
    /// The camera settings that the targetting camera defaults to if not given a virtual camera to target.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private Camera defaultSettings;

    /// <summary>
    /// A reference parameter used by SmoothDamp in the FixedUpdate function
    /// </summary>
    private Vector3 velocity;

    #endregion

    //---------------------------------------------------------------------
    // Unity API
    //---------------------------------------------------------------------
    void Start() {
      Transform child = transform.GetChild(0);
      defaultSettings = child.gameObject.GetComponent<Camera>();
      /// defaultSettings = gameObject.GetComponentInChildren<Camera>(true);
      defaultSettings.enabled = false;

      // Calculate offset vectors.
      centeredOffset = new Vector3(0, 0, -10);
      leftOffset = new Vector3(-HorizontalOffset, VerticalOffset, -10);
      rightOffset = new Vector3(HorizontalOffset, VerticalOffset, -10);

      // Find the player if possible.
      if (player == null) {
        player = FindObjectOfType<PlayerCharacter>();
        if (player == null) return;
      }

      // Snap to a virtual camera at the start of the scene
      // if one is specified
      if (virtualCameraName != null && virtualCameraName != "") {

        // Try to find the correct virtual camera view and have the Trailing Camera snap to it.
        foreach (VirtualCamera cam in FindObjectsOfType<VirtualCamera>()) {
          if (cam.name == virtualCameraName) {
            // Make this virtual camera the active camera view.
            cam.Activate();
            SnapToTarget();
            return;
          }
        }

        throw new UnityException("Could not find Virtual Camera \"" + virtualCameraName + "\" in the current scene.");
      } else {
        ClearTarget();
        SnapToSpawn();
      }
    }


    /// <summary>
    /// Continually tracks the current target 
    /// This will either be the player, or a virtual camera if a player is within
    /// range of a virtual camera.
    /// </summary>
    void FixedUpdate() {
      if (target == null) {
        return;
      }

      if (player == null) {
        player = FindObjectOfType<PlayerCharacter>();
        if (player == null) return;
      }


      float futureSize = targetSettings.orthographicSize;
      float smoothing = (target == player.transform) ? PlayerPanSpeed : VCamPanSpeed;
      Vector3 futurePos = GetFuturePosition();

      // interpolate camera zoom
      Camera.main.orthographicSize = Interpolate(Camera.main.orthographicSize, targetSettings.orthographicSize, ZoomSpeed);

      // interpolate camera position
      transform.position = Vector3.SmoothDamp(transform.position, futurePos, ref velocity, smoothing);
    }



    /// <summary>
    /// Find the camera's future position based on the current target + offsets.
    /// </summary>
    /// <returns>The camera's true future position.</returns>
    private Vector3 GetFuturePosition() {
      Vector3 pos;

      // if following the player
      if (target == player.transform) {
        pos = player.transform.position;

        // choose appropriate camera offset.
        if (isCentered) {
          pos += centeredOffset;
        } else if (player.IsFacingRight) {
          pos += rightOffset;
        } else if (!player.IsFacingRight) {
          pos += leftOffset;
        }

        // moving to a predefined location
      } else {
        pos = target.transform.position + centeredOffset;
      }

      return pos;
    }

    /// <summary>
    /// Performs interpolation of floats.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="a"></param>
    /// <returns>The interpolation of x into y by percentage a.</returns>
    private float Interpolate(float x, float y, float a) {
      return x * a + y * (1 - a);
    }


    //---------------------------------------------------------------------
    // Public Interface
    //---------------------------------------------------------------------

    public void SetPlayerSmoothing(float smoothing) {
      PlayerPanSpeed = smoothing;
    }

    public void SetTargetSmoothing(float smoothing) {
      VCamPanSpeed = smoothing;
    }

    /// <summary>
    /// Sets a virtual camera as the target for when the next scene starts.
    /// </summary>
    /// <param name="vcamName">The name of the virtual camera in the editor hierarchy of the next scene.</param>
    public static void SetTarget(string vcamName) {
      virtualCameraName = vcamName;
    }

    /// <summary>
    /// Sets a virtual camera as a target.
    /// </summary>
    /// <param name="cameraSettings"></param>
    public void SetTarget(Camera cameraSettings) {
      targetSettings = cameraSettings;
      isCentered = true;
      target = cameraSettings.transform;
    }

    /// <summary>
    /// Resets the target back to the player.
    /// </summary>
    public void ClearTarget() {
      targetSettings = defaultSettings;
      target = player.transform;
      isCentered = false;
    }

    /// <summary>
    /// Snap the camera to the current target+offset immediately.
    /// </summary>
    public void SnapToTarget() {
      transform.position = target.position;
      if (target == player.transform) {
        if (player.IsFacingRight) {
          transform.position += rightOffset;
        } else {
          transform.position += leftOffset;
        }
      } else if (isCentered) {
        transform.position += centeredOffset;
      }
    }

    /// <summary>
    /// Snap the camera to the game's current spawn location.
    /// </summary>
    public void SnapToSpawn() {
      Vector3 pos = TransitionManager.Instance.GetCurrentSpawnPosition();
      bool isFacingRight = TransitionManager.Instance.GetCurrentSpawnFacing();
      transform.position = pos + (isFacingRight ? rightOffset : leftOffset);
    }

    /// <summary>
    /// Set whether or not the camera should center on its current target.
    /// </summary>
    public void SetCentered(bool centered) {
      isCentered = centered;
    }
  }

}