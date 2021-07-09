using UnityEngine;

namespace HumanBuilders {
  [CreateAssetMenu(fileName="New Boolean Condition", menuName = "Conditions/Boolean")]
  public class BoolCondition : ScriptableCondition {
    public bool Met;
    public override bool IsMet() => Met;
  }
}