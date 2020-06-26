using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.LevelMechanics.Doors {

  /// <summary>
  /// 
  /// </summary>
  public class AutomaticDoor : MonoBehaviour {

    #region Fields
    /// <summary>
    /// A reference to the door's animator.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// How long to wait before closing the door.
    /// </summary>
    [SerializeField]
    [Tooltip("How long to wait before closing the door.")]
    private float closeDelay = 1f;

    #endregion


    #region Unity API
    private void Awake() {
      animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        animator.SetBool("open", true);
      }
    }

    private void OnTriggerExit2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        StartCoroutine(DelayClose(closeDelay));
      }
    }

    private IEnumerator DelayClose(float seconds) {
      for (float delay = seconds; delay > 0; delay -= Time.deltaTime) {
        yield return null;
      }

      animator.SetBool("open", false);
    }
    #endregion
  }
}