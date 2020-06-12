using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Flexible {
  /// <summary>
  /// A behavior which allows a child game object with a collider to trigger its parent.
  /// </summary>
  public class TriggerChild : MonoBehaviour {
  
    #region Unity API
    private void OnTriggerEnter2D(Collider2D col) {
      GetParent().PullTriggerEnter2D(col);
    }

    private void OnTriggerStay2D(Collider2D col) {
      GetParent().PullTriggerStay2D(col);
    }

    private void OnTriggerExit2D(Collider2D col) {
      GetParent().GetComponent<TriggerableParent>().PullTriggerExit2D(col);
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
