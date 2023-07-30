﻿
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// A very simple form of parallax background scrolling.
  /// </summary>
  public class Parallax : MonoBehaviour {

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
    /// The original position of the background.
    /// </summary>
    private Vector3 originalPosition;

    /// <summary>
    /// The scene's camera.
    /// </summary>
    [Tooltip("The scene's camera.")]
    [SerializeField]
    private TargettingCamera targettingCamera = null;

    /// <summary>
    /// The distance of the background from the foreground. Lower means
    /// closer/less paralax
    /// </summary>
    [Tooltip("The distance of the background from the foreground. Lower means closer/less paralax.")]
    [SerializeField]
    private float distance = 0.05f;


    [OnInspectorGUI]
    private void FindCamera() {
      if (targettingCamera == null) {
        targettingCamera = FindObjectOfType<TargettingCamera>();
      }
    }
    
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Start() {
      if (background == null) {
        background = transform;
      }
      FindCamera();
      originalPosition = background.position;

    }


    private void Update() {
      FindCamera();

      Vector3 pos = new Vector3(
        targettingCamera.transform.position.x,
        targettingCamera.transform.position.y,
        background.position.z
      );
      
      background.position = originalPosition + pos*distance;
    }

  }

}