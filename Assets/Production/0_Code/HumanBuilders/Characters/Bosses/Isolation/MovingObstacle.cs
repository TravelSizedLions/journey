using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {

  public class MovingObstacle : MonoBehaviour {
    
    public float speed = 1;

    private float ttl = 60000;
    private Rigidbody2D rb;

    public void Awake() {
      rb = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate() {
      rb.velocity = Vector2.left*speed;
      ttl -= Time.fixedDeltaTime;
      if (ttl < 0) {
        Destroy(gameObject);
      }
    }
  }
}