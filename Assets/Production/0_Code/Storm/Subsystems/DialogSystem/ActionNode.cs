
using UnityEngine;
using UnityEngine.Events;
using XNode;

namespace Storm.Dialog {

  /// <summary>
  /// A dialog node for performing a UnityEvent between spoken dialog.
  /// </summary>
  [NodeTint("#996e39")]
  [CreateNodeMenu("Dialog/Dynamic/Action Node")]
  public class ActionNode : Node {

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    /// <summary>
    /// The action to perform.
    /// </summary>
    [Tooltip("The action to perform.")]
    [Space(8, order=0)]
    public UnityEvent Action;

    /// <summary>
    /// Output connection for the next node.
    /// </summary>
    [Space(8, order=1)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

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