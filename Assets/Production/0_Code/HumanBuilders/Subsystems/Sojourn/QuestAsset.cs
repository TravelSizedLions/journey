#if UNITY_EDITOR
using XNodeEditor;
#endif

using UnityEngine;

namespace HumanBuilders {
  [CreateAssetMenu(fileName="New Quest", menuName="Sojourn/Quest")]
  [RequireNode(typeof(QuestStartNode), typeof(QuestEndNode))]
  public class QuestAsset : AutoGraphAsset {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    private QuestAsset parentQuest;

    //-------------------------------------------------------------------------
    // Public API
    //-------------------------------------------------------------------------
    public void SetParent(QuestAsset parent) {
      parentQuest = parent;
    }

    public QuestAsset GetParent() {
      return parentQuest;
    }

    //-------------------------------------------------------------------------
    // AutoNode API
    //-------------------------------------------------------------------------
    /// <summary>
    /// Start the conversation.
    /// </summary>
    /// <returns>The first dialog node of the conversation.</returns>
    public override IAutoNode FindStartingNode() {
      foreach (var node in nodes) {
        QuestStartNode root = node as QuestStartNode;
        if (root != null) {
          return root;
        }
      } 

      return null;
    }

#if UNITY_EDITOR
    //-------------------------------------------------------------------------
    // Editor Stuff
    //-------------------------------------------------------------------------
    [ContextMenu("To Parent Quest")]
    public void Exit() {
      if (parentQuest != null) {
        NodeEditorWindow.Open(parentQuest);
      }
    }
#endif
  }
}