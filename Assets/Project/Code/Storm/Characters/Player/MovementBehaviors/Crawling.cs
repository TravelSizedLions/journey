using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class Crawling : PlayerState {


    private Rigidbody2D playerRB;

    private float crawlSpeed;


    private void Awake() {
      AnimParam = "crawling";
    }


    public override void OnStateAdded() {
      playerRB = GetComponent<Rigidbody2D>();

      MovementSettings settings = GetComponent<MovementSettings>();

      crawlSpeed = settings.CrawlSpeed;
    }

    public override void OnUpdate() {
      if (!Input.GetButton("Down")) {
        if (Input.GetAxis("Horizontal") != 0) {
          ChangeToState<Running>();
        } 
      }
    }
    
    public override void OnFixedUpdate() {
      float input = Input.GetAxis("Horizontal");

      if (!player.IsTouchingGround()) {
        ChangeToState<Jump1Fall>();
      }

      if (input != 0) {
        float inputDirection = Mathf.Sign(input);
        float playerMovement = inputDirection*crawlSpeed;

        playerRB.velocity = new Vector2(playerMovement, playerRB.velocity.y);

        Facing facing = (Facing)inputDirection;
        player.SetFacing(facing);
      } else {
        ChangeToState<Crouching>();
      }

    }
  }

}