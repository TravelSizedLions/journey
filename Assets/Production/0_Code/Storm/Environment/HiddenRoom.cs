using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Storm.Environment {

  /// <summary>
  /// Walls that disappear when the player enters behind them
  /// </summary>
  public class HiddenRoom : MonoBehaviour {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    private Animator anim;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      anim = GetComponent<Animator>();
      Tilemap t = GetComponent<Tilemap>();
      t.color = new Color(1, 1, 1, 1);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
      if (collider.CompareTag("Player")) {
        anim.SetTrigger("fade_out");
      }
    }

    private void OnTriggerExit2D(Collider2D collider) {
      if (collider.CompareTag("Player")) {
        anim.SetTrigger("fade_in");
      }
    }
    #endregion
  }
}