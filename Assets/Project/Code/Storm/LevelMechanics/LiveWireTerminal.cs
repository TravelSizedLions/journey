using UnityEngine;


using Storm.Characters.Player;


namespace Storm.LevelMechanics {
    public enum Direction { Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft }

    public static class Directions2D {
        public static readonly Vector2 Up = Vector2.up;
        public static readonly Vector2 UpRight = new Vector2(1,1).normalized;
        public static readonly Vector2 Right = Vector2.right;
        public static readonly Vector2 DownRight = new Vector2(1,-1).normalized;
        public static readonly Vector2 Down = Vector2.down;
        public static readonly Vector2 DownLeft = new Vector2(-1,-1).normalized;
        public static readonly Vector2 Left = Vector2.left;
        public static readonly Vector2 UpLeft = new Vector2(-1,1).normalized;


        public static Vector2 toVector(Direction d) {
            switch (d) {
                case Direction.Up:        return Directions2D.Up;
                case Direction.UpRight:   return Directions2D.UpRight;
                case Direction.Right:     return Directions2D.Right;
                case Direction.DownRight: return Directions2D.DownRight;
                case Direction.Down:      return Directions2D.Down;
                case Direction.DownLeft:  return Directions2D.DownLeft;
                case Direction.Left:      return Directions2D.Left;
                case Direction.UpLeft:    return Directions2D.UpLeft;
                default:                  return Directions2D.Up;
            }
        }

        public static bool areOppositeDirections(Vector2 forwardDirection, Vector2 playerDirection) {
            return Vector2.Dot(forwardDirection, playerDirection) < 0;         
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

        private Vector2 direction;

        private BoxCollider2D boxCollider;

        private float disableTimer;

        private float delay = 0.125f;

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
                if (player.liveWireMovement.enabled) {
                    Debug.Log("To Mainframe");
                    player.SwitchMovement(PlayerMovementMode.Mainframe);

                } else {
                    Debug.Log("To LiveWire");
                    player.SwitchMovement(PlayerMovementMode.LiveWire);
                    Debug.Log(transform.position);
                    
                    player.liveWireMovement.SetDirection(direction);
                }
            }
        }

        #endregion

    }
}