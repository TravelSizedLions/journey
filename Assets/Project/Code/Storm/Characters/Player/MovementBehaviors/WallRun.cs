using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class WallRun : HorizontalMotion {
    //private Rigidbody2D playerRB;    

    private float ascensionTime;

    private float ascensionTimer;

    private float wallRunSpeed;

    private float wallRunBoost;

    private bool ascending;

    private float ascensionThreshold;

    private bool fromGround;

    private void Awake() {
      AnimParam = "wall_run";
    }

    public override void OnUpdate() {
      if (Input.GetButtonDown("Jump")) {
        ChangeToState<WallJump>();
      }
    }

    public override void OnFixedUpdate() {
      MoveHorizontally();


      bool leftWall = player.IsTouchingLeftWall();
      bool rightWall = player.IsTouchingRightWall();

      // Only keep wall running if you're touching a wall.
      float yVel = playerRB.velocity.y;

      if (!(leftWall || rightWall)) {
        SwitchState(yVel);
      }
      
      if (yVel < 0) {
        ChangeToState<WallSlide>();
      } else if (yVel < wallRunSpeed) {
        Ascend();
      }
    }

    private void Ascend() {

      // You can only keep wall running for so long.
      if (ascensionTimer > 0) {
        ascensionTimer -= Time.fixedDeltaTime;

        // You can only keep wall running while you hold down the jump button.
        if (ascending && Input.GetButton("Jump")) {
          playerRB.velocity = new Vector2(playerRB.velocity.x, wallRunSpeed);
          
        } else {
          ascending = false;
        }
      } else {
        if (Mathf.Abs(playerRB.velocity.x) > 0) {
          playerRB.velocity = new Vector2(0, playerRB.velocity.y);
        }
      }
      
    }

    private void SwitchState(float verticalVelocity) {
      if (verticalVelocity > 0) {
        ChangeToState<Jump1Rise>();
      } else {
        ChangeToState<Jump1Fall>();
      }
    }

    public override void OnStateAdded() {
      base.OnStateAdded();

      MovementSettings settings = GetComponent<MovementSettings>();
      wallRunSpeed = settings.WallRunSpeed;
      wallRunBoost = settings.WallRunBoost;
      ascensionTime = settings.WallRunAscensionTime;
      ascensionThreshold = settings.AscensionThreshold;
      playerRB = GetComponent<Rigidbody2D>();
    }


    public override void OnStateEnter() {
      BoxCollider2D collider = GetComponent<BoxCollider2D>();

      RaycastHit2D hit = Physics2D.Linecast(((Vector2)collider.bounds.center)-new Vector2(0, collider.bounds.extents.y), ((Vector2)collider.bounds.center-new Vector2(0, 10000)));

      float dist = hit.distance;
      if (hit.distance < ascensionThreshold) {
        ascending = true;
        ascensionTimer = ascensionTime;
        playerRB.velocity = new Vector2(playerRB.velocity.x, wallRunBoost);
      }
    }

  }

}