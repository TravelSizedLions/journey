
using System;
using XNode;

namespace Storm.Subsystems.Graph {
  
  /// <summary>
  /// A dialog node representing the start of a conversation.
  /// </summary>
  [NodeTint(NodeColors.START_COLOR)]
  [CreateNodeMenu("Start/Start Dialog")]
  public class StartDialogNode : StartNode { }
}