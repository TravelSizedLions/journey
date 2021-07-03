using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HumanBuilders {
  /// <summary>
  /// This behavior causes one set of events to fire when the player enters the
  /// zone, and a different set of events to fire when the player exits the zone.
  /// </summary>
  public class ToggleZone : MonoBehaviour {
    //-------------------------------------------------------------------------
    // Events
    //-------------------------------------------------------------------------
    /// <summary>
    /// The events that fire when entering this zone.
    /// </summary>
    [Tooltip("The events that fire when entering this zone.")]
    public UnityEvent EnterEvents;

    /// <summary>
    /// The events that fire when exiting this zone.
    /// </summary>
    [Tooltip("The events that fire when exiting this zone.")]
    public UnityEvent ExitEvents;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player") && enabled) {
        EnterEvents.Invoke();
      }
    }

    private void OnTriggerExit2D(Collider2D col) {
      if (col.CompareTag("Player") && enabled) {
        ExitEvents.Invoke();
      }
    }

    private void OnEnable() {
      Collider2D col = GetComponent<BoxCollider2D>();
      if (GameManager.Player.Collider.IsTouching(col)) {
        EnterEvents.Invoke();
      } else {
        ExitEvents.Invoke();
      }
    }
  }
}