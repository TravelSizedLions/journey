
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Storm.Subsystems.Graph;
using UnityEngine;

namespace Storm.Characters.Bosses {
  /// <summary>
  /// A node that causes the boss to stop attacking.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.BOSS_COLOR)]
  [CreateNodeMenu("Bosses/StartAttacking")]
  public class StartAttackingNode : AutoNode {

    #region Input Ports
    //-------------------------------------------------------------------------
    // Input Ports
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]
    #endregion

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The boss to make stop attacking.
    /// </summary>
    [Tooltip("The boss to make stop attacking.")]
    public Boss boss;

    [Space(10, order=1)]
    #endregion

    #region Output Ports
    //-------------------------------------------------------------------------
    // Output Ports
    //-------------------------------------------------------------------------
    /// <summary>
    /// Output connection to the next node.
    /// </summary>
    [Output(connectionType = ConnectionType.Override)]
    public EmptyConnection Output;
    #endregion

    #region Auto Node API
    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------

    public override void Handle(GraphEngine graphEngine) {
      if (boss != null) {
        boss.StartAttacking();
      }
    }

    #endregion
  }
}