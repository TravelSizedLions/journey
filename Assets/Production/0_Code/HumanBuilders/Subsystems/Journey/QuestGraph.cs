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
    public QuestProgress Progress { get => progress; }
    private QuestProgress progress = QuestProgress.Unavailable;
    private QuestGraph parentQuest;
    private QuestNode parentNode;

    // --- Rewards ---
    [TitleGroup("Rewards")]
    [AutoTable(typeof(Triggerable), "Quest Rewards", "#ffffff")]
    public List<Triggerable> Rewards;

    // --- Conditions ---
    [ShowInInspector]
    [TitleGroup("Extra Conditions")]
    [AutoTable(typeof(ScriptableCondition), "Availability Conditions", "#ffffff")]
    public List<ScriptableCondition> AvailabilityConditions { get; set; }

    [ShowInInspector]
    [TitleGroup("Extra Conditions")]
  [AutoTable(typeof(ScriptableCondition), "Start Conditions", "#ffffff")]
    public List<ScriptableCondition> StartConditions { get; set; }

    [ShowInInspector]
    [TitleGroup("Extra Conditions")]
    [AutoTable(typeof(ScriptableCondition), "Completion Conditions", "#ffffff")]
    public List<ScriptableCondition> CompletionConditions { get; set; }

    [ShowInInspector]
    [TitleGroup("Extra Conditions")]
    [AutoTable(typeof(ScriptableCondition), "Reward Conditions", "#ffffff")]
    public List<ScriptableCondition> RewardConditions { get; set; }

    // --- Triggers ---
    [ShowInInspector]
    [TitleGroup("Progress Triggers")]
    [AutoTable(typeof(Triggerable), "On Quest Availability", "#ffffff")]
    public List<Triggerable> AvailabilityTriggers { get; set; }

    [ShowInInspector]
    [TitleGroup("Progress Triggers")]
    [AutoTable(typeof(Triggerable), "On Quest Start", "#ffffff")]
    public List<Triggerable> StartTriggers { get; set; }
    
    [ShowInInspector]
    [TitleGroup("Progress Triggers")]
    [AutoTable(typeof(Triggerable), "On Quest Completion", "#ffffff")]
    public List<Triggerable> CompletionTriggers { get; set; }

    [ShowInInspector]
    [TitleGroup("Progress Triggers")]
    [AutoTable(typeof(Triggerable), "On Reward Collection", "#ffffff")]
    public List<Triggerable> RewardTriggers { get; set; }


    //-------------------------------------------------------------------------
    // Public API
    //-------------------------------------------------------------------------
    public void MakeAvailable() {
      progress = QuestProgress.Available;
      PullTriggers(AvailabilityTriggers);
    }

    public void Start() {
      progress = QuestProgress.Started;
      parentNode?.Start();
      PullTriggers(StartTriggers);
    }

    public void MarkComplete() {
      progress = QuestProgress.Completed;
      PullTriggers(CompletionTriggers);
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
    public void AddReward(Triggerable setter) {
      Rewards = Rewards ?? new List<Triggerable>();
      Rewards.Add(setter);
    }

    // --- Conditions ---
    public void AddAvailabilityCondition(ScriptableCondition condition) {
      AvailabilityConditions = AvailabilityConditions ?? new List<ScriptableCondition>();
      AvailabilityConditions.Add(condition);
    }

    public void AddStartCondition(ScriptableCondition condition) {
      StartConditions = StartConditions ?? new List<ScriptableCondition>();
      StartConditions.Add(condition);
    }

    public void AddCompletionCondition(ScriptableCondition condition) {
      CompletionConditions = CompletionConditions ?? new List<ScriptableCondition>();
      CompletionConditions.Add(condition);
    }

    public void AddRewardCondition(ScriptableCondition condition) {
      RewardConditions = RewardConditions ?? new List<ScriptableCondition>();
      RewardConditions.Add(condition);
    }

    // --- Triggers ---
    public void AddAvailabilityTrigger(Triggerable setter) {
      AvailabilityTriggers = AvailabilityTriggers ?? new List<Triggerable>();
      AvailabilityTriggers.Add(setter);
    }

    public void AddStartTrigger(Triggerable setter) {
      StartTriggers = StartTriggers ?? new List<Triggerable>();
      StartTriggers.Add(setter);
    }

    public void AddRewardTrigger(Triggerable setter) {
      RewardTriggers = RewardTriggers ?? new List<Triggerable>();
      RewardTriggers.Add(setter);
    }

    public void AddCompletionTrigger(Triggerable setter) {
      CompletionTriggers = CompletionTriggers ?? new List<Triggerable>();
      CompletionTriggers.Add(setter);
    }


    private void Awake() {
      AvailabilityConditions = AvailabilityConditions ?? new List<ScriptableCondition>();
      StartConditions = StartConditions ?? new List<ScriptableCondition>();
      CompletionConditions = CompletionConditions ?? new List<ScriptableCondition>();
      RewardConditions = RewardConditions ?? new List<ScriptableCondition>();
      AvailabilityTriggers = AvailabilityTriggers ?? new List<Triggerable>();
      StartTriggers = StartTriggers ?? new List<Triggerable>();
      CompletionTriggers = CompletionTriggers ?? new List<Triggerable>();
      RewardTriggers = RewardTriggers ?? new List<Triggerable>();
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

    private void PullTriggers(List<Triggerable> triggers) {
      if (triggers != null) {
        foreach (var trigger in triggers) {
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