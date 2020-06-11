
using UnityEngine;
using UnityEngine.Events;
using XNode;

namespace Storm.Dialog {
  [NodeTint("#996e39")]
  [CreateNodeMenu("Dialog/Action Node")]
  public class ActionNode : Node {
    [Input]
    public EmptyConnection input;

    [Space(8, order=0)]
    public UnityEvent action;

    [Space(8, order=1)]
    [Output]
    public EmptyConnection output;


    public override object GetValue(NodePort port) {
      return null;
    }
  }
}