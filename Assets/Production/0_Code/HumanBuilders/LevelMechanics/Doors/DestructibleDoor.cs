using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// A door that can be destroyed by throwing an object at it.
  /// </summary>
  public class DestructibleDoor : MonoBehaviour {

    private void OnCollisionEnter2D(Collision2D col) {
      Transform root = col.collider.transform.root;
      Carriable carriable = root.GetComponentInChildren<Carriable>();
      if (carriable != null) {
        carriable.Physics.Velocity = Vector2.zero;
        Destroy(gameObject);
      }
    }
  }
}