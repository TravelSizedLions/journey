using Storm.Flexible.Interaction;
using Storm.Subsystems.FSM;
using UnityEngine;


namespace Storm.Characters.Player {

  public interface IInteractionComponent {
    Interactible CheckForInteractible(Vector2 playerPosition, bool running);

    void TryInteract();
  }

  public class InteractionComponent : IInteractionComponent {

    #region Fields
    private LayerMask layerMask;

    private InteractionSettings settings;

    private PlayerCharacter player;

    private FiniteStateMachine fsm;

    #endregion

    #region Constructors
    public InteractionComponent(PlayerCharacter player, FiniteStateMachine fsm, InteractionSettings settings) {
      layerMask = LayerMask.GetMask(new string[] {"Interactive", "Non-Player-Collision", "NPCs"});

      this.settings = settings;
      this.player = player;
      this.fsm = fsm;
    }
    #endregion

    #region Interface API

    /// <summary>
    /// Check if there's an interactive object close by.
    /// </summary>
    /// <param name="playerPosition">The player's position.</param>
    /// <param name="running">Whether or not the player is running.</param>
    /// <returns>The closest interactible object within a limited radius. If
    /// nothing interactive is close to the player, this returns null.</returns>
    public Interactible CheckForInteractible(Vector2 playerPosition, bool running) {

      float radius = running ? settings.MovingInteractionRadius : settings.IdleInteractionRadius;
      Collider2D[] hits = Physics2D.OverlapCircleAll(playerPosition, radius, layerMask);

      return GetClosest(playerPosition, hits);
    }


    /// <summary>
    /// Finds the closest interactive object to a position.
    /// </summary>
    /// <param name="position">The position to check with.</param>
    /// <param name="hits">The list of colliders for close-by interactive objects.</param>
    /// <returns>The closest interactive object in the list.</returns>
    private Interactible GetClosest(Vector2 position, Collider2D[] hits) {
      if (hits.Length == 0) {
        return null;
      }

      Collider2D closest = hits[0];
      float closestDist = (position - (Vector2)closest.transform.position).magnitude;

      foreach (Collider2D hit in hits) {
        float dist = (position - (Vector2)hit.transform.position).magnitude;
        if (dist < closestDist) {
          dist = closestDist;
        }
      }

      return closest.GetComponent<Interactible>();
    }

    public void TryInteract() {
      Interactible interactible = CheckForInteractible(player.Center, player.Physics.Velocity.magnitude > 0);
      if (interactible != null) {
        interactible.OnInteract();

        fsm.Signal(interactible.gameObject);
      }
    }

    #endregion
  }
}