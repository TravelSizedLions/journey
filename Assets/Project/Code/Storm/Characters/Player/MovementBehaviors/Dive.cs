using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public class Dive : PlayerState {

    private Rigidbody2D playerRB;

    private Vector2 rightDiveHop;

    private Vector2 leftDiveHop;

    private void Awake() {
      AnimParam = "dive";
    }

    public void OnDiveFinished() {
      ChangeToState<Crawling>();
    }

    public override void OnStateAdded() {
      playerRB = GetComponent<Rigidbody2D>();

      MovementSettings settings = GetComponent<MovementSettings>();

      rightDiveHop = settings.DiveHop;
      leftDiveHop = new Vector2(-rightDiveHop.x, rightDiveHop.y);
    }

    public override void OnStateEnter() {
      if (playerRB.velocity.x > 0) {
        playerRB.velocity += rightDiveHop;
      } else {
        playerRB.velocity += leftDiveHop;
      }
    }
  }
}