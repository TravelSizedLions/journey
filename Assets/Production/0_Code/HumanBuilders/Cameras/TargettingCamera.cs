using System.Collections;



using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering.Universal;
using Sirenix.OdinInspector;

namespace HumanBuilders {

  //[RequireComponent(typeof(Camera))]
  public class TargettingCamera : MonoBehaviour {
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
    /// The area within the camera the player should always stay in.
    /// </summary>
    [Tooltip("The area within the camera the player should always stay in.")]
    public Vector2 CameraTrap;

    [Space(15, order = 2)]

    [Header("Targetting Speed", order = 3)]
    [Space(5, order = 4)]

    /// <summary>
    /// How quickly to pan the camera to a vCam target.
    /// </summary>
    [Tooltip("How quickly to pan the camera to a vCam target. 0 - Instantaneous, 1 - No Panning")]
    [Range(0, 1)]
    public float VCamPanSpeed;

    /// <summary>
    /// How quickly to pan the camera to the player if they're the target.
    /// </summary>
    [Tooltip("How quickly to pan the camera to the player if they're the target. 0 - Instantaneous, 1 - No Panning")]
    [Range(0, 1)]
    public float PlayerPanSpeed;

    /// <summary>
    /// How quickly you transition from one pan speed to another.
    /// </summary>
    [Tooltip("How quickly you transition from one pan speed to another.")]
    [Range(0, 1)]
    public float PanSpeedTransitionInterp = 0.5f;

    /// <summary>
    /// An override setting for panning speed.
    /// </summary>
    public float OverridePanSpeed;

    /// <summary>
    /// How quickly to zoom the camera in and out.
    /// </summary>
    [Tooltip("How quickly to zoom the camera in and out. 0 - No panning, 1 - Instantaneous")]
    public float ZoomSpeed = 0.98f;


    [Space(10, order=5)]
    [Header("Debug Information", order=6)]

    /// <summary>
    /// Whether or not the camera should center on the target instead of offsetting.
    /// </summary>
    [Tooltip("Whether or not the camera should center on the target instead of offsetting.")]
    [SerializeField]
    [ReadOnly]
    private static bool isCentered;

    /// <summary>
    /// Whether or not the camera should be moving in the X direction.
    /// </summary>
    [Tooltip("Whether or not the camera should be moving in the X direction.")]
    [SerializeField]
    [ReadOnly]
    private static bool activeX;

    /// <summary>
    /// Whether or not the camera should be moving in the Y direction.
    /// </summary>
    [Tooltip("Whether or not the camera should be moving in the Y direction.")]
    [SerializeField]
    [ReadOnly]
    private static bool activeY;

    /// <summary>
    /// The last position the camera was at when it started moving.
    /// </summary>
    [Tooltip("The last position the camera was at when it started moving.")]
    [SerializeField]
    [ReadOnly]
    private static Vector2 lastActivePosition;

    /// <summary>
    /// The position the camera would be in if not locked to pixel coordinates.
    /// </summary>
    [Tooltip("The position the camera would be in if not locked to pixel coordinates.")]
    [SerializeField]
    [ReadOnly]
    private Vector3 virtualPosition;

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
    private static Camera defaultSettings;

    /// <summary>
    /// A reference parameter used by SmoothDamp in the FixedUpdate function
    /// </summary>
    private Vector3 velocity;

    /// <summary>
    /// The previous position of the target.
    /// </summary>
    private Vector3 prevTargetPosition;

    /// <summary>
    /// PixelPerfect related settings.
    /// </summary>
    public PixelPerfectCamera PixelPerfectCameraSettings;

    /// <summary>
    /// The actual settings for the camera. 
    /// </summary>
    public Camera CameraSettings;

    /// <summary>
    /// The current panning speed. Used to interpolate between to pan speeds to
    /// prevent whiplash.
    /// </summary>
    private float currentPanSpeed;

    // The camera's collider
    private BoxCollider2D col;

