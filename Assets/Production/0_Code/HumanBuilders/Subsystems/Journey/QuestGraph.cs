#if UNITY_EDITOR
using XNodeEditor;
#endif

using UnityEngine;
using XNode;

namespace HumanBuilders {
  [CreateAssetMenu(fileName="New Quest", menuName="Journey/Quest")]
  [RequireNode(typeof(QuestStartNode), typeof(QuestEndNode))]
  public class QuestGraph : AutoGraphAsset {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    public QuestProgress Progress { get => progress; }
    private QuestProgress progress = QuestProgress.Unavailable;
    private QuestGraph parentQuest;
    private QuestNode parentNode;

    //-------------------------------------------------------------------------
    // Public API
    //-------------------------------------------------------------------------
    public void MakeAvailable() {
      progress = QuestProgress.Available;
    }

    public void Start() {
      progress = QuestProgress.Started;
      parentNode?.Start();
    }

    public void MarkComplete() {
      progress = QuestProgress.Completed;
    }

    public void CollectRewards() {
      progress = QuestProgress.RewardsCollected;
      parentNode?.MarkComplete();
    }

    public void SetParent(QuestGraph parent) {
      parentQuest = parent;
    }

    public void SetParentNode(QuestNode parentNode) {
      this.parentNode = parentNode;
    }

    public QuestGraph GetParent() {
      return parentQuest;
    }

    public QuestNode GetParentNode() {
      return parentNode;
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