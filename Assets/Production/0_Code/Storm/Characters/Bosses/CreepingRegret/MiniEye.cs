﻿using Storm.Flexible;
using Storm.Flexible.Interaction;
using UnityEngine;

namespace Storm.Characters.Bosses {
  public class MiniEye : BossWeakSpot {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Call back signature type.
    /// </summary>
    public delegate void OnEyeCloseCallback();

    /// <summary>
    /// Event fires when the eye closes;
    /// </summary>
    public event OnEyeCloseCallback OnEyeClose;
    #endregion

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// A component for shaking the eye.
    /// </summary>
    private Shaking shaking;

    /// <summary>
    /// A reference to creeping regret's main eye.
    /// </summary>
    private MegaEye megaEye;
    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    protected new void Awake() {
      base.Awake();
      shaking = GetComponent<Shaking>();
      megaEye = FindObjectOfType<MegaEye>();
    }
    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    /// <summary>
    /// Take damage if the object that hit it is a Carriable item.
    /// </summary>
    /// <param name="col">the collider of the object that hit the eye.</param>
    /// <returns>True if the Eye is open and the object that hit the eye is a
    /// <see cref="Carriable" /></returns>
    public override bool DamageCondition(Collider2D col) {
      Carriable carriable = col.transform.root.GetComponent<Carriable>();
      return carriable != null && 
             col == carriable.Collider && 
             Exposed;
    }

    /// <summary>
    /// Opens the eye.
    /// </summary>
    public override void OnExposed() {
      // Open up the eye.
      Open();
    }

    /// <summary>
    /// Close the eye.
    /// </summary>
    /// <param name="col">The collider of the object that hit the eye.</param>
    public override void OnHit(Collider2D col) {
      // Stop the object that hit the eye.
      Carriable carriable = col.transform.root.GetComponent<Carriable>();
      if (carriable != null) {
        carriable.Physics.Velocity = Vector2.zero;
      }

      // Have the eye shake and flash red.
      shaking.Shake();
      animator.SetTrigger("damage");

      // Close the eye.
      Close();
    }

    /// <summary>
    /// Close the eye.
    /// </summary>
    /// <param name="preventCallback">Whether or not the skip the callback for
    /// closing the eyes.</param>
    public void Close(bool preventCallback = false) {
      animator.SetBool("open", false);
      if (OnEyeClose != null && !preventCallback)  {
        OnEyeClose.Invoke();
      }
    }

    
    /// <summary>
    /// Open the eye.
    /// </summary>
    public void Open() {
      animator.SetBool("open", true);
      shaking.Shake();
    }
    #endregion


    #region Helper Methods
    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------

    #endregion

  }
}