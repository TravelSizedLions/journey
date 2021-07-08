using UnityEngine;

namespace HumanBuilders {
  public class DummyCondition : ScriptableCondition {
    public bool Met;
    public override bool IsMet() => Met;
  }
}