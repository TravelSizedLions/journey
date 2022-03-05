using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {
  public class Fruit : MonoBehaviour {
    public float Points = 1;
    public Vector2 GenRange = new Vector2(100, 100);
    public SpriteRenderer FruitSprite;

    public void Awake() {
      NewSpot();
    }

    public void OnTriggerEnter2D(Collider2D collider) {
      if (collider.CompareTag("Player")) {
        NewSpot();
        GameManager.Inventory.AddCurrency("FlopCoin", Points);
        SnakePlayer player = collider.GetComponent<SnakePlayer>();
        if (player != null) {
          player.Extend();
        }
      }
    }

    public void NewSpot() {
      float x = Random.Range(-GenRange.x, GenRange.x);
      float y = Random.Range(-GenRange.y, GenRange.y);
      transform.position = new Vector3(x, y, transform.position.z);
    }
  }
}
