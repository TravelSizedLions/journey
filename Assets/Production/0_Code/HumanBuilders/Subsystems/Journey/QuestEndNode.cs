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

    [FoldoutGroup("Rewards")]
    [AutoTable(typeof(Reward), "Quest Rewards", NodeColors.END_NODE)]
    public AutoTable<Reward> Rewards = null;
    
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(WorldTrigger), "World Triggers on Quest Completion", NodeColors.END_NODE)]
    public AutoTable<WorldTrigger> CompletionTriggers = null;

    [Space(10)]
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(WorldTrigger), "World Triggers on Reward Collection", NodeColors.END_NODE)]
    public AutoTable<WorldTrigger> RewardTriggers = null;

    [FoldoutGroup("Extra Quest Conditions")]
    [AutoTable(typeof(ICondition), "Additional Quest Completion Conditions", NodeColors.END_NODE)]
    public AutoTable<ICondition> CompletionConditions = null;

    [Space(10)]
    [FoldoutGroup("Extra Quest Conditions")]
    [AutoTable(typeof(ICondition), "Additional Reward Conditions", NodeColors.END_NODE)]
    public AutoTable<ICondition> RewardConditions = null;


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

        if (quest.GetParentNode() == null) {
          graphEngine.EndGraph();
        }
      }

    }

    public override void PostHandle(GraphEngine graphEngine) {
      QuestGraph quest = (QuestGraph)graph;
      QuestNode outerNode = quest.GetParentNode();

      if (outerNode != null) {
        graphEngine.RemoveNode(this);
        NodePort outputPort = outerNode.GetOutputPort("Output");

        foreach (NodePort inputPort in outputPort.GetConnections()) {
          graphEngine.AddNode((IAutoNode)inputPort.node);
        }

        graphEngine.Continue();
      }
    }

    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    public void AddCompletionCondition(ICondition condition) {
      CompletionConditions = CompletionConditions ?? new AutoTable<ICondition>();
      CompletionConditions.Add(condition);
    }

    public void AddRewardCondition(ICondition condition) {
      RewardConditions = RewardConditions ?? new AutoTable<ICondition>();
      RewardConditions.Add(condition);
    }

    public bool CanMarkCompleted() {
      NodePort inPort = GetInputPort("Input");
      foreach (NodePort outputPort in inPort.GetConnections()) {
        IJourneyNode jnode = (IJourneyNode)outputPort.node;
        if (jnode.Progress != QuestProgress.Completed && jnode.Required) {
          return false;
        }
      }

      if (CompletionConditions != null) {
        foreach (var condition in CompletionConditions) {
          if (!condition.IsMet()) {
            return false;
          }
        }
      }

      return progress == QuestProgress.Unavailable;
    }

    public bool CanCollectReward() {
      if (RewardConditions != null) {
        foreach (var condition in RewardConditions) {
          if (!condition.IsMet()) {
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
