using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Characters.Player;

namespace Storm.Flexible {
        
    public class NoMoveZone : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                other.GetComponent<PlayerCharacter>().activeMovementMode.DisableMoving();
            }
        }

        public void OnTriggerExit2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                other.GetComponent<PlayerCharacter>().activeMovementMode.EnableMoving();
            }
        }
    }
}