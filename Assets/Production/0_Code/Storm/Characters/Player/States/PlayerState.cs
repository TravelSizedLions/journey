using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Storm.Components;
using Storm.Subsystems.FSM;
using Storm.Flexible.Interaction;

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
    protected IPhysics physics;
    #endregion


    /// <summary>
    /// Injection point for state dependencies.
    /// </summary>
    public void Inject(IPlayer player, IPhysics physics) {
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


    public bool CanCarry(GameObject obj) {
      Interactible interactible = obj.GetComponent<Interactible>();
      return (interactible != null) && (interactible is Carriable);
    }

    public void DropItem(Carriable item) {
      item.OnPutDown();
      
      CarrySettings settings = GetComponent<CarrySettings>();
      if (player.HoldingUp()) {
        item.Physics.Vy = settings.VerticalThrowForce;
      } else {
        if (player.Facing == Facing.Right) {
          item.Physics.Vx = settings.DropForce.x;
        } else {
          item.Physics.Vx = -settings.DropForce.x;
        }
      }
    }

    public void ThrowItem(Carriable item) {
      item.OnThrow();
      item.Physics.Velocity = player.Physics.Velocity;

      CarrySettings settings = GetComponent<CarrySettings>();
      if (player.HoldingUp()) {
        item.Physics.Vy = settings.VerticalThrowForce + player.Physics.Vy;
      } else {
        if (player.Facing == Facing.Right) {
          item.Physics.Vx += settings.ThrowForce.x;
        } else {
          item.Physics.Vx -= settings.ThrowForce.x;
        }
        item.Physics.Vy += settings.ThrowForce.y;
      }
    }
  }

}