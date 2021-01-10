using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using SubjectNerd.Utilities;
using Sirenix.OdinInspector;

namespace HumanBuilders {

  /// <summary>
  /// An event trigger zone that waits until 1+ target objects have been placed within
  /// it, then performs a set of actions.
  /// </summary>
  public class DeliveryZone : MonoBehaviour {

    #region Fields 
    //---------------------------------------------------
    // Fields
    //---------------------------------------------------

    [Space(10, order=1)]
    /// <summary>
    /// Whether or not the events should fire every time that all target objects
    /// are in the zone. (i.e., if you move an object inside the zone, then back
    /// outside the zone, there)
    /// </summary>
    [Tooltip("Whether or not the events should fire every time that all target objects are in the zone.\n\nFor example if you move an object outside the zone then back inside the zone, the same events will fire again.")]
    public bool FireEveryTime;
    [Space(10, order=2)]


    /// <summary>
    /// The names of target objects to track. The reason why this is a list of
    /// strings instead of Collider2Ds or GameObjects is because the objects the
    /// zone needs to check for may not necessarily start in the scene.
    /// </summary>
    [Tooltip("The names of target objects to track. The names should be unique, and the target objects don't necessarily have to start in the scene.")]
    [Reorderable]
    public List<GuidReference> TargetObjects;

    [Space(10, order=3)]

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

    [Space(5, order=4)]
    [Header("Debug Information", order=5)]
    [Space(5, order=6)]

    /// <summary>
    /// Whether or not all tracked objects are inside the delivery zone.
    /// </summary>
    [Tooltip("Whether or not all tracked objects are inside the delivery zone.")]
    [SerializeField]
    [ReadOnly]
    private bool allInside;

    /// <summary>
    /// Whether or not the events have fired.
    /// </summary>
    [Tooltip("Whether or not the events have fired.")]
    [SerializeField]
    [ReadOnly]
    private bool fired;

    #endregion

    #region Unity API
    //---------------------------------------------------
    // Unity API
    //---------------------------------------------------

    private void Awake() {
      fired = false;

      deliveryStatus = new Dictionary<string, bool>();
      foreach (GuidReference guid in TargetObjects) {
        deliveryStatus.Add(guid.ToString(), false);
      }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
      if (enabled) {
        GuidComponent guid = GetObjectReference(collider);
        string key = guid != null ? guid.ToString() : "";

        if (guid != null && deliveryStatus.ContainsKey(key)) {
          deliveryStatus[key] = true;
          CheckAllInside();

          if (allInside && (!fired || FireEveryTime)) {
            fired = true;
            Actions.Invoke();
          }
        }
      }
    }

    private GuidComponent GetObjectReference(Collider2D collider) {
      if (collider.CompareTag("Player")) {
        PlayerCharacter player = collider.GetComponent<PlayerCharacter>();
        Carriable carriable = player.CarriedItem; 

        if (carriable != null) {
          return carriable.GetComponentInChildren<GuidComponent>();
        } 

      } else {
        return collider.transform.root.GetComponentInChildren<GuidComponent>();
      }

      return null;
    }

    private void OnTriggerExit2D(Collider2D collider) {
      if (enabled) {
        string name = collider.gameObject.name;
        if (deliveryStatus.ContainsKey(name)) {
          deliveryStatus[name] = false;
          CheckAllInside();
        }
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
    private void CheckAllInside() {
      foreach (bool delivered in deliveryStatus.Values) {
        if (!delivered) {
          allInside = false;
          return;
        }
      }

      allInside = true;
    }

    #endregion
  }
}