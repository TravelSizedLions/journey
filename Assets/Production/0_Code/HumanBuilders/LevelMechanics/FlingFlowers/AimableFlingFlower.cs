using System.Collections;
using System.Collections.Generic;


using UnityEngine;

namespace HumanBuilders {
  public class AimableFlingFlower : FlingFlower, ITriggerableParent {
    public void PullTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        PlayerCharacter player = col.GetComponent<PlayerCharacter>();
        player.Signal(gameObject);
      }
    }

    public void PullTriggerExit2D(Collider2D col) { }

    public void PullTriggerStay2D(Collider2D col) { }
  }
}