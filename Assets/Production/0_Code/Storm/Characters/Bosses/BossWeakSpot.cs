using System.Collections;
using Storm.Flexible;
using Storm.Subsystems.Reset;
using UnityEngine;

namespace Storm.Characters.Bosses {
  /// <summary>
  /// An area of the boss that the player can attack.
  /// </summary>
  public abstract class BossWeakSpot : MonoBehaviour, ITriggerableParent {
    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// Whether or not the weakspot is currently exposed.
    /// </summary>
    public bool Exposed { get { return exposed; } }
    #endregion

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The boss whose weak spot this is.
    /// </summary>
    public Boss boss;

    /// <summary>
    /// A reference to the animator on this object.
    /// </summary>
    protected Animator animator;

    /// <summary>
    /// The trigger area that's vulernable to attack. 
    /// </summary>
    protected Collider2D damageArea;

    /// <summary>
    /// Whether or not the weakspot is currently exposed.
    /// </summary>
    /// 
    private bool exposed;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected void Awake() {
      animator = GetComponent<Animator>();
    }


    #endregion

    #region Triggerable Parent API
    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------
    public void PullTriggerEnter2D(Collider2D col) { 
      if (exposed && DamageCondition(col)) {
        Hit(col);
      }
    }

    public void PullTriggerStay2D(Collider2D col) { }

    public void PullTriggerExit2D(Collider2D col) { }

    #endregion


    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    /// <summary>
    /// Make it possible for the player to attack this weak spot.
    /// </summary>
    public void Expose() {
      exposed = true;
      OnExposed();
    }

    /// <summary>
    /// Prevent the player from being able to attack this weak spot.
    /// </summary>
    public void Hide() {
      exposed = false;
      OnHidden();
    }

    /// <summary>
    /// Register a hit from the player.
    /// </summary>
    /// <param name="col">The collider of the object that hit this weak spot.</param>
    public void Hit(Collider2D col) {
      exposed = false;
      OnHit(col);
    }

    #endregion

    #region Subclass Interface
    //-------------------------------------------------------------------------
    //  Subclass Interface
    //-------------------------------------------------------------------------
    
    /// <summary>
    /// The condition under which the weak spot should take damage. This is
    /// called whenever something enters the trigger area related to this weak
    /// spot while the weak spot is exposed.
    /// </summary>
    /// <param name="col">The collider of the object that hit the weak spot.</param>
    /// <returns>True if the weak spot should take damage. False otherwise.</returns>
    protected virtual bool DamageCondition(Collider2D col) { return false; }

    /// <summary>
    /// What should happen to signal to the player that this weak spot is open
    /// to attack.
    /// </summary>
    protected virtual void OnExposed() { }

    /// <summary>
    /// What to do when the weak spot is hit.
    /// </summary>
    protected virtual void OnHit(Collider2D col) { }

    /// <summary>
    /// What should happen to signal to the player that his weak spot is no
    /// longer open to attack. This API is used when the weak spot is no longer
    /// vulnerable, but wasn't hit.
    /// </summary>
    protected virtual void OnHidden() { }
    #endregion

  }
}