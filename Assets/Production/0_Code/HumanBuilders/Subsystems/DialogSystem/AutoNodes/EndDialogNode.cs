using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// A dialog node which ends a conversation.
  /// </summary>
  [NodeTint(NodeColors.END_NODE)]
  [CreateNodeMenu("End/End Dialog")]
  public class EndDialogNode : EndNode {

    public override void Handle(GraphEngine graphEngine) {
      Debug.Log("Ending!");
      graphEngine.EndGraph();
    }

    public override void PostHandle(GraphEngine graphEngine) {}
  }
}
