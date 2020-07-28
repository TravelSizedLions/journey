using System.Collections.Generic;
using Storm.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace Storm.Flexible {

  /// <summary>
  /// An event trigger zone that waits until 1+ target objects have been placed within
  /// it, then performs a set of actions.
  /// </summary>
  public class DeliveryZone : MonoBehaviour {

    #region Fields 
    //---------------------------------------------------
    // Fields
    //---------------------------------------------------

    /// <summary>
    /// The names of target objects to track. The reason why this is a list of
    /// strings instead of Collider2Ds or GameObjects is because the objects the
    /// zone needs to check for may not necessarily start in the scene.
    /// </summary>
    [Tooltip("The names of target objects to track. The names should be unique, and the target objects don't necessarily have to start in the scene.")]
    public List<string> TargetObjects;

    [Space(10, order=2)]

    /// <summary>
    /// The actions to perform after all objects have been delivered to the zone.
    /// </summary>
    [Tooltip("The actions to perform after all objects have been delivered to the zone.")]
    public UnityEvent Actions; 

    /// <summary>
    /// Whether or not the target objects are currently in the zone. True -
    /// currently in the zone, False - outside the zone.
    /// </summary>
    [Tooltip("Whether or not the target objects are currently in the zone. True - currently in the zone, False - outside the zone.")]
    private Dictionary<string, bool> deliveryStatus;

    [Space(5, order=3)]
    [Header("Debug Information", order=4)]
    [Space(5, order=5)]

    /// <summary>
    /// Whether or not all tracked objects are inside.
    /// </summary>
    [Tooltip("Whether or not all tracked objects are inside.")]
    [SerializeField]
    [ReadOnly]
    private bool allInside;
    #endregion

    #region Unity API
    //---------------------------------------------------
    // Unity API
    //---------------------------------------------------

    private void Awake() {
      // Initialize dictionary. All targets marked as "outside the zone" at first.
      deliveryStatus = new Dictionary<string, bool>();
      foreach (string name in TargetObjects) {
        deliveryStatus.Add(name, false);
      }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
      // Debug.Log("Collision!! Object: " + collider.gameObject.name);
      string name = collider.gameObject.name;
      if (deliveryStatus.ContainsKey(name)) {
        // Debug.Log("Inside!!");
        deliveryStatus[name] = true;
      }

      if (AllInside()) {
        Actions.Invoke();
      }
    }

    private void OnTriggerExit2D(Collider2D collider) {
      string name = collider.gameObject.name;
      if (deliveryStatus.ContainsKey(name)) {
        deliveryStatus[name] = false;
      }
    }

    #endregion

    #region Helper Methods
    //---------------------------------------------------
    // Helper Methods
    //---------------------------------------------------

    /// <summary>
    /// Check whether or not all tracked items are inside the zone.
    /// </summary>
    /// <returns>True if all items are in the zone. False otherwise.</returns>
    private bool AllInside() {
      foreach (bool delivered in deliveryStatus.Values) {
        if (!delivered) {
          allInside = false;
          return false;
        }
      }

      allInside = true;
      return true;
    }

    #endregion
  }
}