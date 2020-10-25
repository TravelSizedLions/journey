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
    private bool exposed;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected void Awake() {
      animator = GetComponent<Animator>();
    }


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
    public void Expose() {
      exposed = true;
      OnExposed();
    }

    public void Hit(Collider2D col) {
      exposed = false;
      OnHit(col);
    }


    #endregion

    #region Public Abstract Interface
    //-------------------------------------------------------------------------
    // Public Abstract Interface
    //-------------------------------------------------------------------------
    /// <summary>
    /// The condition under which the weak spot should take damage. This is
    /// called whenever something enters the trigger area while the weak spot is
    /// exposed.
    /// </summary>
    /// <param name="col">The collider of the object that hit the weak spot.</param>
    /// <returns>True if the weak spot should take damage. False otherwise.</returns>
    public abstract bool DamageCondition(Collider2D col);

    /// <summary>
    /// Open up this weak spot to attack from the player.
    /// </summary>
    public abstract void OnExposed();

    /// <summary>
    /// What to do when the weak spot is hit.
    /// </summary>
    public abstract void OnHit(Collider2D col);
    #endregion

  }
}