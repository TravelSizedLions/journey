using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// A collection of settings for player interaction.
  /// </summary>
  public class InteractionSettings : MonoBehaviour {

    /// <summary>
    /// The radius the player uses to check for interactive objects when the
    /// player isn't moving.
    /// </summary>
    [Tooltip("The radius the player uses to check for interactive objects when the player isn't moving.")]
    public float IdleInteractionRadius;

    /// <summary>
    /// The radius the player uses to check for interactive objects when the
    /// player is moving.
    /// </summary>
    [Tooltip("The radius the player uses to check for interactive objects when the player is moving.")]
    public float MovingInteractionRadius;
  }
}