using System.Collections;
using System.Collections.Generic;
using Storm.Attributes;
using Storm.Characters.Player;
using UnityEngine;

namespace Storm.LevelMechanics.Doors {

  /// <summary>
  /// 
  /// </summary>
  public class AutomaticDoor : MonoBehaviour {

    #region Fields
    /// <summary>
    /// How long to wait before closing the door.
    /// </summary>
    [SerializeField]
    [Tooltip("How long to wait before closing the door in seconds.")]
    private float closeDelay = 1f;

    /// <summary>
    /// How fast should the door open and close.
    /// </summary>
    [SerializeField]
    [Tooltip("How fast the door should open and close.")]
    private float speed = 1f;

    /// <summary>
    /// The door.
    /// </summary>
    [Tooltip("The door.")]
    [SerializeField]
    public Transform door;

    /// <summary>
    /// The position of the door when it's closed.
    /// </summary>
    [Tooltip("The position of the door when it's closed.")]
    [SerializeField]
    public Transform closePoint;

    /// <summary>
    /// The position of the door when it's open.
    /// </summary>
    [Tooltip("The position of the door when it's open.")]
    [SerializeField]
    public Transform openPoint;

    /// <summary>
    /// Whether or not the door should be opening or closing.
    /// </summary>
    [Tooltip("Whether or not the door should be opening or closing.")]
    [SerializeField]
    [ReadOnly]
    private bool open;

    /// <summary>
    /// The direction the door moves when it opens.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private Vector3 openDirection;
    #endregion

    #region Unity API
    private void Awake() {
      openDirection = (Vector3)(openPoint.position - closePoint.position).normalized;
    }

    private void Update() {
      if (open) {
        OpenDoor();
      } else {
        CloseDoor();
      }
    }

    private void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        open = true;
      }
    }

    private void OnTriggerStay2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        open = true;
      }
    }

    private void OnTriggerExit2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        StartCoroutine(DelayClose(closeDelay));
      }
    }

    #endregion

    #region Helper Functions
    private void OpenDoor() {
      door.position += openDirection*(speed*Time.deltaTime);
      if (IsPastOpenPoint()) {
        door.position = openPoint.position;
      }
    }

    private bool IsPastOpenPoint() {
      return (openDirection.y > 0 && door.position.y > openPoint.position.y) ||
             (openDirection.y < 0 && door.position.y < openPoint.position.y);
    }

    private void CloseDoor() {
      door.position -= openDirection*(speed*Time.deltaTime);
      if (IsPastClosePoint()) {
        door.position = closePoint.position;
      }
    }

    private bool IsPastClosePoint() {
      return (openDirection.y > 0 && door.position.y < closePoint.position.y) ||
             (openDirection.y < 0 && door.position.y > closePoint.position.y);
    }

    private IEnumerator DelayClose(float seconds) {
      for (float delay = seconds; delay > 0; delay -= Time.deltaTime) {
        yield return null;
      }

      open = false;
    }
    #endregion
  }
}