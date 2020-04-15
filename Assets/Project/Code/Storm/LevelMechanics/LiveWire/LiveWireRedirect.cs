using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using UnityEngine;

namespace Storm.LevelMechanics.LiveWire {

  public class LiveWireRedirect : MonoBehaviour {
    public Direction forwardMotionDirection;

    public Direction backwardMotionDirection;

    private Vector2 forwardDirection;

    private Vector2 backwardDirection;

    private BoxCollider2D boxCollider;
    private float disableTimer;

    private float delay = 0.125f;

    // Start is called before the first frame update
    void Awake() {
      forwardDirection = Directions2D.toVector(forwardMotionDirection);
      backwardDirection = Directions2D.toVector(backwardMotionDirection);
      boxCollider = GetComponent<BoxCollider2D>();
    }

    public void Update() {
      if (!boxCollider.enabled) {
        disableTimer -= Time.deltaTime;
        if (disableTimer < 0) {
          boxCollider.enabled = true;
        }
      }
    }

    public void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        PlayerCharacter player = GameManager.Instance.player;

        if (!(player.directedLiveWireMovement.enabled || player.ballisticLiveWireMovement.enabled)) {
          return;
        }

        if (player.ballisticLiveWireMovement.enabled) {
          player.SwitchBehavior(PlayerBehaviorEnum.DirectedLiveWire);
        }

        player.transform.position = transform.position;
        disableTimer = delay;
        boxCollider.enabled = false;

        if (Directions2D.areOppositeDirections(backwardDirection, player.rb.velocity)) {
          player.directedLiveWireMovement.SetDirection(forwardDirection);
        } else if (Directions2D.areOppositeDirections(forwardDirection, player.rb.velocity)) {
          player.directedLiveWireMovement.SetDirection(backwardDirection);
        } else {
          player.directedLiveWireMovement.SetDirection(forwardDirection);
        }
      }
    }
  }

}