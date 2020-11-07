using Storm.Characters.Player;
using Storm.Flexible;
using UnityEngine;

namespace Storm.LevelMechanics {
  public class DirectionalFlingFlower : FlingFlower, ITriggerableParent {
    public void PullTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        PlayerCharacter player = col.GetComponent<PlayerCharacter>();
        player.Signal(gameObject);
      }
    }

    public void PullTriggerExit2D(Collider2D col) {}

    public void PullTriggerStay2D(Collider2D col) {}
  }

}