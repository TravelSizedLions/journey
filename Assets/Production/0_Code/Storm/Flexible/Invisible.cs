using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Storm.Flexible {


  /// <summary>
  /// This behavior causes the game object attached to it to become invisible. This is useful for creating invisible walls while laying out your level, or for creating in-editor markers that are necessary for building the game, but shouldn't appear during gameplay.
  /// </summary>
  [RequireComponent(typeof(GuidComponent))]
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

    private void OnEnable() {
      if (HideChildren) {
        HideRecursive();
      } else {
        HideSelf();
      }
    }


    private void OnDisable() {
      if (HideChildren) {
        ShowRecursive();
      } else {
        ShowSelf();
      }
    }

    #endregion

    #region Helper Methods
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    /// <summary>
    /// Hide visible components on this gameobject.
    /// </summary>
    private void HideSelf() {
      var sprite = GetComponent<SpriteRenderer>();
      if (sprite != null) {
        sprite.enabled = false;
      }

      var image = GetComponent<Image>();
      if (image != null) {
        image.enabled = false;
      }

      var text = GetComponent<TextMeshProUGUI>();
      if (text != null) {
        text.enabled = false;
      }
    }

    /// <summary>
    /// Hide visible components on this gameobject and all descendents.
    /// </summary>
    private void HideRecursive() {
      foreach (var child in GetComponentsInChildren<SpriteRenderer>(true)) {
        child.enabled = false;
      }

      foreach (var child in GetComponentsInChildren<Image>(true)) {
        child.enabled = false;
      }

      foreach (var child in GetComponentsInChildren<TextMeshProUGUI>(true)) {
        child.enabled = false;
      }
    }

    /// <summary>
    /// Show visible components on this gameobject.
    /// </summary>
    private void ShowSelf() {
      var sprite = GetComponent<SpriteRenderer>();
      if (sprite != null) {
        sprite.enabled = true;
      }

      var image = GetComponent<Image>();
      if (image != null) {
        image.enabled = true;
      }

      var text = GetComponent<TextMeshProUGUI>();
      if (text != null) {
        text.enabled = true;
      }
    }

    /// <summary>
    /// Show visible components on this gameobject and all descendents.
    /// </summary>
    private void ShowRecursive() {
      foreach (var child in GetComponentsInChildren<SpriteRenderer>(true)) {
        child.enabled = true;
      }

      foreach (var child in GetComponentsInChildren<Image>(true)) {
        child.enabled = true;
      }

      foreach (var child in GetComponentsInChildren<TextMeshProUGUI>(true)) {
        child.enabled = true;
      }
    }
    #endregion
  }
}