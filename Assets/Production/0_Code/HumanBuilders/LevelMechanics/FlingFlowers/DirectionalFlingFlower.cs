

using UnityEngine;
using UnityEngine.Events;

namespace HumanBuilders {
  public class DirectionalFlingFlower : FlingFlower, ITriggerableParent {

    /// <summary>
    /// Forwards fling direction.
    /// </summary>
    [SerializeField]
    [Tooltip("Forwards fling direction.")]
    private FlingFlowerDirectionIndicator forwardDirection = null;

    /// <summary>
    /// Backwards fling direction.
    /// </summary>
    [SerializeField]
    [Tooltip("Backwards fling direction.")]
    private FlingFlowerDirectionIndicator backwardDirection = null;

    /// <summary>
    /// The direction the flower is currently primed to fling in.
    /// </summary>
    private FlingFlowerDirectionIndicator flingDirection = null;

    public void PullTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        PlayerCharacter player = col.GetComponent<PlayerCharacter>();
        player.Signal(gameObject);
        Entry();
      }
    }

    public void PullTriggerExit2D(Collider2D col) {}

    public void PullTriggerStay2D(Collider2D col) {}


    /// <summary>
    /// Fling the player.
    /// </summary>
    /// <param name="player">The player character to be flung.</param>
    public override void Fling(IPlayer player) {
      float angle = flingDirection.transform.localEulerAngles.z;
      float angleRad = angle*Mathf.Deg2Rad;

      Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
      float magnitude = player.PowersSettings.FlingFlowerDirectedVelocity;

      player.Physics.Velocity = direction*magnitude;

      if (OnFling != null) {
        OnFling.Invoke();
      }
    }


    public override void PickDirection(IPlayer player) {
      if (backwardDirection == null) {
        flingDirection = forwardDirection;
        return;
      } else if (forwardDirection == null) {
        flingDirection =  backwardDirection;
        return;
      }

      Vector2 playerDir = player.Physics.Velocity;

      float forwardAngle = Mathf.Deg2Rad*forwardDirection.transform.localEulerAngles.z;
      Vector2 forwardDir = new Vector2(Mathf.Cos(forwardAngle), Mathf.Sin(forwardAngle));

      float backwardAngle = Mathf.Deg2Rad*backwardDirection.transform.localEulerAngles.z;
      Vector2 backwardDir = new Vector2(Mathf.Cos(backwardAngle), Mathf.Sin(backwardAngle));

      float forwardDiff = (forwardDir.normalized - playerDir.normalized).magnitude;
      float backwardDiff = (backwardDir.normalized - playerDir.normalized).magnitude;
      
      if (forwardDiff < backwardDiff) {
        flingDirection = forwardDirection;
      } else {
        flingDirection = backwardDirection;
      }
    }

    public void SetForwardDirection(float angle) {
      if (forwardDirection != null) {
        forwardDirection.transform.localRotation = Quaternion.Euler(0, 0, angle);
      }
    }

    public void SetForwardDirection(FlingFlowerDirectionIndicator indicator) {
      forwardDirection = indicator;
    }

    public void ClearForwardsDirection() {
      forwardDirection = null;
    }

    public void SetBackwardDirection(float angle) {
      if (backwardDirection != null) {
        backwardDirection.transform.localRotation = Quaternion.Euler(0, 0, angle);
      }
    }

    public void SetBackwardsDirection(FlingFlowerDirectionIndicator indicator) {
      backwardDirection = indicator;
    }

    public void ClearBackwardsDirection() {
      backwardDirection = null;
    }

    public bool IsTerminal() {
      return (forwardDirection == null && backwardDirection != null) ||
             (backwardDirection == null && forwardDirection != null);
    }
  }

}