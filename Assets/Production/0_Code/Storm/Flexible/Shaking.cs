using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// An object that shakes.
  /// </summary>
  public class Shaking : MonoBehaviour {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

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
    /// Start shaking as soon as the scene loads.
    /// </summary>
    [Tooltip("Start shaking as soon as the scene loads.")]
    public bool StartOnAwake;

    /// <summary>
    /// The original position of the body.
    /// </summary>
    private Vector3 originalPosition;
    #endregion

    #region Unity API
    private void Awake() {
      originalPosition = transform.position;
      if (StartOnAwake) {
        StartCoroutine(_Shake(Duration, Delay, Intensity));
      }
    }

    #endregion


    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    public void Shake() {
      StartCoroutine(_Shake(Duration, Delay, Intensity));
    }

    private IEnumerator _Shake(float duration, float delay, float intensity) {
      for (float delayTimer = delay; delayTimer > 0; delayTimer -= Time.deltaTime) {
        yield return null;
      }

      float realIntensity;
      float slope = 2*(intensity/duration);
      for (float curTime = 0 ; curTime < duration; curTime += Time.deltaTime) {
        if (duration != 0) {
          float percent=curTime/duration;
          if (percent > 0.5) {
            // ramp up intensity
            realIntensity = slope*percent;
          } else {
            // ramp down intensity
            realIntensity = (2*intensity) - (slope*percent); 
          }
        } else {
          realIntensity = intensity;
        }

        float dx = Rand.Normal(0, realIntensity);
        float dy = Rand.Normal(0, realIntensity);

        transform.position = originalPosition + new Vector3(dx, dy, 0);

        yield return null;
      }

      transform.position = originalPosition;
    }
    #endregion
  }

}