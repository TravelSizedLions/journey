using HumanBuilders.Graphing;
using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// A node representing one phase of a boss battle.
  /// </summary>
  [NodeWidth(180)]
  [NodeTint(NodeColors.END_NODE)]
  [CreateNodeMenu("Bosses/Battle End Marker")]
  public class BattleEndMarkerNode : AutoNode {
    //-------------------------------------------------------------------------
    // Input Ports
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    /// <summary>
    /// Output connection to the next node.
    /// </summary>
    [Output(connectionType = ConnectionType.Override)]
    public EmptyConnection Output;
  }
}