
using System;
using XNode;

namespace Storm.Subsystems.Graph {
  
  /// <summary>
  /// A node representing the start of a conversation.
  /// </summary>
  [NodeTint(NodeColors.START_COLOR)]
  [CreateNodeMenu("Start/Start")]
  public class StartNode : AutoNode {
    
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;
  }
}