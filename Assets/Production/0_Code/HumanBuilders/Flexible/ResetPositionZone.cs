using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// A trigger zone that moves an object back to its saved position.
  /// </summary>
  public class ResetPositionZone : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collider) {
      SavePosition pos = collider.transform.root.GetComponentInChildren<SavePosition>();
      
      if (pos != null) {
        // Move the object back to its last saved position and 
        pos.Retrieve();

        Rigidbody2D rb = collider.transform.root.GetComponentInChildren<Rigidbody2D>();
        if (rb != null) {
          rb.velocity = Vector3.zero;
        }
      }
    }
  }
}