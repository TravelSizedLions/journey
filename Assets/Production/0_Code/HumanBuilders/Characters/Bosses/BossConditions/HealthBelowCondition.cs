
using HumanBuilders.Graphing;
using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// A condition that checks whether or not a boss is below a certain amount of
  /// health.
  /// </summary>
  public class HealthBelowCondition : AutoCondition {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The boss to check this condition for.
    /// </summary>
    [Tooltip("The boss to check this condition for.")]
    public Boss boss;

    /// <summary>
    /// When the boss has less than this amount of health, then the transition
    /// will trigger.
    /// </summary>
    [Tooltip("When the boss has less than this amount of health, then the transition will trigger.")]
    public float RemainingHealth;
    #endregion


    #region Condition Check Interface
    //-------------------------------------------------------------------------
    // Condition Check Interface
    //-------------------------------------------------------------------------
    /// <summary>
    /// Checks the given condition.
    /// </summary>
    public override bool IsMet() {
      return boss.RemainingHealth < RemainingHealth;
    }
    #endregion

  }
}