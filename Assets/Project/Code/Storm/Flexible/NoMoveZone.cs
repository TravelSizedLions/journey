using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using UnityEngine;

namespace Storm.Flexible {

  public class NoMoveZone : MonoBehaviour {
    public void OnTriggerEnter2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        other.GetComponent<PlayerCharacter>().NormalMovement.DisableMoving();
      }
    }

    public void OnTriggerExit2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        other.GetComponent<PlayerCharacter>().NormalMovement.EnableMoving();
      }
    }
  }
}