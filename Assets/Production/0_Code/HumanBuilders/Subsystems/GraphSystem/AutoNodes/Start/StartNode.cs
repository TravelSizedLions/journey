using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders.Graphing {
  
  /// <summary>
  /// A node representing the start of a graph.
  /// </summary>
  [NodeTint(NodeColors.START_COLOR)]
  [CreateNodeMenu("Start/Start")]
  public class StartNode : AutoNode {
    
    [Space(4, order=0)]
    [PropertyOrder(100)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;
  }
}