using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Flexible {
  /// <summary>
  /// A behavior which allows a child game object with a collider to trigger its parent.
  /// TODO: For performance reasons, it might actually make sense to separate
  /// out each of the trigger callbacks into its own component. This way, we can
  /// avoid unnessary calls to the performance sensitive OnTriggerStay2D
  /// callback here.
  /// </summary>
  public class TriggerChild : MonoBehaviour {

    /// <summary>
    /// Whether or not the trigger should fire on trigger enter.
    /// </summary>
    public bool enter;

    /// <summary>
    /// Whether or not the trigger should fire on trigger stay.
    /// </summary>
    public bool stay;

    /// <summary>
    /// Whether or not the trigger should fire on trigger exit.
    /// </summary>
    public bool exit;
  
    #region Unity API
    private void OnTriggerEnter2D(Collider2D col) {
      if (enter) GetParent().PullTriggerEnter2D(col);
    }

    private void OnTriggerStay2D(Collider2D col) {
      if (stay) GetParent().PullTriggerStay2D(col);
    }

    private void OnTriggerExit2D(Collider2D col) {
      if (exit) GetParent().GetComponent<TriggerableParent>().PullTriggerExit2D(col);
    }

    #endregion


    #region Auxiliary Methods

    /// <summary>
    /// Traverses up the parent transforms until it finds a parent that can be triggered.
    /// </summary>
    /// <returns>A game object that implements TriggerableParent.</returns>
    private TriggerableParent GetParent() {
      TriggerableParent triggerParent = transform.parent.gameObject.GetComponent<TriggerableParent>();
      if (triggerParent == null) {
        Transform parent = transform.parent;

        while(triggerParent == null && transform.parent != null) {
          parent = parent.transform.parent;

          triggerParent = transform.parent.gameObject.GetComponent<TriggerableParent>();          
        }
      }

      if (triggerParent == null) {
        throw new NullReferenceException("Could not find a trigger parent for " + gameObject.name + ".");
      }

      return triggerParent;
    }

    #endregion
  }
}
