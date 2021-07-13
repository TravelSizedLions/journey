
using UnityEngine;

namespace HumanBuilders {
  public interface ITrigger {}
  public abstract class WorldTrigger : ScriptableObject, ITrigger {
    public abstract void Trigger();
  }
}