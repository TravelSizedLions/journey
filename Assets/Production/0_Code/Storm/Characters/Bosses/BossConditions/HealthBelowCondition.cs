using Storm.Subsystems.Graph;

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

    public override bool ConditionMet() {
      return boss.RemainingHealth < RemainingHealth;
    }
  }
}