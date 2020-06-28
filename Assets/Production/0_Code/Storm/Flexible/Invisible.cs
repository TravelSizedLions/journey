using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Storm.Flexible {


  /// <summary>
  /// This behavior causes the game object attached to it to become invisible. This is useful for creating invisible walls while laying out your level, or for creating in-editor markers that are necessary for building the game, but shouldn't appear during gameplay.
  /// </summary>
  public class Invisible : MonoBehaviour { 

    /// <summary>
    /// Whether or not the children of this object should also be invisible.
    /// </summary>
    [Tooltip("Whether or not the children of this object should also be invisible.")]
    public bool HideChildren;

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    void Awake() {
      var sprite = GetComponent<SpriteRenderer>();
      if (sprite != null) {
        sprite.enabled = false;
      }

      var image = GetComponent<Image>();
      if (image != null) {
        image.enabled = false;
      }

      if (HideChildren) {
        foreach (var child in GetComponentsInChildren<SpriteRenderer>(true)) {
          child.enabled = false;
        }

        foreach (var child in GetComponentsInChildren<Image>(true)) {
          child.enabled = false;
        }
      }
    }
    #endregion
  }
}