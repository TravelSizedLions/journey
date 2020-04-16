using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using UnityEngine;

namespace Storm.LevelMechanics.LiveWire {
  public class BallisticLiveWireRedirect : MonoBehaviour {

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

        // Snap to this node's position;
        player.transform.position = transform.position;

        // Pick which direction to fling the player.
        Vector2 direction = chooseDirection(player.Rigidbody.velocity);

        // Switch to Ballistic from directed, if necessary.
        if (player.DirectedLiveWireMovement.enabled) {
          player.SwitchBehavior(PlayerBehaviorEnum.BallisticLiveWire);

          // Fling the player at maximum ballistic speed.
          player.Rigidbody.velocity = player.Rigidbody.velocity.normalized * player.AimLiveWireMovement.MaxLaunchSpeed;
        }

        if (player.BallisticLiveWireMovement.enabled) {
          player.transform.position = transform.position;
          disableTimer = delay;
          boxCollider.enabled = false;

          player.BallisticLiveWireMovement.SetDirection(direction);
        } else {
          player.SwitchBehavior(PlayerBehaviorEnum.BallisticLiveWire);



          Vector2 initialVelocity = player.AimLiveWireMovement.MaxLaunchSpeed * direction;
          player.BallisticLiveWireMovement.SetInitialVelocity(initialVelocity);
        }
      }
    }


    public Vector2 chooseDirection(Vector2 playerVelocity) {
      if (Directions2D.areOppositeDirections(backwardDirection, playerVelocity)) {
        return forwardDirection;
      } else {
        return backwardDirection;
      }
    }
  }
}