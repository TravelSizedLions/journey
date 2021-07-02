using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {
  public class LostPerson : Collectible {

    //-------------------------------------------------------------------------
    // Variables
    //-------------------------------------------------------------------------
    /// <summary>
    /// The name of the orphan. This name should be unique.
    /// </summary>
    [Tooltip("The name of the person. This name should be unique.")]
    public string Name = "";

    /// <summary>
    /// The key that this person is saved to.
    /// </summary>
    protected string key = "";

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
    protected virtual void Awake() {
      key = string.IsNullOrEmpty(key) ? Name : key;

      if (!string.IsNullOrEmpty(key)) {
        if (VSave.Get(StaticFolders.PEOPLE, key, out bool collected) && collected) {
          base.OnCollected();
          DisableComponents();
        }
      } else {
        Debug.LogWarning("Lost person \"" + name + "\" is missing the name property!");
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
      if (!string.IsNullOrEmpty(key)) {
        VSave.Set(StaticFolders.PEOPLE, key, true);
        DisableComponents();
      } else {
        Debug.LogWarning("Lost person \"" + name + "\" is missing the name property! Collection not saved!");
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