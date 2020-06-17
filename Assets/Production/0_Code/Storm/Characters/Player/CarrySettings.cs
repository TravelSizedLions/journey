using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// A collection of settings for player movement. This class is meant to expose movement parameters to the Unity Editor.
  /// </summary>
  public class CarrySettings : MonoBehaviour {

    /// <summary>
    /// How quickly the character moves while carrying something.
    /// </summary>
    [Tooltip("How quickly the character moves while carrying something.")]
    public float MaxCarrySpeed = 24f;

    /// <summary>
    /// The strength of the player's jump when carrying something.
    /// </summary>
    [Tooltip("The strength of the player's jump when carrying something.")] 
    public float JumpForce = 50f;

    
    /// <summary>
    /// The force that nudges an item a certain direction when it's dropped.
    /// </summary>
    [Tooltip("The force that nudges an item a certain direction when it's dropped.")]
    public Vector2 DropForce;

    /// <summary>
    /// The force that carried items are throw with.
    /// </summary>
    [Tooltip("The force that carried items are throw with.")]
    public Vector2 ThrowForce;

    /// <summary>
    /// The force that carried items are throw upwards with.
    /// </summary>
    [Tooltip("The force that carried items are throw upwards with.")]
    public float VerticalThrowForce;
  }
}