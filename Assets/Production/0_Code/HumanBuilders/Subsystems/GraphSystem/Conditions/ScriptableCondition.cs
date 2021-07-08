using UnityEngine;
using XNode;

namespace HumanBuilders {
  public class ScriptableCondition : ScriptableObject, ICondition {
    public string OutputPort { get; set; }
    public virtual bool IsMet() => false;

    public void Transition(GraphEngine graphEngine, IAutoNode node) {
      Node xnode = (Node) node;

      NodePort port = xnode.GetOutputPort(OutputPort);
      NodePort nextPort = port.Connection;
      IAutoNode nextNode = (IAutoNode) nextPort.node;

      graphEngine.SetCurrentNode(nextNode);
      graphEngine.Continue();
    }
  }
}