using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class Dive : MovementBehavior {

    private bool animationFinished;

    private new Rigidbody2D rigidbody;

    private Vector2 diveHop;

    private float idleThreshold;

    private void Awake() {
      AnimParam = "dive";
    }

    public override void HandlePhysics() {
      float input = Input.GetAxis("Horizontal");
      if (input == 0 || Mathf.Abs(rigidbody.velocity.x) < idleThreshold) {
        if (Input.GetButton("Down")) {
          ChangeState<Crouching>();
        }
      } 


      Debug.Log("HandlePhysics");
      if (animationFinished) {
        OnAnimationFinished();
      }
    }

    public void OnAnimationFinished() {
      Debug.Log("OnAnimationFinished");
      animationFinished = true;
      if (Input.GetButton("Down")) {
        if (Input.GetAxis("Horizontal") != 0) {
          ChangeState<Crawling>();
        } else {
          ChangeState<Crouching>();
        }
      } else {
        if (Input.GetAxis("Horizontal") != 0) {
          Debug.Log("Running");
          ChangeState<Running>();
        } else {
          ChangeState<Idle>();
        }
      }
    }

    public override void OnStateEnter(PlayerCharacter p) {
      base.OnStateEnter(p);

      rigidbody = p.rigidbody;
      diveHop = GetComponent<MovementSettings>().DiveHop;

      Vector2 left = new Vector2(-diveHop.x, diveHop.y);
      float input = Input.GetAxis("Horizontal");

      if (input > 0) {
        rigidbody.velocity += diveHop;
      } else {
        rigidbody.velocity += left;
      }

      idleThreshold = GetComponent<MovementSettings>().IdleThreshold;
    }

  }
}