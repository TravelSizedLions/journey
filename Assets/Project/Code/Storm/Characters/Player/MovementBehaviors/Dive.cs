using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public class Dive : PlayerState {

    private Rigidbody2D playerRB;

    private Vector2 rightDiveHop;

    private Vector2 leftDiveHop;

    private bool animFinished;

    private void Awake() {
      AnimParam = "dive";
    }


    public override void OnFixedUpdate() {
      if (animFinished && player.IsTouchingGround()) {
        ChangeToState<Crawling>();
      }
    }

    public void OnDiveFinished() {
      animFinished = true;
    }

    public override void OnStateAdded() {
      playerRB = GetComponent<Rigidbody2D>();

      MovementSettings settings = GetComponent<MovementSettings>();

      rightDiveHop = settings.DiveHop;
      leftDiveHop = new Vector2(-rightDiveHop.x, rightDiveHop.y);
    }

    public override void OnStateEnter() {
      animFinished = false;
      if (playerRB.velocity.x > 0) {
        playerRB.velocity += rightDiveHop;
      } else {
        playerRB.velocity += leftDiveHop;
      }
    }
  }
}