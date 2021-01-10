using System.Collections;
using System.Collections.Generic;

using UnityEngine;


namespace HumanBuilders {
  /// <summary>
  /// A behavior for storing whether certain objects are enabled. Unfortunately,
  /// due to limitations in the Unity framework, an object cannot track its own
  /// Active state, and cannot track other states if the object this script is
  /// on is disabled.
  /// </summary>
  public class SaveObjectEnabled : MonoBehaviour, IStorable {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// Often to poll for whether or not the objects are active (in seconds).
    /// </summary>
    private const float POLLING_TIME = 0.1f;

    /// <summary>
    /// The list of game objects to track.
    /// </summary>
    [Tooltip("The list of game objects to track.")]
    public List<GuidReference> ObjectsToTrack;        // For the inspector.
    private Dictionary<string, bool> trackedObjects;  // For runtime.
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Start() {
      Retrieve();

      trackedObjects = new Dictionary<string, bool>();
      foreach (GuidReference guid in ObjectsToTrack) {
        string key = guid.ToString()+Keys.ACTIVE;
        if (guid.gameObject == null) {
          Debug.Log("Object for " + guid.ToString() + " was null.");
        }
        bool value = guid.gameObject.activeSelf;
        trackedObjects.Add(key, value);
      }

      StartCoroutine(_Poll());
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
    /// Retrieve the active status of the list of game objects.
    /// </summary>
    public void Retrieve() {
      foreach (GuidReference guid in ObjectsToTrack) {
        RetrieveObject(guid);
      }
    }

    /// <summary>
    /// Retrieve the active status of a single game object.
    /// </summary>
    /// <param name="guid">the global ID of the game object to load.</param>
    private void RetrieveObject(GuidReference guid) {
      string key = guid.ToString()+Keys.ACTIVE;
      if (VSave.Get(StaticFolders.BEHAVIOR, key, out bool value)) {
        guid.gameObject.SetActive(value);
      }
    }

    /// <summary>
    /// Store the active status of the list of game objects.
    /// </summary>
    public void Store() {
      foreach (string key in trackedObjects.Keys) {
        StoreObject(key);
      }
    }


    /// <summary>
    /// Store the active status of a single game object.
    /// </summary>
    /// <param name="guid">The global ID of the game object to store.</param>
    private void StoreObject(string key) {
      VSave.Set(StaticFolders.BEHAVIOR, key, trackedObjects[key]);
    }


    /// <summary>
    /// Check the active status of the tracked objects every so often.
    /// We do this because OnDestroy isn't strictly ordered, so the game objects 
    /// we track may already be finalized by then.
    /// </summary>
    private IEnumerator _Poll() {
      while (gameObject != null) {
        yield return new WaitForSecondsRealtime(0.1f);
        foreach (GuidReference guid in ObjectsToTrack) {
          if (guid.gameObject != null) {
            string key = guid.ToString()+Keys.ACTIVE;
            trackedObjects[key] = guid.gameObject.activeSelf;
          }
        }
      }
    }
    #endregion
  }

}