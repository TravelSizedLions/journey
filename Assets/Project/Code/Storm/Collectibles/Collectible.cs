using System.Collections;
using System.Collections.Generic;
using Storm.AudioSystem;
using UnityEngine;

namespace Storm.Collectibles {

  [RequireComponent(typeof(SpriteRenderer))]
  [RequireComponent(typeof(Collider2D))]
  public abstract class Collectible : MonoBehaviour {

    protected bool collected = false;


    protected virtual void Awake() {

    }

    public void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        OnCollected();
      }
    }

    public virtual void OnCollected() {
      collected = true;
    }


    public bool IsCollected() {
      return collected;
    }
  }
}