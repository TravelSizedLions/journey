using Storm.Attributes;
using UnityEngine;

namespace Storm.Flexible.Interaction {
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

    public Vector2 Offset {
      get { return (Vector2)transform.localPosition; }
    }
  }
}