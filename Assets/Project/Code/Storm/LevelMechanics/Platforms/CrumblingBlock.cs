using UnityEngine;
using Storm.Attributes;
using Storm.Components;

namespace Storm.LevelMechanics.Platforms {

  /// <summary>
  /// One block of a crumbling platform.
  /// </summary>
  /// <seealso cref="CrumblingPlatform" />
  public class CrumblingBlock : MonoBehaviour {

    #region Variables
  
    #region Appearance & Animation
    /// <summary>
    /// Whether or not the block should be crumbling away. This gets set when the player touches the crumbling platform.
    /// </summary>
    [ReadOnly]
    public bool IsCrumbling;

    /// <summary>
    /// The sprite for this block.
    /// </summary>
    private SpriteRenderer sprite;

    /// <summary>
    /// A reference to the animator.
    /// </summary>
    private Animator anim;
    #endregion

    #region Colliders
    /// <summary>
    /// The collider that's responsible for collision physics.
    /// </summary>
    private BoxCollider2D physicsCol;

    /// <summary>
    /// The collider that's responsible for triggering crumbling.
    /// </summary>
    private BoxCollider2D triggerCol;

    #endregion

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Start() {
      IsCrumbling = false;
      anim = GetComponent<Animator>();
      var cols = GetComponents<BoxCollider2D>();
      physicsCol = cols[0];
      triggerCol = cols[1];
      sprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
      // Signal that the the block should start crumbling away when the player touches it.
      if (other.CompareTag("Player")) {
        IsCrumbling = true;
        triggerCol.enabled = false;
      }
    }
    #endregion

    #region  Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Make the block visible and solid.
    /// </summary>
    public void Enable() {
        physicsCol.enabled = true;
        triggerCol.enabled = true;
        sprite.enabled = true;
    }

    /// <summary>
    /// Make the block disappear.
    /// </summary>
    public void Disable() {
        physicsCol.enabled = false;
        triggerCol.enabled = false;
        sprite.enabled = false;
    }

    /// <summary>
    /// Change the animation state of the block.
    /// </summary>
    /// <param name="state">Which state the block should be in.</param>
    public void ChangeState(int state) {
      anim.SetInteger("State", state);
    }

    #endregion
  }

}