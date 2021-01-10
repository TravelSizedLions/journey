

using UnityEngine;

namespace HumanBuilders {
  public abstract class FlingFlower : MonoBehaviour {
    public virtual void Fling(IPlayer player) {}

    public virtual void PickDirection(IPlayer player) {}
  }
}