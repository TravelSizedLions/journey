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

    /// <summary>
    /// Settings about the player's movement.
    /// </summary>
    protected MovementSettings settings;
    #endregion


    /// <summary>
    /// Injection point for state dependencies.
    /// </summary>
    public void Inject(IPlayer player, IPhysics physics, MovementSettings settings) {
      this.player = player;
      this.physics = physics;
      this.settings = settings;
    }


    /// <summary>
    /// Pre-hook called by the Player Character when a player state is first added to the player.
    /// </summary>
    public override void OnStateAddedGeneral() {
      player = GetComponent<PlayerCharacter>();
      physics = player.Physics;
      if (player.MovementSettings) {
        settings = player.MovementSettings;
      } else {
        settings = GetComponent<MovementSettings>();
      }
      
    }

    /// <summary>
    /// Whether or not the player can carry this game object.
    /// </summary>
    /// <param name="obj">The object to check.</param>
    /// <returns>True if the object can be carried. False otherwise.</returns>
    public bool CanCarry(GameObject obj) {
      Interactible interactible = obj.GetComponent<Interactible>();
      return (interactible != null) && (interactible is Carriable);
    }

    public void DropItem(Carriable item) {
      item.OnPutDown();
      
      if (player.HoldingUp()) {
        item.Physics.Vy = settings.VerticalThrowForce;
      } else {
        item.Physics.Vy = settings.DropForce.y;
        if (player.Facing == Facing.Right) {
          item.Physics.Vx = settings.DropForce.x;
        } else {
          item.Physics.Vx = -settings.DropForce.x;
        }
      }
    }

    public void ThrowItem(Carriable item) {
      item.OnThrow();
      
      Debug.Log("player vel: " + player.Physics.Velocity);
      item.Physics.Velocity = player.Physics.Velocity;

      if (player.HoldingUp()) {
        item.Physics.Vy += settings.VerticalThrowForce;
      } else {
        if (player.Facing == Facing.Right) {
          item.Physics.Vx += settings.ThrowForce.x;
        } else {
          item.Physics.Vx -= settings.ThrowForce.x;
        }
        item.Physics.Vy += settings.ThrowForce.y;
      }
    }


    public Facing ProjectToWall() {
      float distToRight = player.DistanceToRightWall();
      float distToLeft = player.DistanceToLeftWall();
      
      Facing whichWall;

      if (distToLeft < distToRight) {
        whichWall = Facing.Left;
        // player.Physics.Px -= distToLeft;
        
      } else {
        whichWall = Facing.Right;
        // player.Physics.Px += distToRight;
      }

      return whichWall;
    }
  }

}