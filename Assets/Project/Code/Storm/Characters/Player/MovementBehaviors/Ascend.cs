using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {
  public class Ascend : MovementBehavior {
    #region Fields
    private float jumpForce = 16f;

    private Rigidbody2D playerRB;
    #endregion

    #region Movement Behavior Implementation
    //-------------------------------------------------------------------------
    // Movement Behavior Implementation
    //-------------------------------------------------------------------------

    public override void HandlePhysics() {
      player.SetAnimParam("y_velocity", playerRB.velocity.y);
      if (playerRB.velocity.y < 0) {
        player.TriggerAnimation();
        ChangeState<Fall>();
      }
    }

    public override void OnStateEnter(PlayerCharacter player) {
      base.OnStateEnter(player);

      player.SetAnimParam("in_air", true);

      playerRB = GetComponent<Rigidbody2D>();
      playerRB.velocity += new Vector2(0, jumpForce);
    }
    #endregion
  }
}