using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace HumanBuilders {

  /// <summary>
  /// Track a set of components on the object for whether or not they're enabled.
  /// </summary>
  [RequireComponent(typeof(GuidComponent))]
  public class SaveComponentEnabled : MonoBehaviour, IStorable {
    /// <summary>
    /// Which API call to perform an action on.
    /// </summary>
    public enum APICall { Awake, Start, Delay };

    #region Fields  
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// Which components to track.
    /// </summary>
    [ValueDropdown("Monobehaviors")]
    [Tooltip("Which components on this object to track the 'enabled' state for.")]
    public List<MonoBehaviour> ComponentsToTrack;

    /// <summary>
    /// When to restore the enable state of the component.
    /// </summary>
    [Tooltip("When to restore the enable state of the component. Awake - As the scene loads. Start - Just before the first frame.")]
    public APICall RestoreOn = APICall.Awake;

    /// <summary>
    /// How long to delay retrieving this information in seconds.
    /// </summary>
    [Tooltip("How long to delay retrieving this information in seconds.")]
    [ShowIf("ShowDelayField")]
    [SerializeField]
    private float delay = 0;

    /// <summary>
    /// A reference to the game object's global identifier.
    /// </summary>
    private GuidComponent guid;

    /// <summary>
    /// The list of keys that are used to store the behaviors' enabled state.
    /// </summary>
    private List<string> keys;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      guid = GetComponent<GuidComponent>();
      if (guid == null) {
        Debug.LogWarning("Game object \"" + name + "\" is missing a GUID component! Add one in the inspector.");
      }

      string keyBase = guid.ToString()+Keys.ENABLED;
      keys = new List<string>();

      foreach (MonoBehaviour comp in ComponentsToTrack) {
        keys.Add(keyBase+comp.GetType().ToString());
      }

      if (RestoreOn == APICall.Awake) {
        Retrieve();
      }
      
    }

    private void Start() {
      if (RestoreOn == APICall.Start) {
        Retrieve();
      } else if (RestoreOn == APICall.Delay) {
        new UnityTask(_Delay(delay));
      }
    }

    private void OnDestroy() {
      Store();
    }
    #endregion

    #region Storable API
    //-------------------------------------------------------------------------
    // Storable API
    //-------------------------------------------------------------------------
    /// <summary>
    /// Store whether or not each component on the object is enabled.
    /// </summary>
    public void Store() {
      List<bool> enabledList = new List<bool>();

      foreach (MonoBehaviour comp in ComponentsToTrack) {
        enabledList.Add(comp.enabled);
      }

      VSave.Set(StaticFolders.BEHAVIOR, keys, enabledList);
    }

    /// <summary>
    /// Retrieve whether or not each component on the object is enabled.
    /// </summary>
    public void Retrieve() {
      if (VSave.Get(StaticFolders.BEHAVIOR, keys, out List<bool> values)) {
        for (int i = 0; i < values.Count; i++) {
          ComponentsToTrack[i].enabled = values[i];
        }
      }
    }

    #endregion

    private IEnumerator _Delay(float seconds) {
      yield return new WaitForSeconds(seconds);
      Retrieve();
    }


    #region Odin Inspector Stuff
    //-------------------------------------------------------------------------
    // Odin Inspector
    //-------------------------------------------------------------------------

    /// <summary>
    /// Get the list of MonoBehaviours on the object.
    /// </summary>
    private IEnumerable Monobehaviors() {
      return new List<MonoBehaviour>(GetComponents<MonoBehaviour>());
    }

    private bool ShowDelayField() {
      return RestoreOn == APICall.Delay;
    }
    #endregion
  }
}