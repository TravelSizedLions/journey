using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {

  public class Fall : MovementBehavior {

    private Rigidbody2D playerRB;

    public override void OnStateEnter(PlayerCharacter p) {
      base.OnStateEnter(p);
      
      playerRB = p.GetComponent<Rigidbody2D>();
    }

    public override void HandlePhysics() {
      player.SetAnimParam("y_velocity", playerRB.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      if (collision.gameObject.CompareTag("Ground") && player.IsTouchingGround()) {
        ChangeState<Land>();
      }
    }
  }
}