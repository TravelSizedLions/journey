using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  public class Following : MonoBehaviour {

    /// <summary>
    /// How fast the object accelerates to max speed.
    /// </summary>
    [Tooltip("How fast the object accelerates to max speed.")]
    [SerializeField]
    private float acceleration = 0.5f;

    /// <summary>
    /// How fast the object accelerates to max speed.
    /// </summary>
    [Tooltip("How fast the object deccelerates to zero.")]
    [SerializeField]
    private float deceleration = 0.5f;

    /// <summary>
    /// The max speed that the object follows the player.
    /// </summary>
    [Tooltip("The speed that the object follows the player.")]
    [SerializeField]
    private float speed = 2f;

    /// <summary>
    /// Whether or not the object should start out moving towards the player.
    /// </summary>
    [Tooltip("Whether or not the object should start out moving towards the player.")]
    [SerializeField]
    private bool moveOnAwake = false;

    /// <summary>
    /// Which layers are allowed collisions for the non-trigger collider.
    /// </summary>
    [Tooltip("Which layers are allowed collisions for the non-trigger collider.")]
    [SerializeField]
    private LayerMask mask = default(LayerMask);

    /// <summary>
    /// Whether or not the object is moving towards the player.
    /// </summary>
    [Tooltip("Whether or not the object is moving towards the player.")]
    [SerializeField]
    [ReadOnly]
    private bool moving = false;

    /// <summary>
    /// A reference to the player character
    /// </summary>
    private IPlayer player;

    /// <summary>
    /// The animator of the object.
    /// </summary>
    private Animator anim;

    /// <summary>
    /// The object's physics.
    /// </summary>
    private PhysicsComponent physics;

    /// <summary>
    /// The object's collisions.
    /// </summary>
    private CollisionComponent collision;

    private void Awake() {
      moving = moveOnAwake;
      player = GameManager.Player;
      physics = GetComponentInChildren<PhysicsComponent>(true);
      anim = GetComponent<Animator>();

      Collider2D[] cols = GetComponents<Collider2D>();
      foreach (Collider2D col in cols) {
        if (!col.isTrigger) {
          collision = new CollisionComponent(col);
          break;
        }
      }
    }

    private void Update() {
      if (moving) {
        Facing facing = GetDirection();
        float facingScale = (float)facing;
        float accel = acceleration*facingScale;
        physics.Vx += accel;
        physics.Vx = Mathf.Clamp(physics.Vx, -speed, speed);

        if (IsStoppedByBarrier(facing)) {
          physics.Vx = 0;
          anim.SetBool("bool", false);
        } else {
          anim.SetBool("bool", true);
        }
      } else if (physics.IsMovingHorizontally()){
        float dir = Mathf.Sign(physics.Vx);
        physics.Vx -= (deceleration*dir);

        if (Mathf.Sign(physics.Vx) != dir) {
          physics.Vx = 0;
          anim.SetBool("bool", false);

        }
      }
    }

    private bool IsStoppedByBarrier(Facing facing) {
      // return (
      //   (facing == Facing.Left && (collision.IsTouchingLeftWall() || collision.IsOverLeftLedge())) ||
      //   (facing == Facing.Right && (collision.IsTouchingRightWall() || collision.IsOverRightLedge()))
      // );
      return (
        (facing == Facing.Left && (collision.IsTouchingLeftWall())) ||
        (facing == Facing.Right && (collision.IsTouchingRightWall()))
      );
    }

    private Facing GetDirection() {
      return (transform.position.x > player.Physics.Px) ? Facing.Left : Facing.Right;
    }


    private void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        moving = false;
      }
    }

    private void OnTriggerExit2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        moving = true;
      }
    }
  }
}