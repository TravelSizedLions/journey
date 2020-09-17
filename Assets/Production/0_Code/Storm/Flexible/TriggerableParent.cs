using UnityEngine;

namespace Storm.Flexible {

  /// <summary>
  /// An abstract class that allows a monobehavior to react to triggers getting
  /// called in a child object.
  /// </summary>
  public interface ITriggerableParent {

    void PullTriggerEnter2D(Collider2D col);

    void PullTriggerStay2D(Collider2D col);

    void PullTriggerExit2D(Collider2D col);
  }

}