
using System.Collections.Generic;
using Sirenix.OdinInspector;

using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// A node that causes the boss to start attacking.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.BOSS_COLOR)]
  [CreateNodeMenu("Bosses/Start Attacking")]
  public class StartAttackingNode : AutoNode {
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]
    [Tooltip("The boss to make stop attacking.")]
    public Boss boss;

    [Space(10, order=1)]
    [Output(connectionType = ConnectionType.Override)]
    public EmptyConnection Output;

    public override void Handle(GraphEngine graphEngine) {
      if (boss != null) {
        boss.StartAttacking();
      }
    }
  }
}