using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using UnityEngine;


namespace Storm.Flexible {


  public class NoJumpZone : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        other.GetComponent<PlayerCharacter>().NormalMovement.DisableJump();
      }
    }

    public void OnTriggerExit2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        other.GetComponent<PlayerCharacter>().NormalMovement.EnableJump();
      }
    }
  }
}