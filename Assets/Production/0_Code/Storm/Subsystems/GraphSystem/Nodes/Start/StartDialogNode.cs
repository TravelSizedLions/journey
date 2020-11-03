
using XNode;

namespace Storm.Subsystems.Graph {
  
  /// <summary>
  /// A dialog representing the start of a conversation.
  /// </summary>
  [NodeTint(NodeColors.START_COLOR)]
  [CreateNodeMenu("Start/Start")]
  public class StartDialogNode : AutoNode {
    
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;
  }
}