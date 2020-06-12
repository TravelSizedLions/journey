using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Storm.Components;
using Storm.Subsystems.FSM;

namespace Storm.Characters.Player {

  /// <summary>
  /// The base class for player states.
  /// </summary>
  [RequireComponent(typeof(PlayerCharacter))]
  public abstract class PlayerState : State {

    #region Fields

    /// <summary>
    /// A reference to the player character.
    /// </summary>
    protected IPlayer player;

    /// <summary>
    /// Information about the player's physics.
    /// </summary>
    protected IPhysicsComponent physics;
    #endregion


    /// <summary>
    /// Injection point for state dependencies.
    /// </summary>
    public void Inject(IPlayer player, IPhysicsComponent physics) {
      this.player = player;
      this.physics = physics;
    }

    /// <summary>
    /// Pre-hook called by the Player Character when a player state is first added to the player.
    /// </summary>
    public override void OnStateAddedGeneral() {
      player = GetComponent<PlayerCharacter>();
      physics = player.Physics;
    }
  }

}