using UnityEngine;


namespace HumanBuilders {
  [RequireComponent(typeof(GuidComponent))]
  public class SavePosition : MonoBehaviour, IStorable {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// A reference to the game objects unique ID.
    /// </summary>
    GuidComponent guid;
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

    public void Store() {
      string folder = StaticFolders.ANIMATION;
      string key = guid.ToString()+Keys.POSITION;
      float[] values = new float[] { transform.position.x, transform.position.y, transform.position.z };
    
      VSave.Set(folder, key, values);
    }

    public void Retrieve() {
      string folder = StaticFolders.ANIMATION;
      string key = guid.ToString()+Keys.POSITION;
      if (VSave.Get(folder, key, out float[] values)) {
        transform.position = new Vector3(values[0], values[1], values[2]);
      }
    }
    #endregion
  }
}