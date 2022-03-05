using UnityEngine;

namespace HumanBuilders {
  public abstract class SnakeState : State {
    protected SnakePlayer player;
    protected SnakeMovement settings;
    protected IPhysics physics;



    /// <summary>
    /// Pre-hook called by the Player Character when a player state is first added to the player.
    /// </summary>
    public override void OnStateAddedGeneral() {
      player = GetComponent<SnakePlayer>();
      physics = player.Physics;

      if (player.SnakeSettings != null) {
        settings = player.SnakeSettings;
      } else {
        settings = GetComponent<SnakeMovement>();
      }
    }
  }
}