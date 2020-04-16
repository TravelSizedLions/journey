using System.Collections;
using System.Collections.Generic;
using Storm.Cameras;
using Storm.Characters.Player;
using UnityEngine;

namespace Storm.LevelMechanics.Platforms {
  public class MovingPlatform : MonoBehaviour {
    public void OnCollisionEnter2D(Collision2D collision) {
      if (collision.collider.CompareTag("Player")) {
        collision.collider.transform.SetParent(transform);
        PlayerCharacter player = collision.collider.GetComponent<PlayerCharacter>();

        player.NormalMovement.EnablePlatformMomentum();
      }
    }

    public void OnCollisionExit2D(Collision2D collision) {
      if (collision.collider.CompareTag("Player")) {
        PlayerCharacter player = collision.collider.GetComponent<PlayerCharacter>();

        player.NormalMovement.DisablePlatformMomentum();
      }
    }
  }
}