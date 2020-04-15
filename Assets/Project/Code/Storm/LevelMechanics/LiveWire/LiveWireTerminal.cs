using Storm.Characters.Player;
using UnityEngine;


namespace Storm.LevelMechanics.LiveWire {
  public enum Direction { Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft }

  public static class Directions2D {
    public static readonly Vector2 Up = Vector2.up;
    public static readonly Vector2 UpRight = new Vector2(1, 1).normalized;
    public static readonly Vector2 Right = Vector2.right;
    public static readonly Vector2 DownRight = new Vector2(1, -1).normalized;
    public static readonly Vector2 Down = Vector2.down;
    public static readonly Vector2 DownLeft = new Vector2(-1, -1).normalized;
    public static readonly Vector2 Left = Vector2.left;
    public static readonly Vector2 UpLeft = new Vector2(-1, 1).normalized;


    public static Vector2 toVector(Direction d) {
      switch (d) {
        case Direction.Up:
          return Directions2D.Up;
        case Direction.UpRight:
          return Directions2D.UpRight;
        case Direction.Right:
          return Directions2D.Right;
        case Direction.DownRight:
          return Directions2D.DownRight;
        case Direction.Down:
          return Directions2D.Down;
        case Direction.DownLeft:
          return Directions2D.DownLeft;
        case Direction.Left:
          return Directions2D.Left;
        case Direction.UpLeft:
          return Directions2D.UpLeft;
        default:
          return Directions2D.Up;
      }
    }

    /// <summary>
    /// Checks if two vectors are going roughly opposite directions
    /// </summary>
    /// <param name="v1">the first vector</param>
    /// <param name="v2">the second vector</param>
    /// <returns>true if the vectors are pointing roughly opposite directions (i.e. their dot product is less that 0).</returns>
    public static bool areOppositeDirections(Vector2 v1, Vector2 v2) {
      return Vector2.Dot(v1, v2) < 0;
    }

    /// <summary>
    /// Returns the angle between two vectors in degrees.
    /// </summary>
    /// <param name="v1">the first vector</param>
    /// <param name="v2">the second vector</param>
    /// <returns></returns>
    public static float angleBetween(Vector2 v1, Vector2 v2) {
      float cosTheta = Vector2.Dot(v1, v2) / (v1.magnitude * v2.magnitude);
      float thetaRads = Mathf.Acos(cosTheta);
      return Mathf.Rad2Deg * thetaRads;
    }

    public static Vector2 reverseDirection(Vector2 d) {
      return new Vector2(-d.x, -d.y);
    }
  }


  /**
    When Jerrod runs into one of these during a Mainframe Stage,
    he turns into a ball of pure electricity and fires off through a wire
    to another part of the stage. This is "Live Wire" mode.
  */
  public class LiveWireTerminal : MonoBehaviour {

    #region Members

    /*The selected direction*/
    public Direction motionDirection;

    public Direction exitDirection;

    private Vector2 direction;

    private BoxCollider2D boxCollider;

    private float disableTimer;

    private float delay = 0.25f;

    #endregion

    #region Unity Methods

    public void Awake() {
      direction = Directions2D.toVector(motionDirection);
      boxCollider = GetComponent<BoxCollider2D>();
    }


    public void Update() {
      if (!boxCollider.enabled) {
        disableTimer -= Time.deltaTime;
        if (disableTimer < 0) {
          boxCollider.enabled = true;
        }
      }
    }

    public void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        PlayerCharacter player = GameManager.Instance.player;
        player.transform.position = transform.position;
        disableTimer = delay;
        boxCollider.enabled = false;

        // Transition to or from LiveWire mode.
        Debug.Log("Direction: " + direction + ", velocity: " + player.rb.velocity);
        float angleBetween = Directions2D.angleBetween(direction, player.rb.velocity);
        if (player.directedLiveWireMovement.enabled ||
          (player.ballisticLiveWireMovement.enabled && (angleBetween > 135 && angleBetween < 225))) {

          Debug.Log("To Mainframe");

          player.SwitchBehavior(PlayerBehaviorEnum.Normal);

          player.normalMovement.DisableFastDeceleration();

          Vector2 dir = Directions2D.toVector(exitDirection);
          Vector2 vel = player.rb.velocity;

          player.rb.velocity = vel.magnitude * dir;

          player.normalMovement.hasJumped = true;
          player.normalMovement.canDoubleJump = true;

        } else {
          Debug.Log("To LiveWire");
          player.SwitchBehavior(PlayerBehaviorEnum.DirectedLiveWire);

          player.directedLiveWireMovement.SetDirection(direction);
        }
      }
    }


    public void OnTriggerExit2D(Collider2D col) {

    }
    #endregion

  }
}