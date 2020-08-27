using UnityEngine;


namespace Storm.Cameras {
  /// <summary>
  /// A class that can trigger a camera-shake effect.
  /// </summary>
  public class CameraShaker : MonoBehaviour {
    #region Fields
    /// <summary>
    /// How much the camera should shake.
    /// </summary>
    [Tooltip("How much the camera should shake.")]
    public float Intensity = 1f;

    /// <summary>
    /// How long the camera should shake.
    /// </summary>
    [Tooltip("How long the camera should shake (in seconds).")]
    public float Duration = 1f;


    /// <summary>
    /// How long the camera should wait before shaking.
    /// </summary>
    [Tooltip("How long the camera should wait before shaking (in seconds).")]
    public float Delay = 1f;


    /// <summary>
    /// A reference to the targetting camera.
    /// </summary>
    private TargettingCamera cam;
    #endregion


    #region Unity API

    private void Awake() {
      cam = FindObjectOfType<TargettingCamera>();
    }
    #endregion
  
  
    #region Public Interface
    /// <summary>
    /// Shake the targetting camera.
    /// </summary>
    public void Shake() {
      if (cam == null) {
        cam = FindObjectOfType<TargettingCamera>();
      }
      
      if (cam != null) {
        cam.CameraShake(Duration, Delay, Intensity);
      }
    }
    #endregion
  }
}