
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HumanBuilders {
  [CreateNodeMenu("Objective")]
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  public class ObjectiveNode : JourneyNode, ISkippable {
    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    [PropertyOrder(0)]
    [Input(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Input;


    [ShowIf("Required")]
    [Space(10)]
    [PropertyOrder(999)]
    [Output(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    [TitleGroup("Details")]
    [PropertyOrder(1)]
    [OnValueChanged("ChangeName")]
    public string Description;

    [TitleGroup("Details")]
    [PropertyOrder(2)]
    [ShowInInspector]
    public VCondition Condition;

    [TitleGroup("Details")]
    [PropertyOrder(3)]
    [ShowInInspector]
    public bool Required { get => required; set => required = value; }

    [FoldoutGroup("Objective")]
    [PropertyOrder(3)]
    [HideIf("Required")]
    public VCondition SkipCondition;

    // --- Rewards ---
    [SerializeField]
    [TitleGroup("Rewards")]
    [AutoTable(typeof(VTrigger), "Rewards", NodeColors.BASIC_COLOR)]
    [PropertyOrder(4)]
    private List<VTrigger> Rewards;

    [SerializeField]
    [TitleGroup("Progress Triggers")]
    [AutoTable(typeof(VTrigger), "On Start", NodeColors.BASIC_COLOR)]
    [PropertyOrder(5)]
    private List<VTrigger> StartTriggers;

    [SerializeField]
    [TitleGroup("Progress Triggers")]
    [AutoTable(typeof(VTrigger), "On Completion", NodeColors.BASIC_COLOR)]
    [PropertyOrder(6)]
    private List<VTrigger> CompletionTriggers;


    [SerializeField]
    [TitleGroup("Progress Triggers")]
    [AutoTable(typeof(VTrigger), "On Skipped", NodeColors.BASIC_COLOR)]
    [HideIf("Required")]
    [PropertyOrder(7)]
    private List<VTrigger> SkipTriggers;

    //-------------------------------------------------------------------------
    // AutoNode API
    //-------------------------------------------------------------------------
    public override void PostHandle(GraphEngine graphEngine) {
      bool skip = ShouldSkip();
      if (skip || progress == QuestProgress.Skipped) {
        if (skip) {
          MarkSkipped();
        }

        TryPostHandle(graphEngine);
      } else {
        if (CanMarkStarted()) {
          MarkStarted();
        }

        if (CanMarkCompleted()) {
          MarkCompleted();
          TryPostHandle(graphEngine);
        }
      }
    }

    public void TryPostHandle(GraphEngine graphEngine) {
      if (GetOutputPort("Output").ConnectionCount > 0) {
        base.PostHandle(graphEngine);
      }
    }

    public bool ShouldSkip() {
      if (Required) {
        return false;
      }

      if (SkipCondition != null && SkipCondition.IsMet()) {
        return true;
      }

      return false;
    }

    public void MarkSkipped() {
      progress = QuestProgress.Skipped;
      if (SkipTriggers != null) {
        foreach (var trig in SkipTriggers) {
          trig.Pull();
        }
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
      return progress == QuestProgress.Started && Condition != null && Condition.IsMet();
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
      Rewards = Rewards ?? new List<VTrigger>();
      Rewards.Add(setter);
    }

    public void AddStartTrigger(VTrigger setter) {
      StartTriggers = StartTriggers ?? new List<VTrigger>();
      StartTriggers.Add(setter);
    }

    public void AddCompletionTrigger(VTrigger setter) {
      CompletionTriggers = CompletionTriggers ?? new List<VTrigger>();
      CompletionTriggers.Add(setter);
    }

    public void AddSkipTriggers(VTrigger trigger) {
      SkipTriggers = SkipTriggers ?? new List<VTrigger>();
      SkipTriggers.Add(trigger);
    }

    protected override void OnEnable() {
      base.OnEnable();
      if (!Required) {
        ((QuestGraph)graph).RegisterOptionalObjective(this);
      }
    }

    public void ChangeName() {
      #if UNITY_EDITOR
      name = string.IsNullOrEmpty(Description) ? "Objective" : "Objective: " + Description;
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
      #endif
    }
  }
}