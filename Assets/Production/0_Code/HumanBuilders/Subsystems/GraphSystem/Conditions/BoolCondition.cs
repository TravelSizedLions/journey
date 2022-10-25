using UnityEngine;

namespace HumanBuilders.Graphing {
  [CreateAssetMenu(fileName="New Boolean Condition", menuName = "Conditions/Boolean")]
  public class BoolCondition : ScriptableCondition {
    public bool Met;
    public override bool IsMet() => Met;
  }
}