using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Characters.Player;


namespace Storm.Flexible {


    public class NoJumpZone : MonoBehaviour {
        void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                other.GetComponent<PlayerCharacter>().activeMovementMode.DisableJump();
            }
        }

        public void OnTriggerExit2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                other.GetComponent<PlayerCharacter>().activeMovementMode.EnableJump();
            }
        }
    }
}