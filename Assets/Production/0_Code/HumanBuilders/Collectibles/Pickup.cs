using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// The base class for anything the player could collect in-game. Examples of this would be currency or in-game items like keys.
  /// </summary>
  [RequireComponent(typeof(SpriteRenderer))]
  [RequireComponent(typeof(Collider2D))]
  public abstract class Pickup : Collectible {
    private void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        OnCollected();
      }
    }
  }
}