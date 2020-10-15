using Storm.Flexible.Interaction;
using UnityEngine;

namespace Storm.Characters.Player {

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
      carriable.OnThrow();

      Vector3 playerHead = new Vector3(player.transform.position.x, player.transform.position.y + player.Collider.bounds.size.y);
      Vector3 direction = (player.GetMouseWorldPosition() - playerHead);
      direction.z = 0;
      direction = direction.normalized;

      Debug.Log("Direction of throw: " + direction);
      carriable.Physics.Velocity = direction*settings.ThrowingForce;

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
            Vector3 nudge = CalculateNudge(backHit, direction, carriable);
            carriable.transform.position = (Vector3)backHit.point + nudge;
            carriable.transform.position += CalculateNudge(backHit, direction, carriable);
            moved = true;
          }
        }

        nextPos = nextPos + carriable.Physics.Velocity;
      }
    }

    /// <summary>
    /// Determine how much the carriable needs to move to be out of the way of
    /// the player.
    /// </summary>
    /// <param name="hit">The collision with the player.</param>
    /// <param name="direction">The direction of the throw.</param>
    /// <param name="carriable">The carriable object to adjust.</param>
    /// <returns>The amount to translate the carriable by to avoid a collision
    /// with the player.</returns>
    private Vector3 CalculateNudge(RaycastHit2D hit, Vector3 direction, Carriable carriable) {
      // Get the angle of the negative velocity. We do this for the
      // negative velocity because it allows us to use the carriable collider's
      // extents as the adjacent length of the right triangle formed
      // from the velocity vector. The extents,
      // which are always known, naturally form the adjacent of the
      // triangle we need to solve in order to know how much to move
      // the object to get it out of the way of the player's collider.
      float angle = Mathf.Rad2Deg*Mathf.Atan2(-direction.y, -direction.x);
      
      // The adjacent angle will either be the vertical extent or
      // horizontal extent based on the angle of the throw.
      float adjacent = GetAdjacentLength(angle, carriable.Collider);

      // From here, we use some wonderful trigonometry to determine
      // how much the carriable object needs to keep moving in the
      // direction of travel to effectively "skip" hitting the player's hitbox
      // and take its intended trajectory.
      float opposite = adjacent*Mathf.Tan(angle);
      float hyp = Mathf.Sqrt((opposite*opposite) + (adjacent*adjacent));
      
      // The hypotenuse is the magnitude needed to get the collider out of the
      // way of the player.
      Vector3 nudge = direction*hyp;
      nudge.z = 0;

      return nudge;      
    }


    /// <summary>
    /// Get the adjacent length to use when calculating how to nudge the carriable
    /// outside of colliding with the player.
    /// </summary>
    /// <param name="angle">An angle between 0 and 180 (negatives not supported)</param>
    /// <param name="collider">The collider used to determine the length of the
    /// adjacent side.</param>
    /// <returns>The length of the side of the collider that is adjacent to the
    /// desired hypotenuse angle.</returns>
    private float GetAdjacentLength(float angle, Collider2D collider) {
      if (angle > 45f && angle < 135f) {
        return collider.bounds.extents.y;
      } else {
        return collider.bounds.extents.x;
      }
    }

    /// <summary>
    /// Drop the given item.
    /// </summary>
    /// <param name="carriable">The item to drop.</param>
    public void Drop(Carriable carriable) {
      carriable.OnPutDown();
      
      carriable.Physics.Vy = settings.DropForce.y;
      if (player.Facing == Facing.Right) {
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