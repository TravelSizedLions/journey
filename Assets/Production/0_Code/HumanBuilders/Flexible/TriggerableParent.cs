using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// An abstract class that allows a monobehavior to react to triggers getting
  /// called in a child object.
  /// </summary>
  public interface ITriggerableParent {

    /// <summary>
    /// This method fires when a collider enters the trigger area of a child
    /// game object.
    /// </summary>
    /// <param name="col">The other collider</param>
    void PullTriggerEnter2D(Collider2D col);

    /// <summary>
    /// This method fires while a collider is within the trigger area of a child
    /// game object.
    /// </summary>
    /// <param name="col">The other collider</param>
    void PullTriggerStay2D(Collider2D col);

    /// <summary>
    /// This method fires when a collider exits the trigger area of a child
    /// game object.
    /// </summary>
    /// <param name="col">The other collider</param>
    void PullTriggerExit2D(Collider2D col);
  }

}