    //---------------------------------------------------------------------
    // Unity API
    //---------------------------------------------------------------------
    private void Start() {
      // Update the game's current camera.
      GameManager.CurrentTargettingCamera = this;
      GameManager.CurrentCamera = GetComponent<Camera>();
      CameraSettings = GameManager.CurrentCamera;

      Transform child = transform.GetChild(0);
      defaultSettings = child.gameObject.GetComponent<Camera>();
      defaultSettings.enabled = false;

      SceneManager.sceneLoaded += OnNewScene;

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
          if (cam.name == virtualCameraName && cam.gameObject.scene == SceneManager.GetActiveScene()) {
            // Make this virtual camera the active camera view.
            cam.Activate();
            SnapToTarget();
            return;
          }
        }

        ClearTarget();
      } else {
        ClearTarget();
        SnapToSpawn();
      }

      PixelPerfectCameraSettings = GetComponent<PixelPerfectCamera>();

      // It all starts here, baby.
      virtualPosition = transform.position;
      currentPanSpeed = 0;
      ZoomSpeed = 1-ZoomSpeed;
    }

    /// <summary>
    /// Removes the scene load callback delegate from the scene manager.
    /// </summary>
    private void Destroy() {
      SceneManager.sceneLoaded -= OnNewScene;
    }

    /// <summary>
    /// Continually tracks the current target 
    /// This will either be the player, or a virtual camera if a player is within
    /// range of a virtual camera.
    /// </summary>
    private void FixedUpdate() {
      if (player == null) {
        player = FindObjectOfType<PlayerCharacter>();
        if (player == null) return;
      }

      if (target == null) {
        if(!player.IsDead()) {
          ClearTarget();
        } else {
         return;
        }
      }

      if (IsActive()) {
        float targetPanSpeed = (target == player.transform) ? PlayerPanSpeed : VCamPanSpeed;
        if (currentPanSpeed < targetPanSpeed) {
          currentPanSpeed = (targetPanSpeed * (1-PanSpeedTransitionInterp)) + (currentPanSpeed * PanSpeedTransitionInterp);
        } else {
          currentPanSpeed = (targetPanSpeed * PanSpeedTransitionInterp) + (currentPanSpeed * (1-PanSpeedTransitionInterp));
        }
        Vector3 futurePos = GetFuturePosition();

        // interpolate camera position
        Vector3 newPosition = Vector3.SmoothDamp(virtualPosition, futurePos, ref velocity, currentPanSpeed);

        Vector3 raw = new Vector3(
          (activeX) ? newPosition.x : virtualPosition.x,
          (activeY) ? newPosition.y : virtualPosition.y,
          newPosition.z
        );

        Vector3 trapped = TrapTarget(raw);

        // Vector3 pixelTruncated = Pixels.ToPixel(trapped);

        CameraSettings.orthographicSize = Mathf.Lerp(CameraSettings.orthographicSize, targetSettings.orthographicSize, ZoomSpeed);

        virtualPosition = trapped;
        transform.position = trapped;
        prevTargetPosition = target.position;
      }
    }

    private Vector3 TrapTarget(Vector3 position) {
      if (target.transform == player.transform) {
        Bounds bounds = GetCameraBounds();

        // Keep player within horizontal bounds.
        if (target.transform.position.x >= (position.x + bounds.extents.x)) {
          float diff = target.transform.position.x - (position.x + bounds.extents.x);
          position = new Vector3(position.x + diff, position.y, position.z);
        } else if (target.transform.position.x <= (position.x - bounds.extents.x)) {
          float diff = (position.x - bounds.extents.x) - target.transform.position.x;
          position = new Vector3(position.x - diff, position.y, position.z);
        }

        // Keep player within vertical bounds.
        if (target.transform.position.y >= (position.y + bounds.extents.y)) {
          float diff = target.transform.position.y - (position.y + bounds.extents.y);
          position = new Vector3(position.x, position.y + diff, position.z);
        } else if (target.transform.position.y <= (position.y - bounds.extents.y)) {
          float diff = (position.y - bounds.extents.y) - target.transform.position.y;
          position = new Vector3(position.x, position.y - diff, position.z);
        }
      }

      return position;
    }


    private Bounds GetCameraBounds() {
      float hExtent = Camera.main.orthographicSize * (((float) Screen.width) / ((float) Screen.height));
      float vExtent = Camera.main.orthographicSize;

      Bounds bounds = new Bounds(
        transform.position,
        new Vector3(
          ((hExtent * 2) - (CameraTrap.x)),
          ((vExtent * 2) - (CameraTrap.y))
        )
      );

      return bounds;
    }


    /// <summary>
    /// Get the camera's distance to the target
    /// </summary>
    /// <returns></returns>
    private Vector2 GetDistanceToTarget() {

      Vector3 camPosition = virtualPosition;
      Vector3 targetPosition = target.position;
      if (target == player.transform) {

        if (player.Facing == Facing.None && !player.IsWallJumping()) {
          targetPosition += centeredOffset;
        } else if (player.Facing == Facing.Right) {
          targetPosition += rightOffset;
        } else if (player.Facing == Facing.Left) {
          targetPosition += leftOffset;
        }
      } 

      Vector3 dist = camPosition - targetPosition;

      dist = new Vector2(
        Mathf.Abs(dist.x),
        Mathf.Abs(dist.y)
      );

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

        return activeX || activeY;
      } else {
        activeX = true;
        activeY = true;
        return true;
      }

    }

    /// <summary>
    /// How much the target has moved from the position it was at when the
    /// camera was last active.
    /// </summary>
    private Vector2 GetTargetDelta() {
      Vector2 delta = lastActivePosition - (Vector2) target.transform.position;
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
    /// <param name="cameraSettings">The virtual camera to target.</param>
    /// <param name="panSpeed">The speed at which you
    public void SetTarget(Camera cameraSettings, float panSpeed) {
      if (targetSettings != cameraSettings) {
        ResetTracking(false, false);

        targetSettings = cameraSettings;
        isCentered = true;
        target = cameraSettings.transform;
        VCamPanSpeed = panSpeed;
      }
    }

    /// <summary>
    /// Resets the target back to the player.
    /// </summary>
    public static void ClearTarget() {      
      targetSettings = defaultSettings;
      target = GameManager.Player?.transform;
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
      ResetTracking(false, false);
    }

    /// <summary>
    /// Snap the camera to the game's current spawn location.
    /// </summary>
    public void SnapToSpawn() {
      Vector3 pos = TransitionManager.GetCurrentSpawnPosition();
      bool isFacingRight = TransitionManager.GetCurrentSpawnFacing();
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


    public static void ResetTracking(bool x, bool y) {
      if (!x && target != null) {
        lastActivePosition.x = target.transform.position.x;
      }

      if (!y && target != null) {
        lastActivePosition.y = target.transform.position.y;
      }


      activeX = x;
      activeY = y;
    }


    private void OnNewScene(Scene aScene, LoadSceneMode aMode) {
      player = FindObjectOfType<PlayerCharacter>();
      ClearTarget();
    }

    /// <summary>
    /// Shake the camera for a period of time.
    /// </summary>
    /// <param name="duration">How long the camera should shake.</param>
    /// <param name="delay">How long the camera should wait before shaking.</param>
    /// <param name="intensity">How much the camera should shake.</param>
    public void CameraShake(float duration, float delay, float intensity) {
      StartCoroutine(_CameraShake(duration, delay, intensity));
    }

    public IEnumerator _CameraShake(float duration, float delay, float intensity) {
      activeX = true;
      activeY = true;
      yield return new WaitForSeconds(delay);

      for (float durationTimer = duration; durationTimer > 0; durationTimer -= Time.fixedDeltaTime) {
        float dx = Rand.Normal(0, intensity);
        float dy = Rand.Normal(0, intensity);

        transform.position = virtualPosition + new Vector3(dx, dy, 0);

        yield return new WaitForFixedUpdate();
      }
    }

    private void OnDrawGizmos() {
      Camera cam = transform.GetComponentInChildren<Camera>(true);
      Vector3 position = cam.transform.position;

      Color color = new Color(0.3f, .75f, 0.3f, 1f);
      CameraUtils.DrawCameraBox(position, cam.orthographicSize, cam.aspect, color, 5);
      CameraUtils.DrawCameraBox(position, cam.orthographicSize, 16f/9f, color, 2);
      CameraUtils.DrawCameraBox(position, cam.orthographicSize, 16f/10f, color, 1);
    }
  }

}