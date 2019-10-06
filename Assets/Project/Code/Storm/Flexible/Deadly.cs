using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Characters.Player;

namespace Storm.Flexible {
    public class Deadly : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
                PlayerCharacter player = other.GetComponent<PlayerCharacter>();
                GameManager.Instance.KillPlayer(player);
            }
        }
    }

}

