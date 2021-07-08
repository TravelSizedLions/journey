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
      QuestGraph quest = (QuestGraph)graph;
      if (quest.Progress == QuestProgress.Started) {
        base.PostHandle(graphEngine);
      }
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
