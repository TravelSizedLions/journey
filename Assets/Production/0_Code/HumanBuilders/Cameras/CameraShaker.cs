using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// A class that can trigger a camera-shake effect.
  /// </summary>
  public class CameraShaker : MonoBehaviour {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    [Tooltip("How much the camera should shake.")]
    public float Intensity = 1f;

    [Tooltip("How long the camera should shake (in seconds).")]
    public float Duration = 1f;

    [Tooltip("How long the camera should wait before shaking (in seconds).")]
    public float Delay = 1f;

    private TargettingCamera cam;

    private void Awake() {
      cam = FindObjectOfType<TargettingCamera>();
    }
  
    public void Shake() {
      if (cam == null) {
        cam = FindObjectOfType<TargettingCamera>();
      }
      
      if (cam != null) {
        cam.CameraShake(Duration, Delay, Intensity);
      }
    }
  }
}