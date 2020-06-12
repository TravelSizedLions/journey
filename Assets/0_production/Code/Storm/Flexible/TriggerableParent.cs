using UnityEngine;

namespace Storm.Flexible {
  public abstract class TriggerableParent : MonoBehaviour {

    public abstract void PullTriggerEnter2D(Collider2D col);

    public abstract void PullTriggerStay2D(Collider2D col);

    public abstract void PullTriggerExit2D(Collider2D col);
  }

}