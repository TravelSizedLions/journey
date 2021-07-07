#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

using UnityEngine;
using Sirenix.OdinInspector;

namespace HumanBuilders {
  [CreateAssetMenu(fileName="New Quest", menuName="Sojourn/Quest")]
  public class QuestAsset : AutoGraphAsset {
    private QuestAsset parentQuest;
    private bool created = false;
    public void OnCreate() {
      if (!created) {
        QuestStartNode startNode = AddNode<QuestStartNode>();
        startNode.name = "Start";
        startNode.position = new Vector2(-400f, 0);

        QuestEndNode endNode = AddNode<QuestEndNode>();
        endNode.name = "End";
        endNode.position = new Vector2(400f, 0);
        
        created = true;
      }
    }

    public void SetParent(QuestAsset parent) {
      parentQuest = parent;
    }

    public QuestAsset GetParent() {
      return parentQuest;
    }

#if UNITY_EDITOR
    [ContextMenu("To Parent Quest")]
    public void Exit() {
      if (parentQuest != null) {
        NodeEditorWindow.Open(parentQuest);
      }
    }
#endif
  }
}