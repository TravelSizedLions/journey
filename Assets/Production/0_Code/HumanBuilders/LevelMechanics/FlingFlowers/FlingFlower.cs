

using UnityEngine;

namespace HumanBuilders {
  public abstract class FlingFlower : MonoBehaviour {

    public float DelayBeforeLaunch = 0.5f;
    public virtual void Fling(IPlayer player) {}

    public virtual void PickDirection(IPlayer player) {}
  }
}