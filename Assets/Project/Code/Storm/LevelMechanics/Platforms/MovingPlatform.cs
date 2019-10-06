using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Characters.Player;
using Storm.Cameras;

namespace Storm.LevelMechanics.Platforms {
    public class MovingPlatform : MonoBehaviour {
        public void OnCollisionEnter2D(Collision2D collision) {
            if (collision.collider.CompareTag("Player")) {
                collision.collider.transform.SetParent(transform);
                PlayerCharacter player = collision.collider.GetComponent<PlayerCharacter>();
                
                player.activeMovementMode.EnablePlatformMomentum();
            }
        }

        public void OnCollisionExit2D(Collision2D collision) {
            if (collision.collider.CompareTag("Player")) {
                PlayerCharacter player = collision.collider.GetComponent<PlayerCharacter>();
                
                player.activeMovementMode.DisablePlatformMomentum();
            }
        }
    }
}