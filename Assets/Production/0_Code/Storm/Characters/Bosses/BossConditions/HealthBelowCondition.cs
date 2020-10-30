using Storm.Subsystems.Graph;
using UnityEngine;

namespace Storm.Characters.Bosses {
  public class HealthBelowCondition : Condition {

    /// <summary>
    /// The boss to check this condition for.
    /// </summary>
    public Boss boss;

    /// <summary>
    /// The maximum remaining health to trigger the transition.
    /// </summary>
    public float RemainingHealth;

    /// <summary>
    /// Check the given condition.
    /// </summary>
    public override bool ConditionMet() {
      return boss.RemainingHealth < RemainingHealth;
    }
  }
}