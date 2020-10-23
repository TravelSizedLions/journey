using System;
using UnityEngine;

namespace Storm.Subsystems.Graph {

  public interface ICondition {

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    /// <summary>
    /// Check the given condition.
    /// </summary>
    /// <seealso cref="Condition.ConditionMet" />
    bool ConditionMet();

    /// <summary>
    /// Perform the transition from one node to the next.
    /// </summary>
    /// <param name="node">The node to transition from.</param>
    /// <seealso cref="Condition.Transition" />
    void Transition(IAutoNode node);

    #endregion
  }

  /// <summary>
  /// This class represents a condition that can be used to wait until 
  /// </summary>
  public class Condition : MonoBehaviour, ICondition {
    /// <summary>
    /// Check the given condition.
    /// </summary>
    public virtual bool ConditionMet() => false;

    /// <summary>
    /// Perform the transition from one node to the next.
    /// </summary>
    /// <param name="node">The node to transition from.</param>
    public virtual void Transition(IAutoNode node) {}
  }
}