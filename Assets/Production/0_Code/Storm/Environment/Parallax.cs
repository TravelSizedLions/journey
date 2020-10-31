using Storm.Cameras;
using UnityEngine;

namespace Storm.Environment {

  /// <summary>
  /// A very simple form of parallax background scrolling.
  /// </summary>
  public class Parallax : MonoBehaviour {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    
    /// <summary>
    /// The background object to have be part of the effect.
    /// </summary>
    [Tooltip("The background object to have be part of the effect.")]
    [SerializeField]
    private Transform background = null;

    /// <summary>
    /// The scene's camera.
    /// </summary>
    [Tooltip("The scene's camera.")]
    [SerializeField]
    private TargettingCamera targettingCamera = null;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Start() {
      if (background == null) {
        background = transform;
      }

      if (targettingCamera == null) {
        targettingCamera = FindObjectOfType<TargettingCamera>();
      }
    }


    private void Update() {
      Vector3 pos = new Vector3(
        targettingCamera.transform.position.x,
        targettingCamera.transform.position.y,
        background.position.z
      );
      
      background.position = pos;
    }


    #endregion
  }

}