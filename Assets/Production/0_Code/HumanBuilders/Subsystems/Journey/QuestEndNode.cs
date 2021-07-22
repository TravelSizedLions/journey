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
  public class QuestEndNode : JourneyNode, ISerializationCallbackReceiver {
    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    [Input(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Input;

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    [ShowInInspector]
    [TitleGroup("Rewards")]
    [AutoTable(typeof(VTrigger), "Quest Rewards", NodeColors.END_NODE)]
    public List<VTrigger> Rewards {
      get => ((QuestGraph)graph).Rewards;
      set => ((QuestGraph)graph).Rewards = value;
    }

    // --- Triggers ---
    [ShowInInspector]
    [TitleGroup("Progress Conditions")]
    [AutoTable(typeof(VCondition), "Completion Prerequisites", NodeColors.END_NODE)]
    public List<VCondition> CompletionConditions {
      get => ((QuestGraph)graph).CompletionConditions;
      set => ((QuestGraph)graph).CompletionConditions = value;
    }

    [ShowInInspector]
    [TitleGroup("Progress Conditions")]
    [AutoTable(typeof(VCondition), "Reward Prerequisites", NodeColors.END_NODE)]
    public List<VCondition> RewardConditions {
      get => ((QuestGraph)graph).RewardConditions;
      set => ((QuestGraph)graph).RewardConditions = value;
    }

    // --- Conditions ---
    [ShowInInspector]
    [TitleGroup("Progress Triggers")]
    [AutoTable(typeof(VTrigger), "On Quest Completion", NodeColors.END_NODE)]
    public List<VTrigger> CompletionTriggers {
      get => ((QuestGraph)graph).CompletionTriggers;
      set => ((QuestGraph)graph).CompletionTriggers = value;
    }

    [ShowInInspector]
    [TitleGroup("Progress Triggers")]
    [AutoTable(typeof(VTrigger), "On Reward Collection", NodeColors.END_NODE)]
    public List<VTrigger> RewardTriggers {
      get => ((QuestGraph)graph).RewardTriggers;
      set => ((QuestGraph)graph).RewardTriggers = value;
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
    public void AddCompletionCondition(VCondition condition) {
      CompletionConditions = CompletionConditions ?? new List<VCondition>();
      CompletionConditions.Add(condition);
    }

    public void AddRewardCondition(VCondition condition) {
      RewardConditions = RewardConditions ?? new List<VCondition>();
      RewardConditions.Add(condition);
    }

    public void AddReward(VTrigger setter) {
      Rewards = Rewards ?? new List<VTrigger>();
      Rewards.Add(setter);
    }

    public void AddRewardTrigger(VTrigger setter) {
      RewardTriggers = RewardTriggers ?? new List<VTrigger>();
      RewardTriggers.Add(setter);
    }

    public void AddCompletionTrigger(VTrigger setter) {
      CompletionTriggers = CompletionTriggers ?? new List<VTrigger>();
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


    public void OnBeforeSerialize() {
      // Debug.Log("Quest End OnBeforeSerialize");
    }

    public void OnAfterDeserialize() {
      // Debug.Log("Quest End OnAfterDeserialize");
    }
  }
}
