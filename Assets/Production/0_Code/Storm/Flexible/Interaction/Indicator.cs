using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// Animated UI element for indicating what an object does when you interact with it.
  /// </summary>
  public class Indicator : MonoBehaviour {
    
    /// <summary>
    /// The name of the indicator.
    /// </summary>
    [Tooltip("The name of the indicator.")]
    public string Name;

    /// <summary>
    /// Whether or not the player has been instantiated in the scene.
    /// </summary>
    [Tooltip("Whether or not the player has been instantiated in the scene.")]
    [ReadOnly]
    public bool Instantiated;

    /// <summary>
    /// The indicator's position relative to its parent.
    /// </summary>
    public Vector2 Offset {
      get { return (Vector2)transform.localPosition; }
    }
  }
}
