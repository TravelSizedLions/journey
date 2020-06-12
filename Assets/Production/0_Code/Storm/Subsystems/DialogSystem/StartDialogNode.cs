
using XNode;

namespace Storm.Subsystems.Dialog {
  
  /// <summary>
  /// A dialog representing the start of a conversation.
  /// </summary>
  [NodeTint("#33a643")]
  [CreateNodeMenu("Dialog/Terminal/Start Node")]
  public class StartDialogNode : Node {
    
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