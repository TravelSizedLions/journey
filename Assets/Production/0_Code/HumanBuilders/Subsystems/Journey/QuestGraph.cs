#if UNITY_EDITOR
using XNodeEditor;
#endif

using UnityEngine;
using XNode;
using Sirenix.OdinInspector;

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
    [AutoTable(typeof(ITriggerable), "Quest Rewards", "#ffffff")]
    public AutoTable<ITriggerable> Rewards;

    // --- Conditions ---
    [ShowInInspector]
    [TitleGroup("Extra Conditions")]
    [AutoTable(typeof(ICondition), "Availability Conditions", "#ffffff")]
    public AutoTable<ICondition> AvailabilityConditions { get; set; }

    [ShowInInspector]
    [TitleGroup("Extra Conditions")]
    [AutoTable(typeof(ICondition), "Start Conditions", "#ffffff")]
    public AutoTable<ICondition> StartConditions { get; set; }

    [ShowInInspector]
    [TitleGroup("Extra Conditions")]
    [AutoTable(typeof(ICondition), "Completion Conditions", "#ffffff")]
    public AutoTable<ICondition> CompletionConditions { get; set; }

    [ShowInInspector]
    [TitleGroup("Extra Conditions")]
    [AutoTable(typeof(ICondition), "Reward Conditions", "#ffffff")]
    public AutoTable<ICondition> RewardConditions { get; set; }

    // --- Triggers ---
    [ShowInInspector]
    [TitleGroup("Progress Triggers")]
    [AutoTable(typeof(ITriggerable), "On Quest Availability", "#ffffff")]
    public AutoTable<ITriggerable> AvailabilityTriggers { get; set; }

    [ShowInInspector]
    [TitleGroup("Progress Triggers")]
    [AutoTable(typeof(ITriggerable), "On Quest Start", "#ffffff")]
    public AutoTable<ITriggerable> StartTriggers { get; set; }
    
    [ShowInInspector]
    [TitleGroup("Progress Triggers")]
    [AutoTable(typeof(ITriggerable), "On Quest Completion", "#ffffff")]
    public AutoTable<ITriggerable> CompletionTriggers { get; set; }

    [ShowInInspector]
    [TitleGroup("Progress Triggers")]
    [AutoTable(typeof(ITriggerable), "On Reward Collection", "#ffffff")]
    public AutoTable<ITriggerable> RewardTriggers { get; set; }


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
    public void AddReward(ITriggerable setter) {
      Rewards = Rewards ?? new AutoTable<ITriggerable>();
      Rewards.Add(setter);
    }

    // --- Conditions ---
    public void AddAvailabilityCondition(ICondition condition) {
      AvailabilityConditions = AvailabilityConditions ?? new AutoTable<ICondition>();
      AvailabilityConditions.Add(condition);
    }

    public void AddStartCondition(ICondition condition) {
      StartConditions = StartConditions ?? new AutoTable<ICondition>();
      StartConditions.Add(condition);
    }

    public void AddCompletionCondition(ICondition condition) {
      CompletionConditions = CompletionConditions ?? new AutoTable<ICondition>();
      CompletionConditions.Add(condition);
    }

    public void AddRewardCondition(ICondition condition) {
      RewardConditions = RewardConditions ?? new AutoTable<ICondition>();
      RewardConditions.Add(condition);
    }

    // --- Triggers ---
    public void AddAvailabilityTrigger(ITriggerable setter) {
      AvailabilityTriggers = AvailabilityTriggers ?? new AutoTable<ITriggerable>();
      AvailabilityTriggers.Add(setter);
    }

    public void AddStartTrigger(ITriggerable setter) {
      StartTriggers = StartTriggers ?? new AutoTable<ITriggerable>();
      StartTriggers.Add(setter);
    }

    public void AddRewardTrigger(ITriggerable setter) {
      RewardTriggers = RewardTriggers ?? new AutoTable<ITriggerable>();
      RewardTriggers.Add(setter);
    }

    public void AddCompletionTrigger(ITriggerable setter) {
      CompletionTriggers = CompletionTriggers ?? new AutoTable<ITriggerable>();
      CompletionTriggers.Add(setter);
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

    private void PullTriggers(AutoTable<ITriggerable> triggers) {
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