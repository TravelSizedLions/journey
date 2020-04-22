using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using UnityEngine;

/// <summary>
/// This class defines the common API for the Live Wire game mechanic. Live Wire movement is a type of player behavior where the player is flung around as a spark of energy.
/// </summary>
/// <seealso cref="PlayerBehavior" />
public abstract class LivewireMovement : PlayerBehavior {

  /// <summary>
  /// Set the direction of movement for the player.
  /// </summary>
  /// <param name="direction">The direction the player should move.</param>
  public abstract void SetDirection(Vector2 direction);

}