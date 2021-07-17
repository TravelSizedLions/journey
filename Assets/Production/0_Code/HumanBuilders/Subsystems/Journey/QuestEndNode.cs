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
    [ShowInInspector]
    [FoldoutGroup("Rewards")]
    [AutoTable(typeof(ITriggerable), "Quest Rewards", NodeColors.END_NODE)]
    public AutoTable<ITriggerable> Rewards {
      get => ((QuestGraph)graph).Rewards;
      set => ((QuestGraph)graph).Rewards = value;
    }
    
    [ShowInInspector]
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(ITriggerable), "World Triggers on Quest Completion", NodeColors.END_NODE)]
    public AutoTable<ITriggerable> CompletionTriggers {
      get => ((QuestGraph)graph).CompletionTriggers;
      set => ((QuestGraph)graph).CompletionTriggers = value;
    }
    
    [ShowInInspector]
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(ITriggerable), "World Triggers on Reward Collection", NodeColors.END_NODE)]
    public AutoTable<ITriggerable> RewardTriggers {
      get => ((QuestGraph)graph).RewardTriggers;
      set => ((QuestGraph)graph).RewardTriggers = value;
    }
    
    [ShowInInspector]
    [FoldoutGroup("Extra Quest Conditions")]
    [AutoTable(typeof(ICondition), "Quest Completion Conditions", NodeColors.END_NODE)]
    public AutoTable<ICondition> CompletionConditions {
      get => ((QuestGraph)graph).CompletionConditions;
      set => ((QuestGraph)graph).CompletionConditions = value;
    }

    [ShowInInspector]
    [FoldoutGroup("Extra Quest Conditions")]
    [AutoTable(typeof(ICondition), "Additional Reward Conditions", NodeColors.END_NODE)]
    public AutoTable<ICondition> RewardConditions {
      get => ((QuestGraph)graph).RewardConditions;
      set => ((QuestGraph)graph).RewardConditions = value;
    }

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

      if (outerNode != null && quest.Progress == QuestProgress.RewardsCollected) {
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

    public void AddReward(ITriggerable setter) {
      Rewards = Rewards ?? new AutoTable<ITriggerable>();
      Rewards.Add(setter);
    }

    public void AddRewardTrigger(ITriggerable setter) {
      RewardTriggers = RewardTriggers ?? new AutoTable<ITriggerable>();
      RewardTriggers.Add(setter);
    }

    public void AddCompletionTrigger(ITriggerable setter) {
      CompletionTriggers = CompletionTriggers ?? new AutoTable<ITriggerable>();
      CompletionTriggers.Add(setter);
    }

    private bool CanMarkCompleted() {
      NodePort inPort = GetInputPort("Input");
      foreach (NodePort outputPort in inPort.GetConnections()) {
        IJourneyNode jnode = (IJourneyNode)outputPort.node;
        if (jnode.Progress != QuestProgress.Completed) {
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

    private bool CanCollectReward() {
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

    // public void RewardsChanged() {
    //   Debug.Log("Quest End Node Reward Change");
    //   QuestGraph quest = (QuestGraph)graph;
    //   quest.Rewards = Rewards;
    // }
#endif
  }
}
