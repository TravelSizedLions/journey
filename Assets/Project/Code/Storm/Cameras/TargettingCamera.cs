using Storm.Attributes;
using Storm.Characters.Player;
using Storm.Characters;
using Storm.Subsystems.Transitions;
using UnityEngine;
using System;

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
    /// The threshold at which the camera activates following its target.
    /// </summary>
    [Tooltip("The threshold at which the camera activates following its target.")]
    public Vector2 ActivateThreshold;

    /// <summary>
    /// The threshold at which the camera deactivates following its target.
    /// </summary>
    [Tooltip("The threshold at which the camera activates following its target.")]
    public Vector2 DeactivateThreshold;


    private bool activeX;

    private bool activeY;

    private Vector2 lastActivePosition;


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

    public Vector2 CameraTrap;


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

    #region New Interpolation Params

    private Vector3 lastTarget;

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

      if (IsActive()) {
        float smoothing = (target == player.transform) ? PlayerPanSpeed : VCamPanSpeed;
        Vector3 futurePos = GetFuturePosition();

        // interpolate camera position
        Vector3 newPosition = Vector3.SmoothDamp(transform.position, futurePos, ref velocity, smoothing);

        transform.position = new Vector3(
          (activeX) ? newPosition.x : transform.position.x,
          (activeY) ? newPosition.y : transform.position.y,
          newPosition.z
        );

        TrapTarget();
      } 
      
      Camera.main.orthographicSize = Interpolate(Camera.main.orthographicSize, targetSettings.orthographicSize, ZoomSpeed);

      //InterpCameraSize();
    }


    private void TrapTarget() {
      if (target.transform == player.transform) {
        Bounds bounds = GetCameraBounds();
        Debug.Log("//------------------------------------");
        Debug.Log("Bounds: " + bounds);
        Debug.Log("Player: " + player.transform.position);
        Debug.Log("BEFORE TRAP: "+transform.position);

        // Keep player within horizontal bounds.
        if (target.transform.position.x >= (transform.position.x + bounds.extents.x)) {
          Debug.Log("A");
          float diff = target.transform.position.x - (transform.position.x + bounds.extents.x);
          transform.position = new Vector3(transform.position.x + diff, transform.position.y, transform.position.z);
        } else if (target.transform.position.x <= (transform.position.x - bounds.extents.x)) {
          Debug.Log("B");
          float diff = (transform.position.x - bounds.extents.x) - target.transform.position.x;
          transform.position = new Vector3(transform.position.x - diff, transform.position.y, transform.position.z);
        }

        // Keep player within vertical bounds.
        if (target.transform.position.y >= (transform.position.y + bounds.extents.y)) {
          Debug.Log("C");
          float diff = target.transform.position.y - (transform.position.y + bounds.extents.y);
          transform.position = new Vector3(transform.position.x, transform.position.y + diff, transform.position.z);
        } else if (target.transform.position.y <= (transform.position.y - bounds.extents.y)) {
          Debug.Log("D");
          float diff = (transform.position.y - bounds.extents.y) - target.transform.position.y;
          transform.position = new Vector3(transform.position.x, transform.position.y - diff, transform.position.z);
        }
        Debug.Log("AFTER TRAP: "+transform.position);
      }
    }


    private Bounds GetCameraBounds() {
      float hExtent = Camera.main.orthographicSize*(((float)Screen.width)/((float)Screen.height));
      float vExtent = Camera.main.orthographicSize;

      Bounds bounds = new Bounds(
        transform.position, 
        new Vector3(
          ((hExtent*2)-(CameraTrap.x)),
          ((vExtent*2)-(CameraTrap.y))
        )
      );

      return bounds;
    }


    private Vector2 GetDistanceToTarget() {
      // Debug.Log("cam position: " + transform.position);
      // Debug.Log("target position: " + target.transform.position);

      Vector3 dist = transform.position;

      if (target == player.transform) {
        if (player.Facing == Facing.None && !player.IsWallJumping()) {
          dist -= centeredOffset;
        } else if (player.Facing == Facing.Right) {
          dist -= rightOffset;
        } else if (player.Facing == Facing.Left) {
          dist -= leftOffset;
        }
      }

      // Debug.Log("after correction: " + dist);

      dist -= target.transform.position;
      
      // Debug.Log("difference: " + dist);

      dist = new Vector2(
        Mathf.Abs(dist.x), 
        Mathf.Abs(dist.y)
      );

      //Debug.Log("Distance After: " + dist);

      return dist;
    }

    public bool IsActive() {
      
      if (target.transform == player.transform) {
        Vector2 distance = GetDistanceToTarget();
        Vector2 delta = GetTargetDelta();

        if (distance.x < DeactivateThreshold.x) {
          activeX = false;
          lastActivePosition.x = target.position.x;
        } else if (delta.x > ActivateThreshold.x) {
          activeX = true;
        } 

        if (distance.y < DeactivateThreshold.y) {
          activeY = false;
          lastActivePosition.y = target.position.y;
        } else if (delta.y > ActivateThreshold.y) {
          activeY = true;
        }

        //Debug.Log("("+activeX+", "+activeY+")");
        return activeX || activeY;
      } else {
        activeX = true;
        activeY = true;
        return true;
      }

    }

    public Vector2 GetTargetDelta() {
      Vector2 delta = lastActivePosition - (Vector2)target.transform.position;
      delta = new Vector2(Mathf.Abs(delta.x), Mathf.Abs(delta.y));
      return delta;
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
        if (player.Facing == Facing.None && !player.IsWallJumping()) {
          pos += centeredOffset;
        } else if (player.Facing == Facing.Right) {
          pos += rightOffset;
        } else if (player.Facing == Facing.Left) {
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
      return (x * a) + (y * (1 - a));
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
      if (targetSettings != cameraSettings) {
        ResetTracking(false, false);
        targetSettings = cameraSettings;
        isCentered = true;
        target = cameraSettings.transform;
      }
    }

    /// <summary>
    /// Resets the target back to the player.
    /// </summary>
    public void ClearTarget() {
        targetSettings = defaultSettings;
        target = player.transform;
        isCentered = false;
        ResetTracking(true, true);
    }

    /// <summary>
    /// Snap the camera to the current target+offset immediately.
    /// </summary>
    public void SnapToTarget() {
      transform.position = target.position;
      if (target == player.transform) {
        if (player.Facing == Facing.Right) {
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

    public void ResetLastPosition() {
      lastActivePosition.x = target.transform.position.x;
      lastActivePosition.y = target.transform.position.y;
    }


    public void ResetTracking(bool x, bool y) {
      if (!x) {
        lastActivePosition.x = target.transform.position.x;
      }

      if (!y) {
        lastActivePosition.y = target.transform.position.y;
      }


      activeX = x;
      activeY = y;
    }
  }

}