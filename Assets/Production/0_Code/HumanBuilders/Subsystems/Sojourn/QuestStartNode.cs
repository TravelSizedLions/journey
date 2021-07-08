using UnityEngine;

#if UNITY_EDITOR
using XNodeEditor;
#endif

namespace HumanBuilders {
  [CreateNodeMenu("")]
  [NodeTint(NodeColors.START_COLOR)]
  [DisallowMultipleNodes]
  public class QuestStartNode : AutoNode, ISojournNode {
    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    [Output(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // AutoNode API
    //-------------------------------------------------------------------------
    public override void PostHandle(GraphEngine graphEngine) {
      // Purposely left blank
    }

#if UNITY_EDITOR
    [ContextMenu("To Parent Quest")]
    public void Exit() {
      QuestAsset quest = (QuestAsset)graph;
      if (quest?.GetParent() != null) {
        NodeEditorWindow.Open(quest.GetParent());
      }
    }
#endif

  }
}
