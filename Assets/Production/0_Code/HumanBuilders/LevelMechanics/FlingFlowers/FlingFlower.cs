

using UnityEngine;
using UnityEngine.Events;

namespace HumanBuilders {
  public abstract class FlingFlower : MonoBehaviour {

    public float DelayBeforeLaunch = 0.5f;
    public UnityEvent OnEntry;
    public UnityEvent OnFling;
    public virtual void Fling(IPlayer player) {
      if (OnFling != null) {
        OnFling.Invoke();
      }
    }

    public virtual void Entry() {
      if (OnEntry != null) {
        OnEntry.Invoke();
      }
    }

    public virtual void PickDirection(IPlayer player) {}
  }
}