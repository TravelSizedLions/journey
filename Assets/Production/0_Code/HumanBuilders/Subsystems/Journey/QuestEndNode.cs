using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

#if UNITY_EDITOR
using XNodeEditor;
#endif

namespace HumanBuilders {
  [CreateNodeMenu("")]
  [NodeTint(NodeColors.END_NODE)]
  [NodeWidth(400)]
  [DisallowMultipleNodes]
  public class QuestEndNode : JourneyNode {
    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    [Input(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Input;

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    [FoldoutGroup("Extra Quest Conditions")]
    [ConditionTable("Additional Quest Completion Conditions", 0.631f, 0.227f, 0.314f)]
    public List<ConditionTableEntry> CompletionConditions = null;

    [Space(10)]
    [FoldoutGroup("Extra Quest Conditions")]
    [ConditionTable("Additional Reward Conditions", 0.631f, 0.227f, 0.314f)]
    public List<ConditionTableEntry> RewardConditions = null;


    //-------------------------------------------------------------------------
    // AutoNode API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      QuestGraph quest = (QuestGraph)graph;
      if (CanMarkCompleted()) {
        progress = QuestProgress.Completed;
        quest.MarkComplete();
      }

      if (CanCollectReward()) {
        progress = QuestProgress.RewardsCollected;
        quest.CollectRewards();
        graphEngine.EndGraph();
      }

    }

    public override void PostHandle(GraphEngine graphEngine) {
      // Intentionally overriden as blank.
    }

    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    public void AddCompletionCondition(ICondition condition) {
      CompletionConditions = CompletionConditions ?? new List<ConditionTableEntry>();
      CompletionConditions.Add(new ConditionTableEntry(condition));
    }

    public void AddRewardCondition(ICondition condition) {
      RewardConditions = RewardConditions ?? new List<ConditionTableEntry>();
      RewardConditions.Add(new ConditionTableEntry(condition));
    }

    public bool CanMarkCompleted() {
      NodePort inPort = GetInputPort("Input");
      foreach (NodePort outputPort in inPort.GetConnections()) {
        IJourneyNode jnode = (IJourneyNode)outputPort.node;
        if (jnode.Progress != QuestProgress.Completed) {
          return false;
        }
      }

      if (CompletionConditions != null) {
        foreach (var entry in CompletionConditions) {
          if (!entry.Condition.IsMet()) {
            return false;
          }
        }
      }

      return progress == QuestProgress.Unavailable;
    }

    public bool CanCollectReward() {
      if (RewardConditions != null) {
        foreach (var entry in RewardConditions) {
          if (!entry.Condition.IsMet()) {
            return false;
          }
        }
      }

      return progress == QuestProgress.Completed;    
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
