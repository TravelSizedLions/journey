using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class Dive : MovementBehavior {

    private new Rigidbody2D rigidbody;

    private float idleThreshold;

    private void Awake() {
      AnimParam = "dive";
    }

    public override void HandlePhysics() {
      if (Input.GetAxis("Horizontal") == 0 || Mathf.Abs(rigidbody.velocity.x) < idleThreshold) {
        if (Input.GetButton("Down")) {
          ChangeState<Crouching>();
        }
      }
    }

    public void OnAnimationFinished() {
      if (Input.GetButton("Down")) {
        if (Input.GetAxis("Horizontal") != 0) {
          ChangeState<Crawling>();
        } else {
          ChangeState<Crouching>();
        }
      } else {
        if (Input.GetAxis("Horizontal") != 0) {
          ChangeState<Running>();
        } else {
          ChangeState<Idle>();
        }
      }
    }

    public override void OnStateEnter(PlayerCharacter p) {
      base.OnStateEnter(p);

      rigidbody = p.rigidbody;
      
      float input = Input.GetAxis("Horizontal");
      float inputDirection = Mathf.Sign(input);

      rigidbody.velocity += (inputDirection*(new Vector2(12, 0)) + new Vector2(0, 12));


      idleThreshold = GetComponent<MovementSettings>().IdleThreshold;
    }

  }
}