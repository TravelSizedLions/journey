using UnityEngine;

namespace HumanBuilders {

  public interface IThrowing {
    /// <summary>
    /// Throw the given item.
    /// </summary>
    /// <param name="carriable">The item to throw.</param>
    /// <seealso cref="ThrowingComponent.Throw" />
    void Throw(Carriable carriable);

    /// <summary>
    /// Drop the given item.
    /// </summary>
    /// <param name="carriable">The item to drop.</param>
    /// <seealso cref="ThrowingComponent.Drop" />
    void Drop(Carriable carriable);

    /// <summary>
    /// The direction the player would throw an item they may be holding.
    /// </summary>
    /// <param name="normalized">Whether or not the direction should be normalized.</param>
    /// <seealso cref="ThrowingComponent.GetThrowingDirection" />
    Vector2 GetThrowingDirection(bool normalized = true);

    /// <summary>
    /// The position that the player's throw would start.
    /// </summary>
    /// <seealso cref="ThrowingComponent.GetThrowingPosition" />
    Vector2 GetThrowingPosition();
  }

  public class ThrowingComponent : MonoBehaviour, IThrowing {

    /// <summary>
    /// The player's movement settings.
    /// </summary>
    private MovementSettings settings;

    /// <summary>
    /// A reference to the player character.
    /// </summary>
    private PlayerCharacter player;

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      settings = GetComponent<MovementSettings>();
      player = GetComponent<PlayerCharacter>();
    }

    #endregion


    #region Public Interface

    /// <summary>
    /// Throw the given item.
    /// </summary>
    /// <param name="carriable">The item to throw.</param>
    public void Throw(Carriable carriable) {
      var guide = player.GetComponentInChildren<ThrowingGuide>(true);
      if (guide.ThrowingStrength < 0.33f) {
        Drop(carriable);
        return;
      }


      carriable.OnThrow();

      Vector3 playerHead = new Vector3(player.transform.position.x, player.transform.position.y + player.Collider.bounds.size.y);
      Vector3 direction = (player.GetMouseWorldPosition() - playerHead);
      direction.z = 0;
      direction = direction.normalized;

      carriable.Physics.Velocity = direction*settings.ThrowingForce*guide.ThrowingStrength;

      // Check if object will collide w/ player. If it does, transport it through
      // character.
      if (direction.y < 0) {
        NudgeThrow(carriable, direction);
      }
    }

    /// <summary>
    /// Try to nudge a downward pointing throw out of the player's way.
    /// </summary>
    /// <param name="carriable">The carriable being thrown.</param>
    /// <param name="direction">The normalized direction of the throw.</param>
    private void NudgeThrow(Carriable carriable, Vector3 direction) {
      // Raycast to check if the carriable would collide with the player. In
      // this case, we need to nudge the box out of the way to prevent it from
      // effecting the player's physics.
      Vector2 pos = carriable.Collider.transform.position;
      RaycastHit2D[] hits = Physics2D.RaycastAll(pos, direction, carriable.Physics.Velocity.magnitude);
      foreach (RaycastHit2D hit in hits) {
        if (hit.collider.CompareTag("Player") && player.IsHitBy(carriable.Collider)) {
          CalculateBackCast(carriable, direction);
        }
      }
    }

    /// <summary>
    /// Project the carriable object forward, then calculate where the object would
    /// come out from the player's hitbox and apply it to the carriable's position.
    /// </summary>
    /// <param name="carriable">The carriable to check.</param>
    /// <param name="direction">The normalized direction of the throw.</param>
    private void CalculateBackCast(Carriable carriable, Vector3 direction) {
      Vector2 nextPos = (Vector2)carriable.Collider.transform.position + carriable.Physics.Velocity;
      bool moved = false;
      while (!moved) {

        // project raycast backwards to find where the carriable would
        // exit the player's hitbox.
        RaycastHit2D[] backHits = Physics2D.RaycastAll(nextPos, -direction, carriable.Physics.Velocity.magnitude);
        foreach (RaycastHit2D backHit in backHits) {

          if (backHit.collider.CompareTag("Player") && player.IsHitBy(carriable.Collider)) {
            carriable.transform.position = (Vector3)backHit.point;

            moved = true;
          }
        }

        nextPos = nextPos + carriable.Physics.Velocity;
      }
    }

    /// <summary>
    /// Drop the given item.
    /// </summary>
    /// <param name="carriable">The item to drop.</param>
    public void Drop(Carriable carriable) {
      carriable.OnPutDown();
      Vector3 playerHead = new Vector3(player.transform.position.x, player.transform.position.y + player.Collider.bounds.size.y);
      Vector3 direction = (player.GetMouseWorldPosition() - playerHead);

      carriable.Physics.Vy = settings.DropForce.y;
      if (direction.x > 0) {
        carriable.Physics.Vx = settings.DropForce.x;
      } else {
        carriable.Physics.Vx = -settings.DropForce.x;
      }
    }

    /// <summary>
    /// The direction the player would throw an item they may be holding.
    /// </summary>
    /// <param name="normalized">Whether or not the direction should be normalized.</param>
    public Vector2 GetThrowingDirection(bool normalized = true) {
      Vector2 direction = ((Vector2)player.GetMouseWorldPosition() - GetThrowingPosition());
      return normalized ? direction.normalized : direction;
    }


    /// <summary>
    /// The position that the player's throw would start.
    /// </summary>
    public Vector2 GetThrowingPosition() {
      return ((Vector2)(player.transform.position + new Vector3(0, player.Collider.bounds.size.y)));
    }
    #endregion

  }

}