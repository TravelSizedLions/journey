
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
    [ShowIf("Required")]
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

    [ShowInInspector]
    [PropertyOrder(3)]
    public new bool Required { 
      get => required;
      set => required = value;
    }

    [SerializeField]
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(VSetter), "World Triggers on Start", NodeColors.BASIC_COLOR)]
    [PropertyOrder(4)]
    private AutoTable<VSetter> StartTriggers;

    [SerializeField]
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(VSetter), "World Triggers on Completion", NodeColors.BASIC_COLOR)]
    [PropertyOrder(5)]
    private AutoTable<VSetter> CompletionTriggers;

    [SerializeField]
    [FoldoutGroup("Rewards")]
    [AutoTable(typeof(VSetter), "Completion Rewards", NodeColors.BASIC_COLOR)]
    [PropertyOrder(6)]
    private AutoTable<VSetter> Rewards;

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
        if (jnode.Progress != QuestProgress.Completed && jnode.Required) {
          return false;
        }
      }

      return true;
    }

    public void MarkStarted() {
      if (StartTriggers != null) {
        foreach (var trigger in StartTriggers) {
          trigger.Set();
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
        foreach (var reward in Rewards) {
          reward.Set();
        }
      }

      if (CompletionTriggers != null) {
        foreach (var trigger in CompletionTriggers) {
          Debug.Log("setting: " + trigger.name);
          trigger.Set();
        }
      }
    }

    public void AddReward(VSetter setter) {
      Rewards = Rewards ?? new AutoTable<VSetter>();
      Rewards.Add(setter);
    }

    public void AddStartTrigger(VSetter setter) {
      StartTriggers = StartTriggers ?? new AutoTable<VSetter>();
      StartTriggers.Add(setter);
    }

    public void AddCompletionTrigger(VSetter setter) {
      CompletionTriggers = CompletionTriggers ?? new AutoTable<VSetter>();
      CompletionTriggers.Add(setter);
    }
  }
}