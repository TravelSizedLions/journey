using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {

  public class Floating : MonoBehaviour {


    /// <summary>
    /// How quickly the object bobs up and down.
    /// </summary>
    [Tooltip("How quickly the object bobs up and down, in bobs/second.")]
    [SerializeField]
    private float frequency;

    /// <summary>
    /// The amount that the object moves up and down.
    /// </summary>
    [Tooltip("The amount that the object moves up and down, in Unity units.")]
    [SerializeField]
    private float magnitude;

    /// <summary>
    /// The original starting position of the object.
    /// </summary>
    private Vector3 originalPosition;

    /// <summary>
    /// The current position in the bob.
    /// </summary>
    private float currentPosition;


    private void Start() {
      originalPosition = transform.position;
    }

    private void LateUpdate() {
      transform.position = new Vector3(
        originalPosition.x,
        originalPosition.y+(magnitude*Mathf.Sin(frequency*Time.deltaTime)),
        originalPosition.z
      );
    }
  }
}