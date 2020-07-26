using UnityEngine;

namespace Storm.Flexible {

  /// <summary>
  /// An abstract class that allows a monobehavior to react to triggers getting
  /// called in a child object.
  /// </summary>
  public abstract class TriggerableParent : MonoBehaviour {

    public abstract void PullTriggerEnter2D(Collider2D col);

    public abstract void PullTriggerStay2D(Collider2D col);

    public abstract void PullTriggerExit2D(Collider2D col);
  }

}