using UnityEngine;

namespace Storm.Characters.Bosses {
  public abstract class BossWeakSpot : MonoBehaviour {
    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// Whether or not the weakspot is currently exposed.
    /// </summary>
    public bool Exposed { get { return exposed; }}
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

    private void OnTriggerEnter2D(Collider2D col) {
      if (exposed && DamageCondition(col)) {
        TakeDamage();
      }
    }

    #endregion
    
    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    public void Expose() {
      exposed = true;
      OnExposed();
    }
    
    public void TakeDamage() {
      exposed = false;
      OnTakeDamage();
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
    public abstract void OnTakeDamage();
    #endregion

  }
}