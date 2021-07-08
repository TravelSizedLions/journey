using UnityEngine;

#if UNITY_EDITOR
using XNodeEditor;
#endif

namespace HumanBuilders {
  [CreateNodeMenu("")]
  [NodeTint(NodeColors.END_NODE)]
  [DisallowMultipleNodes]
  public class QuestEndNode : EndNode, ISojournNode {

    //-------------------------------------------------------------------------
    // AutoNode API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      Debug.Log("QuestEndNode");
      base.Handle(graphEngine);
    }

#if UNITY_EDITOR
    [ContextMenu("To Parent Quest")]
    public void Exit() {
      QuestGraph quest = (QuestGraph)graph;
      if (quest?.GetParent() != null) {
        NodeEditorWindow.Open(quest.GetParent());
      }
    }
#endif
  }
}
