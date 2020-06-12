using System.Collections.Generic;

using UnityEngine;

using XNode;

namespace Storm.Dialog {

  /// <summary>
  /// A dialog node representing list of decisions.
  /// </summary>
  [NodeTint("#996e39")]
  [NodeWidth(360)]
  [CreateNodeMenu("Dialog/Decision Node")]
  public class DecisionNode : Node {

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    /// <summary>
    /// The list of decisions the player can make.
    /// </summary>
    [Space(12, order=0)]
    [Output(dynamicPortList=true)]
    public List<string> Decisions;

    /// <summary>
    /// Get the value of a port.
    /// </summary>
    /// <param name="port">The input/output port.</param>
    /// <returns>The value for the port.</returns>
    public override object GetValue(NodePort port) {
      return null;
    }
  }
}