using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {
  public class LostOrphan : Collectible {
    //-------------------------------------------------------------------------
    // Variables
    //-------------------------------------------------------------------------
    /// <summary>
    /// The name of the orphan. This name should be unique among orphans.
    /// </summary>
    [Tooltip("The name of the orphan. This name should be unique among orphans.")]
    public string Name = "";

    /// <summary>
    /// The list of other behaviors on this object.
    /// </summary>
    private List<Behaviour> behaviours;

    /// <summary>
    /// The list of renderers (like SpriteRenderer) on this object.
    /// </summary>
    private List<Renderer> renderers;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      if (!string.IsNullOrEmpty(Name)) {
        if (VSave.Get(StaticFolders.ORPHANS, Name, out bool collected) && collected) {
          base.OnCollected();
          DisableComponents();
        }
      } else {
        Debug.LogWarning("Lost orphan \"" + name + "\" is missing the name property!");
      }

      behaviours = new List<Behaviour>();
      renderers = new List<Renderer>();
      foreach (Behaviour behavior in GetComponents<Behaviour>()) {
        if (behavior != this) {
          behaviours.Add(behavior);
        }
      }

      foreach (Renderer renderer in GetComponents<Renderer>()) {
        if (renderer != this) {
          renderers.Add(renderer);
        }
      }
    }

    //-------------------------------------------------------------------------
    // Collectible API
    //-------------------------------------------------------------------------
    public override void OnCollected() {
      base.OnCollected();
      if (!string.IsNullOrEmpty(Name)) {
        VSave.Set(StaticFolders.ORPHANS, Name, true);
        DisableComponents();
      } else {
        Debug.LogWarning("Lost orphan \"" + name + "\" is missing the name property! Collection not saved!");
      }
    }

    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    public void EnableComponents() {
      foreach (Behaviour behaviour in behaviours) {
        behaviour.enabled = true;
      }

      foreach (Renderer renderer in renderers) {
        renderer.enabled = true;
      }
    }

    public void DisableComponents() {
      foreach (Behaviour behaviour in behaviours) {
        behaviour.enabled = false; 
      }
      
      foreach (Renderer renderer in renderers) {
        renderer.enabled = false;
      }
    }
  }
}