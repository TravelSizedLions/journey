using System.Collections;
using System.Collections.Generic;
using Storm.ResetSystem;
using UnityEngine;

namespace Storm.LevelMechanics.Platforms {

  /// <summary>
  /// One block of a crumbling platform.
  /// </summary>
  /// <seealso cref="CrumblingPlatform" />
  public class CrumblingBlock : MonoBehaviour {

    #region Variables

    /// <summary>
    /// Whether or not the block should be deteriorating. This gets set when the player touches the crumbling platform.
    /// </summary>
    public bool deteriorating;

    /// <summary>
    /// The collider that's responsible for collision physics.
    /// </summary>
    public BoxCollider2D physicsCol;

    /// <summary>
    /// The collider that's responsible for triggering deterioration.
    /// </summary>
    public BoxCollider2D triggerCol;

    /// <summary>
    /// The sprite for this block.
    /// </summary>
    public SpriteRenderer sprite;

    /// <summary>
    /// A reference to the animator.
    /// </summary>
    public Animator anim;

    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    void Start() {
      deteriorating = false;
      anim = GetComponent<Animator>();
      var cols = GetComponents<BoxCollider2D>();
      physicsCol = cols[0];
      triggerCol = cols[1];
      sprite = GetComponent<SpriteRenderer>();
    }


    void OnTriggerEnter2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        deteriorating = true;
        triggerCol.enabled = false;
      }
    }
    #endregion
  }

}