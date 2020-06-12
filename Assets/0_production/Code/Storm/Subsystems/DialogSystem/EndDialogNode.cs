
using XNode;

namespace Storm.Dialog {

  /// <summary>
  /// A dialog node which ends a conversation.
  /// </summary>
  [NodeTint("#a63333")]
  [CreateNodeMenu("Dialog/End Node")]
  public class EndDialogNode : Node {

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

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