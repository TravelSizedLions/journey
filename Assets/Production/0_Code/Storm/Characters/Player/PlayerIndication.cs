using System.Collections.Generic;
using Storm.Attributes;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// A component for handling indicating different possible interactions to the player.
  /// </summary>
  public class PlayerIndication : MonoBehaviour {

    #region Fields
    /// <summary>
    /// The position indicators will be placed over the player.
    /// </summary>
    [SerializeField]
    [Tooltip("The position indicators will be placed over the player.")]
    private Vector3 indicatorPosition;

    /// <summary>
    /// The current indicator over the player's head.
    /// </summary>
    [ReadOnly]
    [Tooltip("The current indicator over the player's head.")]
    public GameObject CurrentIndicator;

    /// <summary>
    /// The list of indicators that the player can have over their head. This is
    /// a Unity Editor convenience. The real list is stored and accessed from
    /// the dictionary.
    /// </summary>
    [SerializeField]
    [Tooltip("The list of indicators that the player can have over their head.")]
    private List<GameObject> indicators;
    
    /// <summary>
    /// The list of indicators that the player can have over their head.
    /// </summary>
    private Dictionary<string, GameObject> indicatorCache;

    /// <summary>
    /// A reference to the player.
    /// </summary>
    private PlayerCharacter player;
    #endregion

    #region Unity API
    private void Awake() {
      if (indicatorPosition == null) {
        indicatorPosition = Vector3.zero;
      }

      if (indicators == null) {
        indicators = new List<GameObject>();
      }

      player = GetComponent<PlayerCharacter>();
      indicatorCache = new Dictionary<string, GameObject>();

      foreach (var obj in indicators) {
        if (!indicatorCache.ContainsKey(obj.name)) {
          indicatorCache.Add(obj.name, obj);
        }
      }
    }
    #endregion


    #region Public Interface
    /// <summary>
    /// Get the indicator with the given name.
    /// </summary>
    /// <param name="indicatorName">The name of the indicator to get.</param>
    /// <returns>An indicator prefab.</returns>
    private GameObject GetIndicator(string indicatorName) {
      if (!indicatorCache.ContainsKey(indicatorName)) {
        throw new UnityException("There is no player indicator named " + indicatorName + " in the list of indicators for the player.");
      }

      return indicatorCache[indicatorName];
    }

    /// <summary>
    /// Adds an indicator aboe the player's head.
    /// </summary>
    /// <param name="name">The name of the indicator prefab to add.</param>
    public void AddIndicator(string name) {
      if (indicatorCache.ContainsKey(name)) {
        GameObject indicator = GetIndicator(name);

        if (HasIndicator()) {
          Debug.Log("Replacing indicator " + CurrentIndicator.name + " with " + name);
          RemoveIndicator();
        }

        CurrentIndicator = Instantiate<GameObject>(
          indicator,
          player.transform.position + indicatorPosition,
          Quaternion.identity
        );

        CurrentIndicator.transform.parent = player.transform;
      }
    }

    /// <summary>
    /// Whether or not the player has an indicator above their head.
    /// </summary>
    /// <returns></returns>
    public bool HasIndicator() {
      return CurrentIndicator != null;
    }

    /// <summary>
    /// Removes the indicator above the player's head.
    /// </summary>
    public void RemoveIndicator() {
      if (HasIndicator()) {
        Destroy(CurrentIndicator);
        CurrentIndicator = null;
      }
    }

    /// <summary>
    /// Whether or not a particular indicator exists.
    /// </summary>
    /// <param name="name">The name of the indicator.</param>
    /// <returns>Whether or not the indicator exists.</returns>
    public bool IndicatorExists(string name) {
      return indicatorCache.ContainsKey(name);
    }
    #endregion
  }
}