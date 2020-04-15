using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using UnityEngine;

namespace Storm.Flexible {
  public class Deadly : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        PlayerCharacter player = other.GetComponent<PlayerCharacter>();
        player.SwitchBehavior(PlayerBehaviorEnum.Normal);
        GameManager.Instance.KillPlayer(player);
      }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
      if (collision.gameObject.CompareTag("Player")) {
        PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
        player.SwitchBehavior(PlayerBehaviorEnum.Normal);
        GameManager.Instance.KillPlayer(player);
      }
    }

  }


}