
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace HumanBuilders {
  [CreateNodeMenu("Objective")]
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  public class ObjectiveNode : JourneyNode {
    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    [PropertyOrder(0)]
    [Input(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Input;


    [Space(10)]
    [PropertyOrder(999)]
    [Output(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    [Space(10)]
    [PropertyOrder(1)]
    public string Description;

    [Space(10)]
    [PropertyOrder(2)]
    [ShowInInspector]
    public ICondition Condition;

    [SerializeField]
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(VTrigger), "World Triggers on Start", NodeColors.BASIC_COLOR)]
    [PropertyOrder(4)]
    private AutoTable<VTrigger> StartTriggers;

    [SerializeField]
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(VTrigger), "World Triggers on Completion", NodeColors.BASIC_COLOR)]
    [PropertyOrder(5)]
    private AutoTable<VTrigger> CompletionTriggers;

    [SerializeField]
    [FoldoutGroup("Rewards")]
    [AutoTable(typeof(VTrigger), "Completion Rewards", NodeColors.BASIC_COLOR)]
    [PropertyOrder(6)]
    private AutoTable<VTrigger> Rewards;

    //-------------------------------------------------------------------------
    // AutoNode API
    //-------------------------------------------------------------------------
    public override void PostHandle(GraphEngine graphEngine) {
      if (CanMarkStarted()) {
        MarkStarted();
      }

      if (CanMarkCompleted()) {
        MarkCompleted();
        base.PostHandle(graphEngine);
      }
    }


    public bool CanMarkStarted() {
      if (progress != QuestProgress.Unavailable) {
        return false;
      }

      NodePort inPort = GetInputPort("Input");
      foreach (NodePort outputPort in inPort.GetConnections()) {
        IJourneyNode jnode = (IJourneyNode)outputPort.node;
        if (jnode.Progress != QuestProgress.Completed) {
          return false;
        }
      }

      return true;
    }

    public void MarkStarted() {
      if (StartTriggers != null) {
        foreach (var trigger in StartTriggers) {
          trigger.Pull();
        }
      }

      progress = QuestProgress.Started;
    }

    public bool CanMarkCompleted() {
      return progress == QuestProgress.Started && Condition.IsMet();
    }

    public void MarkCompleted() {
      progress = QuestProgress.Completed;
      if (Rewards != null) {
        foreach (var rewardTrigger in Rewards) {
          rewardTrigger.Pull();
        }
      }

      if (CompletionTriggers != null) {
        foreach (var trigger in CompletionTriggers) {
          trigger.Pull();
        }
      }
    }

    public void AddReward(VTrigger setter) {
      Rewards = Rewards ?? new AutoTable<VTrigger>();
      Rewards.Add(setter);
    }

    public void AddStartTrigger(VTrigger setter) {
      StartTriggers = StartTriggers ?? new AutoTable<VTrigger>();
      StartTriggers.Add(setter);
    }

    public void AddCompletionTrigger(VTrigger setter) {
      CompletionTriggers = CompletionTriggers ?? new AutoTable<VTrigger>();
      CompletionTriggers.Add(setter);
    }
  }
}