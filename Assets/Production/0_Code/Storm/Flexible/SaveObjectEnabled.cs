using System.Collections;
using System.Collections.Generic;
using Storm.Subsystems.Save;
using UnityEngine;


namespace Storm.Flexible {
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
    /// The list of game objects to track.
    /// </summary>
    [Tooltip("The list of game objects to track.")]
    public List<GuidReference> ObjectsToTrack;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      Retrieve();
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
      foreach (GuidReference guid in ObjectsToTrack) {
        StoreObject(guid);
      }
    }


    /// <summary>
    /// Store the active status of a single game object.
    /// </summary>
    /// <param name="guid">The global ID of the game object to store.</param>
    private void StoreObject(GuidReference guid) {
      string key = guid.ToString()+Keys.ACTIVE;
      bool value = guid.gameObject.activeSelf;

      VSave.Set(StaticFolders.BEHAVIOR, key, value);
    }

    #endregion
  }

}