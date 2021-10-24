using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace HumanBuilders {
  [NodeTint(NodeColors.DYNAMIC_COLOR)]
  [NodeWidth(360)]
  [CreateNodeMenu("Dialog/Random Path")]
  public class RandomPathNode : AutoNode {
    //---------------------------------------------------
    // Fields
    //---------------------------------------------------
    [PropertyOrder(0)]
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [PropertyOrder(998)]
    [Output(dynamicPortList=true)]
    public List<EmptyConnection> Paths = new List<EmptyConnection>();

    public override IAutoNode GetNextNode() {
      int count = 0;
  
      foreach (NodePort p in DynamicOutputs) count++;
      int num = Random.Range(0, count);
      
      NodePort inPort = GetOutputPort(string.Format("{0} {1}", nameof(Paths), num));
      NodePort outPort = inPort.Connection;
      return (IAutoNode) outPort.node;
    }
  }
}