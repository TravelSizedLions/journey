using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {

  public class MovingObstacle : MonoBehaviour {
    
    public float speed = 1;

    private Rigidbody2D rb;

    public void Awake() {
      rb = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate() {
      rb.velocity = Vector2.left*speed;
    }
  }
}