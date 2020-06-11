using System.Collections.Generic;

using UnityEngine;

using XNode;

namespace Storm.Dialog {
  [NodeTint("#996e39")]
  [NodeWidth(360)]
  [CreateNodeMenu("Dialog/Decision Node")]
  public class DecisionNode : Node {

    [Input]
    public EmptyConnection input;

    [Space(12, order=0)]
    [Output(dynamicPortList=true)]
    public List<string> decisions;

    public override object GetValue(NodePort port) {
      return null;
    }
  }
}