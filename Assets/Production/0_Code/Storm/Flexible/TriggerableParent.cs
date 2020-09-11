using UnityEngine;

namespace Storm.Flexible {

  /// <summary>
  /// An abstract class that allows a monobehavior to react to triggers getting
  /// called in a child object.
  /// </summary>
  public abstract class TriggerableParent : MonoBehaviour {

    public virtual void PullTriggerEnter2D(Collider2D col) {}

    public virtual void PullTriggerStay2D(Collider2D col) {}

    public virtual void PullTriggerExit2D(Collider2D col) {}
  }

}