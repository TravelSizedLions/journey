#if UNITY_EDITOR
using XNodeEditor;
#endif

using UnityEngine;
using XNode;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace HumanBuilders {
  [CreateAssetMenu(fileName="New Quest", menuName="Journey/Quest")]
  [RequireNode(typeof(QuestStartNode), typeof(QuestEndNode))]
  public class QuestGraph : AutoGraphAsset {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    [ShowInInspector]
    [ReadOnly]
    [PropertyOrder(0)]
    public QuestProgress Progress { get => progress; }
    private QuestProgress progress = QuestProgress.Unavailable;
    private QuestGraph parentQuest;
    private QuestNode parentNode;

    private List<ISkippable> optionalObjectives;

    // --- Rewards ---
    [TitleGroup("Rewards")]
    [AutoTable(typeof(VTrigger), "Quest Rewards", "#ffffff", false)]
    public List<VTrigger> Rewards;

    // --- Conditions ---
    [TitleGroup("Progress Conditions")]
    [AutoTable(typeof(VCondition), "Availability Prerequisites", "#ffffff", false)]
    public List<VCondition> AvailabilityConditions;

    [TitleGroup("Progress Conditions")]
    [AutoTable(typeof(VCondition), "Start Prerequisites", "#ffffff", false)]
    public List<VCondition> StartConditions;

    [TitleGroup("Progress Conditions")]
    [AutoTable(typeof(VCondition), "Completion Prerequisites", "#ffffff", false)]
    public List<VCondition> CompletionConditions;

    [TitleGroup("Progress Conditions")]
    [AutoTable(typeof(VCondition), "Reward Prerequisites", "#ffffff", false)]
    public List<VCondition> RewardConditions;

    // --- Triggers ---
    [TitleGroup("Progress Triggers")]
    [AutoTable(typeof(VTrigger), "On Quest Availability", "#ffffff", false)]
    public List<VTrigger> AvailabilityTriggers;

    [TitleGroup("Progress Triggers")]
    [AutoTable(typeof(VTrigger), "On Quest Start", "#ffffff", false)]
    public List<VTrigger> StartTriggers;
    
    [TitleGroup("Progress Triggers")]
    [AutoTable(typeof(VTrigger), "On Quest Completion", "#ffffff", false)]
    public List<VTrigger> CompletionTriggers;

    [TitleGroup("Progress Triggers")]
    [AutoTable(typeof(VTrigger), "On Reward Collection", "#ffffff", false)]
    public List<VTrigger> RewardTriggers;


    //-------------------------------------------------------------------------
    // Public API
    //-------------------------------------------------------------------------
    public void MakeAvailable() {
      progress = QuestProgress.Available;
      PullTriggers(AvailabilityTriggers);
    }

    public void MarkStarted() {
      progress = QuestProgress.Started;
      parentNode?.MarkStarted();
      PullTriggers(StartTriggers);
    }

    public void MarkComplete() {
      progress = QuestProgress.Completed;
      PullTriggers(CompletionTriggers);
      foreach (var obj in optionalObjectives) {
        if (((JourneyNode)obj).Progress != QuestProgress.Completed) {
          obj.MarkSkipped();
        }
      }
    }

    public void MarkSkipped() {
      // Once a quest has started and/or available, it can't be skipped.
      if (progress == QuestProgress.Unavailable) {
        progress = QuestProgress.Skipped;
        // TODO: Add Skip Triggers for a quest. The option to add these triggers
        // should only be visible if the quest has a parent quest and the parent
        // quest node is marked optional.
      }
    }

    public void CollectRewards() {
      progress = QuestProgress.RewardsCollected;
      parentNode?.MarkComplete();
      PullTriggers(Rewards);
      PullTriggers(RewardTriggers);
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

    // --- Rewards ---
    public void AddReward(VTrigger setter) {
      Rewards = Rewards ?? new List<VTrigger>();
      Rewards.Add(setter);
    }

    // --- Conditions ---
    public void AddAvailabilityCondition(VCondition condition) {
      AvailabilityConditions = AvailabilityConditions ?? new List<VCondition>();
      AvailabilityConditions.Add(condition);
    }

    public void AddStartCondition(VCondition condition) {
      StartConditions = StartConditions ?? new List<VCondition>();
      StartConditions.Add(condition);
    }

    public void AddCompletionCondition(VCondition condition) {
      CompletionConditions = CompletionConditions ?? new List<VCondition>();
      CompletionConditions.Add(condition);
    }

    public void AddRewardCondition(VCondition condition) {
      RewardConditions = RewardConditions ?? new List<VCondition>();
      RewardConditions.Add(condition);
    }

    // --- Triggers ---
    public void AddAvailabilityTrigger(VTrigger setter) {
      AvailabilityTriggers = AvailabilityTriggers ?? new List<VTrigger>();
      AvailabilityTriggers.Add(setter);
    }

    public void AddStartTrigger(VTrigger setter) {
      StartTriggers = StartTriggers ?? new List<VTrigger>();
      StartTriggers.Add(setter);
    }

    public void AddRewardTrigger(VTrigger setter) {
      RewardTriggers = RewardTriggers ?? new List<VTrigger>();
      RewardTriggers.Add(setter);
    }

    public void AddCompletionTrigger(VTrigger setter) {
      CompletionTriggers = CompletionTriggers ?? new List<VTrigger>();
      CompletionTriggers.Add(setter);
    }

    public void RegisterOptionalObjective(ISkippable skippableNode) {
      optionalObjectives = optionalObjectives ?? new List<ISkippable>();

      if (!optionalObjectives.Contains(skippableNode)) {
        optionalObjectives.Add(skippableNode);
      }
    }

    private void OnEnable() {
      ResetProgress();
    }

    [Button("Reset Progress")]
    public void ResetProgress() {
      progress = QuestProgress.Unavailable;
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

    private void PullTriggers(List<VTrigger> triggers) {
      if (triggers != null) {
        foreach (var trigger in triggers) {
          // Debug.Log(trigger.Variable);
          trigger.Pull();
        }
      }
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