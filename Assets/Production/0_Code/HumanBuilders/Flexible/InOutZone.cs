using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace HumanBuilders {

  /// <summary>
  /// A zone which triggers a set of events when entering or exiting.
  /// </summary>
  [RequireComponent(typeof(Collider2D))]
  public class InOutZone : MonoBehaviour {
    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// How long to wait before firing the entry events.
    /// </summary>
    [FoldoutGroup("Enter")]
    [Tooltip("How long to wait before firing the entry events.")]
    public float EnterDelay;

    /// <summary>
    /// The events that fire on entering the trigger area.
    /// </summary>
    [FoldoutGroup("Enter")]
    [Tooltip("The events that fire on entering the trigger area.")]
    public UnityEvent EnterEvents;

    /// <summary>
    /// How long to wait before firing the exit events.
    /// </summary>
    [FoldoutGroup("Exit")]
    [Tooltip("How long to wait before firing the exit events.")]
    public float ExitDelay;

    /// <summary>
    /// The events that fire on exiting the trigger area.
    /// </summary>
    [FoldoutGroup("Exit")]
    [Tooltip("The events that fire on exiting the trigger area.")]
    public UnityEvent ExitEvents;
    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        StartCoroutine(InvokeAfter(EnterEvents, EnterDelay));
      }
    }

    private void OnTriggerExit2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        StartCoroutine(InvokeAfter(ExitEvents, ExitDelay));
      }
    }

    private IEnumerator InvokeAfter(UnityEvent events, float delay) {
      yield return new WaitForSeconds(delay);
      events.Invoke();
    }
    #endregion
  }
}