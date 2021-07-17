using UnityEngine;
using XNode;

namespace HumanBuilders {
  public class ScriptableCondition : ScriptableObject, ICondition {
    public virtual bool IsMet() => false;
  }
}