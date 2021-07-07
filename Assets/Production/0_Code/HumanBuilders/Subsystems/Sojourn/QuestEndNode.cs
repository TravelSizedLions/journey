using UnityEngine;

#if UNITY_EDITOR
using XNodeEditor;
#endif

namespace HumanBuilders {
  [CreateNodeMenu("")]
  [NodeTint(NodeColors.END_NODE)]
  [DisallowMultipleNodes]
  public class QuestEndNode : SojournNode {

    [Input(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Input;


    [ContextMenu("To Parent Quest")]
    public void Exit() {
      QuestAsset quest = (QuestAsset)graph;
      if (quest?.GetParent() != null) {
        NodeEditorWindow.Open(quest.GetParent());
      }
    }
  }
}