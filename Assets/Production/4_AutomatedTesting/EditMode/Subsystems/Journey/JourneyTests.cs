using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using XNodeEditor;

namespace HumanBuilders.Tests {
  public class JourneyTests {
    // --- Conditions ----------------------------------------------------------------------------------------------------------
    public Dictionary<string, VTrigger> Triggers;
    public Dictionary<string, VCondition> Conditions;
    public List<string> Variables;
    public List<string> TrivialQuests;
    public List<string> SimpleQuests;
    public List<string> Quests;

    // --- Names ---------------------------------------------------------------------------------------------------------------
    private const string EXTENSION = ".asset";
    private const string TEST_FOLDER = "Assets/Production/4_AutomatedTesting/EditMode/Subsystems/Journey/Resources";

    private const string SIMPLE_OBJECTIVE_QUEST = "simple_objective_quest";
    private const string TRIVIAL_QUEST_WITH_AVAILABILITY_CONDITIONS = "trivial_quest_with_availability_conditions";
    private const string TRIVIAL_QUEST_WITH_START_CONDITIONS = "trivial_quest_with_start_conditions";
    private const string TRIVIAL_QUEST_WITH_AVAIL_START_CONDITIONS = "trivial_quest_with_avail_start_conditions";
    private const string TRIVIAL_QUEST_WITH_COMP_CONDITIONS = "trivial_quest_with_comp_conditions";
    private const string TRIVIAL_QUEST_WITH_REWARD_CONDITIONS = "trivial_quest_with_reward_conditions";
    private const string TRIVIAL_QUEST_WITH_COMP_REWARD_CONDITIONS = "trivial_quest_with_comp_reward_conditions";
    private const string QUEST_PARALLEL_OBJECTIVES = "quest_parallel_objectives";
    private const string NESTED_QUEST_OUTER = "nested_quest_outer";
    private const string CHAINED_NESTED = "chained_nested";
    private const string DEEPLY_NESTED = "deeply_nested";
    private const string NESTED_PARALLEL = "nested_parallel";
    private const string PARALLEL_NESTED = "parallel_nested";
    private const string MIXED_PARALLEL = "mixed_parallel";

    private const string HAS_REWARD = "has_reward";
    private const string OBJECTIVE_REWARD = "objective_reward";
    private const string OBJECTIVE_START_TRIGGER = "objective_start_trigger";
    private const string OBJECTIVE_COMPLETE_TRIGGER = "objective_complete_trigger";
    private const string QUEST_AVAILABLE_TRIGGER = "quest_available_trigger";
    private const string QUEST_STARTED_TRIGGER = "quest_started_trigger";
    private const string QUEST_COMPLETE_TRIGGER = "quest_complete_trigger";
    private const string QUEST_REWARD_TRIGGER = "quest_reward_trigger";

    private const string START_PERSISTENCE = "start_persistence";
    private const string END_PERSISTENCE = "end_persistence";
    private const string OBJ_PERSISTENCE = "obj_persistence";


    // --- Setup / Tear Down ---------------------------------------------------------------------------------------------------

    [OneTimeSetUp]
    public void OneTimeSetup() {
      Conditions = new Dictionary<string, VCondition>();
      TrivialQuests = new List<string>();
      SimpleQuests = new List<string>();
      Quests = new List<string>();
      Triggers = new Dictionary<string, VTrigger>();
      Variables = new List<string>();
      VSave.FolderName = "journey_tests";
    }

    [TearDown]
    public void TearDown() {
      DeleteAssets();
      Conditions.Clear();
      TrivialQuests.Clear();
      SimpleQuests.Clear();
      Quests.Clear();
      Triggers.Clear();
      Variables.Clear();
      VSave.Reset();
    }

    [SetUp]
    public void Setup() {
      VSave.CreateSlot("test");
      VSave.ChooseSlot("test");
    }

    public void DeleteAssets() {
      foreach (string guid in AssetDatabase.FindAssets("l:journey_test")) {
        AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));
      }
      AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    // --- Tests ---------------------------------------------------------------------------------------------------------------

    [Test]
    public void Editor_Inner_Quest_Gets_Parent() {
      QuestGraph outer = ScriptableObject.CreateInstance<QuestGraph>();

      QuestNode qNode = outer.AddNode<QuestNode>();

      QuestGraph inner = ScriptableObject.CreateInstance<QuestGraph>();
      qNode.ChangeQuest(inner);

      Assert.AreEqual(inner.GetParent(), outer);
    }

    [Test]
    public void Editor_Creates_Default_Nodes() {
      QuestGraph quest = GetTrivialQuest("empty");
      Assert.IsTrue(quest.nodes.Count == 2);

      bool foundStart = false;
      bool foundEnd = false;
      foreach (var n in quest.nodes) {
        if (n.GetType() == typeof(QuestStartNode)) {
          foundStart = true;
        }

        if (n.GetType() == typeof(QuestEndNode)) {
          foundEnd = true;
        }
      }

      Assert.IsTrue(foundStart && foundEnd);
    }

    [UnityTest]
    public IEnumerator Editor_Opens_Subquest() {
      QuestGraph outer = ScriptableObject.CreateInstance<QuestGraph>();
      QuestNode qNode = outer.AddNode<QuestNode>();
      QuestGraph inner = ScriptableObject.CreateInstance<QuestGraph>();
      qNode.ChangeQuest(inner);

      yield return null;

      qNode.Open();

      yield return null;

      Assert.IsTrue(NodeEditorWindow.current.graphEditor.target == inner);
    }

    [UnityTest]
    public IEnumerator Editor_Exits_Subquest_When_Parent_Is_Present() {
      QuestGraph outer = ScriptableObject.CreateInstance<QuestGraph>();
      QuestNode qNode = outer.AddNode<QuestNode>();
      QuestGraph inner = ScriptableObject.CreateInstance<QuestGraph>();
      qNode.ChangeQuest(inner);

      yield return null;

      NodeEditorWindow.Open(inner);

      yield return null;

      inner.Exit();

      yield return null;

      Assert.IsTrue(NodeEditorWindow.current.graphEditor.target == outer);
    }

    [UnityTest]
    public IEnumerator Editor_Does_Not_Exit_Quest_Without_Parent() {
      QuestGraph outer = ScriptableObject.CreateInstance<QuestGraph>();
      yield return null;

      NodeEditorWindow.Open(outer);

      yield return null;

      outer.Exit();

      yield return null;

      Assert.IsTrue(NodeEditorWindow.current.graphEditor.target == outer);
    }

    [Test]
    public void QuestNodes_Bind_Changes_To_Quests() {
      QuestGraph outer = GetQuest("outer");
      QuestGraph inner = GetQuest("inner");

      QuestNode qNode = outer.AddNode<QuestNode>();
      qNode.ChangeQuest(inner);

      qNode.Rewards = new List<VTrigger>();
      qNode.AvailabilityConditions = new List<VCondition>();
      qNode.StartConditions = new List<VCondition>();
      qNode.CompletionConditions = new List<VCondition>();
      qNode.RewardConditions = new List<VCondition>();
      qNode.AvailabilityTriggers = new List<VTrigger>();
      qNode.StartTriggers = new List<VTrigger>();
      qNode.CompletionTriggers = new List<VTrigger>();
      qNode.RewardTriggers = new List<VTrigger>();

      VTrigger trigger = GetTrigger("a");
      VCondition cond = GetCondition("b");

      qNode.Rewards.Add(trigger);

      qNode.AvailabilityConditions.Add(cond);
      qNode.StartConditions.Add(cond);
      qNode.CompletionConditions.Add(cond);
      qNode.RewardConditions.Add(cond);

      qNode.AvailabilityTriggers.Add(trigger);
      qNode.StartTriggers.Add(trigger);
      qNode.CompletionTriggers.Add(trigger);
      qNode.RewardTriggers.Add(trigger);

      // Test
      Assert.IsTrue(inner.Rewards.Contains(trigger));

      Assert.IsTrue(inner.AvailabilityConditions.Contains(cond));
      Assert.IsTrue(inner.StartConditions.Contains(cond));
      Assert.IsTrue(inner.CompletionConditions.Contains(cond));
      Assert.IsTrue(inner.RewardConditions.Contains(cond));

      Assert.IsTrue(inner.AvailabilityTriggers.Contains(trigger));
      Assert.IsTrue(inner.StartTriggers.Contains(trigger));
      Assert.IsTrue(inner.CompletionTriggers.Contains(trigger));
      Assert.IsTrue(inner.RewardTriggers.Contains(trigger));
    }

    [Test]
    public void Quests_Bind_Changes_To_QuestNodes() {
      QuestGraph outer = GetQuest("outer");
      QuestGraph inner = GetQuest("inner");

      QuestNode qNode = outer.AddNode<QuestNode>();
      qNode.ChangeQuest(inner);

      inner.Rewards = new List<VTrigger>();
      inner.AvailabilityConditions = new List<VCondition>();
      inner.StartConditions = new List<VCondition>();
      inner.CompletionConditions = new List<VCondition>();
      inner.RewardConditions = new List<VCondition>();
      inner.AvailabilityTriggers = new List<VTrigger>();
      inner.StartTriggers = new List<VTrigger>();
      inner.CompletionTriggers = new List<VTrigger>();
      inner.RewardTriggers = new List<VTrigger>();

      VTrigger trigger = GetTrigger("a");
      VCondition cond = GetCondition("b");

      inner.Rewards.Add(trigger);

      inner.AvailabilityConditions.Add(cond);
      inner.StartConditions.Add(cond);
      inner.CompletionConditions.Add(cond);
      inner.RewardConditions.Add(cond);

      inner.AvailabilityTriggers.Add(trigger);
      inner.StartTriggers.Add(trigger);
      inner.CompletionTriggers.Add(trigger);
      inner.RewardTriggers.Add(trigger);

      // Test
      Assert.IsTrue(qNode.Rewards.Contains(trigger));

      Assert.IsTrue(qNode.AvailabilityConditions.Contains(cond));
      Assert.IsTrue(qNode.StartConditions.Contains(cond));
      Assert.IsTrue(qNode.CompletionConditions.Contains(cond));
      Assert.IsTrue(qNode.RewardConditions.Contains(cond));

      Assert.IsTrue(qNode.AvailabilityTriggers.Contains(trigger));
      Assert.IsTrue(qNode.StartTriggers.Contains(trigger));
      Assert.IsTrue(qNode.CompletionTriggers.Contains(trigger));
      Assert.IsTrue(qNode.RewardTriggers.Contains(trigger));
    }

    [Test]
    public void Quests_Populate_Data_To_QuestNodes_On_Set() {
      QuestGraph outer = GetQuest("outer");
      QuestGraph inner = GetQuest("inner");

      QuestNode qNode = outer.AddNode<QuestNode>();

      inner.Rewards = new List<VTrigger>();
      inner.AvailabilityConditions = new List<VCondition>();
      inner.StartConditions = new List<VCondition>();
      inner.CompletionConditions = new List<VCondition>();
      inner.RewardConditions = new List<VCondition>();
      inner.AvailabilityTriggers = new List<VTrigger>();
      inner.StartTriggers = new List<VTrigger>();
      inner.CompletionTriggers = new List<VTrigger>();
      inner.RewardTriggers = new List<VTrigger>();

      VTrigger trigger = GetTrigger("a");
      VCondition cond = GetCondition("b");

      inner.Rewards.Add(trigger);

      inner.AvailabilityConditions.Add(cond);
      inner.StartConditions.Add(cond);
      inner.CompletionConditions.Add(cond);
      inner.RewardConditions.Add(cond);

      inner.AvailabilityTriggers.Add(trigger);
      inner.StartTriggers.Add(trigger);
      inner.CompletionTriggers.Add(trigger);
      inner.RewardTriggers.Add(trigger);

      // Test
      qNode.ChangeQuest(inner);

      Assert.IsTrue(qNode.Rewards.Contains(trigger));

      Assert.IsTrue(qNode.AvailabilityConditions.Contains(cond));
      Assert.IsTrue(qNode.StartConditions.Contains(cond));
      Assert.IsTrue(qNode.CompletionConditions.Contains(cond));
      Assert.IsTrue(qNode.RewardConditions.Contains(cond));

      Assert.IsTrue(qNode.AvailabilityTriggers.Contains(trigger));
      Assert.IsTrue(qNode.StartTriggers.Contains(trigger));
      Assert.IsTrue(qNode.CompletionTriggers.Contains(trigger));
      Assert.IsTrue(qNode.RewardTriggers.Contains(trigger));
    }

    [Test]
    public void Quests_Bind_Changes_To_Start_End_Nodes() {
      QuestGraph outer = GetQuest("outer");

      outer.Rewards = new List<VTrigger>();
      outer.AvailabilityConditions = new List<VCondition>();
      outer.StartConditions = new List<VCondition>();
      outer.CompletionConditions = new List<VCondition>();
      outer.RewardConditions = new List<VCondition>();
      outer.AvailabilityTriggers = new List<VTrigger>();
      outer.StartTriggers = new List<VTrigger>();
      outer.CompletionTriggers = new List<VTrigger>();
      outer.RewardTriggers = new List<VTrigger>();

      VTrigger trigger = GetTrigger("a");
      VCondition cond = GetCondition("b");

      outer.Rewards.Add(trigger);

      outer.AvailabilityConditions.Add(cond);
      outer.StartConditions.Add(cond);
      outer.CompletionConditions.Add(cond);
      outer.RewardConditions.Add(cond);

      outer.AvailabilityTriggers.Add(trigger);
      outer.StartTriggers.Add(trigger);
      outer.CompletionTriggers.Add(trigger);
      outer.RewardTriggers.Add(trigger);

      QuestStartNode start = outer.FindNode<QuestStartNode>();
      QuestEndNode end = outer.FindNode<QuestEndNode>();

      Assert.IsTrue(end.Rewards.Contains(trigger));

      Assert.IsTrue(start.AvailabilityConditions.Contains(cond));
      Assert.IsTrue(start.StartConditions.Contains(cond));
      Assert.IsTrue(end.CompletionConditions.Contains(cond));
      Assert.IsTrue(end.RewardConditions.Contains(cond));

      Assert.IsTrue(start.AvailabilityTriggers.Contains(trigger));
      Assert.IsTrue(start.StartTriggers.Contains(trigger));
      Assert.IsTrue(end.CompletionTriggers.Contains(trigger));
      Assert.IsTrue(end.RewardTriggers.Contains(trigger));
    }

    [Test]
    public void Start_End_Nodes_Bind_Changes_To_Quests() {
      QuestGraph outer = GetQuest("outer");

      QuestStartNode start = outer.FindNode<QuestStartNode>();
      QuestEndNode end = outer.FindNode<QuestEndNode>();

      end.Rewards = new List<VTrigger>();
      start.AvailabilityConditions = new List<VCondition>();
      // start.StartConditions = new AutoTable<VCondition>();
      start.StartConditions = new List<VCondition>();
      end.CompletionConditions = new List<VCondition>(); // List<QuestPiece<VCondition>>();
      end.RewardConditions = new List<VCondition>(); //List<QuestPiece<VCondition>>();
      start.AvailabilityTriggers = new List<VTrigger>();//List<QuestPiece<VTrigger>>();
      start.StartTriggers = new List<VTrigger>(); // List<QuestPiece<VTrigger>>();
      end.CompletionTriggers = new List<VTrigger>();//List<QuestPiece<VTrigger>>();
      end.RewardTriggers = new List<VTrigger>();//List<QuestPiece<VTrigger>>();

      VTrigger trigger = GetTrigger("a");
      VCondition cond = GetCondition("b");

      end.Rewards.Add(trigger);

      start.AvailabilityConditions.Add(cond);
      start.StartConditions.Add(cond);
      end.CompletionConditions.Add(cond);
      end.RewardConditions.Add(cond);

      start.AvailabilityTriggers.Add(trigger);
      start.StartTriggers.Add(trigger);
      end.CompletionTriggers.Add(trigger);
      end.RewardTriggers.Add(trigger);

      Assert.IsTrue(outer.Rewards.Contains(trigger));

      Assert.IsTrue(outer.AvailabilityConditions.Contains(cond));
      Assert.IsTrue(outer.StartConditions.Contains(cond));
      Assert.IsTrue(outer.CompletionConditions.Contains(cond));
      Assert.IsTrue(outer.RewardConditions.Contains(cond));

      Assert.IsTrue(outer.AvailabilityTriggers.Contains(trigger));
      Assert.IsTrue(outer.StartTriggers.Contains(trigger));
      Assert.IsTrue(outer.CompletionTriggers.Contains(trigger));
      Assert.IsTrue(outer.RewardTriggers.Contains(trigger));
    }

    [Test]
    public void Can_Initialize_Quest_Narrator() {
      SetupNarrator();
      Assert.IsTrue(Journey.Initialized);
    }

    [Test]
    public void Can_Initialize_Quest() {
      SetupNarrator();
      Journey.SetQuest(GetTrivialQuest("a"));
      Assert.IsTrue(Journey.HasQuest);
    }

    [Test]
    public void Can_Traverse_Trivial_Quest() {
      SetupNarrator();

      Journey.SetQuest(GetTrivialQuest("a"));
      Journey.Begin();

      Assert.IsTrue(Journey.Finished);
    }

    [Test]
    public void Can_Traverse_Quest_With_Objectives() {
      SetupNarrator();

      BuildSimpleObjectiveQuest();
      Journey.SetQuest(GetQuest(SIMPLE_OBJECTIVE_QUEST));

      Journey.Begin();
      Assert.IsFalse(Journey.Finished);

      foreach (ObjectiveNode n in Journey.Quest.FindNodes<ObjectiveNode>()) {
        ((VCondition) n.Condition).Variable.Value = true;
      }

      Journey.Step();
      Assert.IsTrue(Journey.Finished);
    }

    [Test]
    public void Quest_Marked_As_Started() {
      SetupNarrator();

      Journey.SetQuest(GetSimpleQuest("a"));
      Journey.Begin();

      Assert.IsFalse(Journey.Finished);
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Started);
    }

    [Test]
    public void Can_Traverse_Quest_With_Availability_Conditions() {
      SetupNarrator();
      BuildAvailabilityQuest();

      Journey.SetQuest(GetQuest(TRIVIAL_QUEST_WITH_AVAILABILITY_CONDITIONS));
      Journey.Begin();

      Assert.IsTrue(Journey.CurrentNode?.GetType() == typeof(QuestStartNode));
      Assert.IsTrue(Journey.CurrentNode?.Progress == QuestProgress.Unavailable);
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Unavailable);

      VCondition conditionA = GetCondition("a");
      VCondition conditionB = GetCondition("b");

      conditionA.Variable.Value = true;

      Journey.Step();
      Assert.IsTrue(Journey.CurrentNode?.GetType() == typeof(QuestStartNode));
      Assert.IsTrue(Journey.CurrentNode?.Progress == QuestProgress.Unavailable);
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Unavailable);

      conditionB.Variable.Value = true;
      Journey.Step();

      Assert.IsTrue(Journey.Finished);
    }

    [Test]
    public void Can_Traverse_Quest_With_Start_Conditions() {
      SetupNarrator();
      BuildStartConditionsQuest();

      Journey.SetQuest(GetQuest(TRIVIAL_QUEST_WITH_START_CONDITIONS));
      Journey.Begin();

      Assert.IsTrue(Journey.CurrentNode?.GetType() == typeof(QuestStartNode));
      Assert.IsTrue(Journey.CurrentNode?.Progress == QuestProgress.Available);
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Available);

      VCondition conditionA = GetCondition("a");
      VCondition conditionB = GetCondition("b");

      conditionA.Variable.Value = true;

      Journey.Step();
      Assert.IsTrue(Journey.CurrentNode?.GetType() == typeof(QuestStartNode));
      Assert.IsTrue(Journey.CurrentNode?.Progress == QuestProgress.Available);
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Available);

      conditionB.Variable.Value = true;

      Journey.Step();
      Assert.IsTrue(Journey.Finished);
    }

    [Test]
    public void Can_Traverse_Quest_With_Availability_And_Start_Conditions() {
      SetupNarrator();
      BuildAvailStartConditionsQuest();

      Journey.SetQuest(GetQuest(TRIVIAL_QUEST_WITH_AVAIL_START_CONDITIONS));
      Journey.Begin();

      Assert.IsTrue(Journey.CurrentNode?.GetType() == typeof(QuestStartNode));
      Assert.IsTrue(Journey.CurrentNode?.Progress == QuestProgress.Unavailable);
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Unavailable);

      VCondition conditionA = GetCondition("a");
      VCondition conditionB = GetCondition("b");

      conditionA.Variable.Value = true;

      Journey.Step();
      Assert.IsTrue(Journey.CurrentNode?.GetType() == typeof(QuestStartNode));
      Assert.IsTrue(Journey.CurrentNode?.Progress == QuestProgress.Available);
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Available);

      conditionB.Variable.Value = true;

      Journey.Step();
      Assert.IsTrue(Journey.Finished);
    }

    [Test]
    public void Can_Traverse_Quest_With_Completion_Conditions() {
      SetupNarrator();
      BuildCompConditionsQuest();

      Journey.SetQuest(GetQuest(TRIVIAL_QUEST_WITH_COMP_CONDITIONS));
      Journey.Begin();

      Assert.IsTrue(Journey.CurrentNode?.GetType() == typeof(QuestEndNode));
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Started);

      VCondition conditionA = GetCondition("a");
      VCondition conditionB = GetCondition("b");

      conditionA.Variable.Value = true;

      Journey.Step();
      Assert.IsTrue(Journey.CurrentNode?.GetType() == typeof(QuestEndNode));
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Started);

      conditionB.Variable.Value = true;

      Journey.Step();
      Assert.IsTrue(Journey.Finished);
    }

    [Test]
    public void Can_Traverse_Quest_With_Rewards_Conditions() {
      SetupNarrator();
      BuildRewardConditionsQuest();

      Journey.SetQuest(GetQuest(TRIVIAL_QUEST_WITH_REWARD_CONDITIONS));
      Journey.Begin();

      Assert.IsTrue(Journey.CurrentNode?.GetType() == typeof(QuestEndNode));
      Assert.IsTrue(Journey.CurrentNode?.Progress == QuestProgress.Completed);
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Completed);

      VCondition conditionA = GetCondition("a");
      VCondition conditionB = GetCondition("b");

      conditionA.Variable.Value = true;

      Journey.Step();
      Assert.IsTrue(Journey.CurrentNode?.GetType() == typeof(QuestEndNode));
      Assert.IsTrue(Journey.CurrentNode?.Progress == QuestProgress.Completed);
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Completed);

      conditionB.Variable.Value = true;

      Journey.Step();
      Assert.IsTrue(Journey.Finished);
    }

    [Test]
    public void Can_Traverse_Quest_With_Completion_And_Rewards_Conditions() {
      SetupNarrator();
      BuildCompRewardsQuest();

      Journey.SetQuest(GetQuest(TRIVIAL_QUEST_WITH_COMP_REWARD_CONDITIONS));
      Journey.Begin();

      Assert.IsTrue(Journey.CurrentNode?.GetType() == typeof(QuestEndNode));
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Started);

      VCondition conditionA = GetCondition("a");
      VCondition conditionB = GetCondition("b");

      conditionA.Variable.Value = true;

      Journey.Step();
      Assert.IsTrue(Journey.CurrentNode?.GetType() == typeof(QuestEndNode));
      Assert.IsTrue(Journey.CurrentNode?.Progress == QuestProgress.Completed);
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Completed);

      conditionB.Variable.Value = true;

      Journey.Step();
      Assert.IsTrue(Journey.Finished);
    }

    [Test]
    public void Can_Traverse_Parallel_Objective_Quest() {
      SetupNarrator();
      BuildSimpleParallelQuest();

      Journey.SetQuest(GetQuest(QUEST_PARALLEL_OBJECTIVES));
      Journey.Begin();

      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Started);
      Assert.IsTrue(Journey.CurrentNodes.Count == 2);

      GetCondition("a").Variable.Value = true;
      Journey.Step();

      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Started);
      Assert.IsTrue(Journey.CurrentNodes.Count == 2);

      GetCondition("c").Variable.Value = true;
      Journey.Step();

      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Started);
      Assert.IsTrue(Journey.CurrentNodes.Count == 2);
      Assert.IsTrue(Journey.CurrentNodes.Contains(Journey.Quest.FindNode<QuestEndNode>()));

      GetCondition("b").Variable.Value = true;
      Journey.Step();

      Assert.IsTrue(Journey.Finished);
    }

    [Test]
    public void Can_Traverse_Parallel_Objective_Quest_Single_Step() {
      SetupNarrator();
      BuildSimpleParallelQuest();

      Journey.SetQuest(GetQuest(QUEST_PARALLEL_OBJECTIVES));
      Journey.Begin();

      GetCondition("a").Variable.Value = true;
      GetCondition("b").Variable.Value = true;
      GetCondition("c").Variable.Value = true;

      Journey.Step();

      Assert.IsTrue(Journey.Finished);
    }

    [Test]
    public void Can_Traverse_Quest_With_Nested_Quest() {
      SetupNarrator();
      BuildNestedQuest();

      Journey.SetQuest(GetQuest(NESTED_QUEST_OUTER));
      Journey.Begin();

      Assert.IsFalse(Journey.Finished);

      foreach (string cond in Conditions.Keys) {
        VCondition condition = GetCondition(cond);
        condition.Variable.Value = true;
      }

      Journey.Step();

      Assert.IsTrue(Journey.Finished);
      Assert.IsTrue(Journey.StepCount > 3);
    }

    [Test]
    public void Can_Traverse_Chained_Nested_Quests() {
      SetupNarrator();
      BuildChainedNestedQuest();

      Journey.SetQuest(GetQuest(CHAINED_NESTED));
      Journey.Begin();

      Assert.IsFalse(Journey.Finished);

      foreach (string cond in Conditions.Keys) {
        VCondition condition = GetCondition(cond);
        condition.Variable.Value = true;
      }

      Journey.Step();

      Assert.IsTrue(Journey.Finished);
      Assert.IsTrue(Journey.StepCount > 4);
    }

    [Test]
    public void Can_Traverse_Deeply_Nested_Quests() {
      SetupNarrator();
      BuildDeeplyNestedQuest();

      Journey.SetQuest(GetQuest(DEEPLY_NESTED));
      Journey.Begin();

      Assert.IsTrue(Journey.Finished);
      Assert.IsTrue(Journey.StepCount > 6);
    }

    [Test]
    public void Can_Traverse_Nested_Parallel_Quests() {
      SetupNarrator();
      BuildNestedParallelQuest();

      Journey.SetQuest(GetQuest(NESTED_PARALLEL));
      Journey.Begin();

      Assert.IsFalse(Journey.Finished);
      Assert.IsTrue(Journey.CurrentNodes.Count == 3);

      foreach (string cond in Conditions.Keys) {
        VCondition condition = GetCondition(cond);
        condition.Variable.Value = true;
      }


      Journey.Step();
      Assert.IsTrue(Journey.Finished);
      Assert.AreEqual(9, Journey.StepCount);
    }

    [Test]
    public void Can_Traverse_Parallel_Nested_Quests() {
      SetupNarrator();
      BuildParallelNestedQuest();

      Journey.SetQuest(GetQuest(PARALLEL_NESTED));
      Journey.Begin();

      Assert.IsFalse(Journey.Finished);
      Assert.IsTrue(Journey.CurrentNodes.Count == 3);

      foreach (string cond in Conditions.Keys) {
        VCondition condition = GetCondition(cond);
        condition.Variable.Value = true;
      }

      Journey.Step();

      Assert.IsTrue(Journey.Finished);
      Assert.AreEqual(15, Journey.StepCount);
    }

    [Test]
    public void Can_Traverse_Mixed_Parallel_Quests() {
      SetupNarrator();
      BuildMixedParallelQuest();
      Journey.SetQuest(GetQuest(MIXED_PARALLEL));
      Journey.Begin();

      Assert.IsFalse(Journey.Finished);

      foreach (string cond in Conditions.Keys) {
        VCondition condition = GetCondition(cond);
        condition.Variable.Value = true;
      }

      Journey.Step();
      Assert.IsTrue(Journey.Finished);
      Assert.AreEqual(11, Journey.StepCount);
    }

    [Test]
    public void Quests_Give_Rewards() {
      SetupNarrator();
      BuildRewardQuest();

      Journey.SetQuest(GetQuest(HAS_REWARD));
      Journey.Begin();

      VTrigger reward = GetTrigger("reward");
      Assert.IsTrue(reward.Variable.BoolValue);
    }

    [Test]
    public void Objectives_Give_Rewards() {
      SetupNarrator();
      BuildObjectiveRewardQuest();

      Journey.SetQuest(GetSimpleQuest(OBJECTIVE_REWARD));
      Journey.Begin();

      VTrigger reward = GetTrigger("reward");
      Assert.IsTrue(reward.Variable.BoolValue);
    }

    [Test]
    public void Objectives_Can_Trigger_Changes_When_Started() {
      SetupNarrator();
      BuildObjectiveStartTriggerQuest();

      Journey.SetQuest(GetSimpleQuest(OBJECTIVE_START_TRIGGER));
      Journey.Begin();

      VTrigger trigger = GetTrigger("a");
      Assert.IsTrue(trigger.Variable.BoolValue);
    }

    [Test]
    public void Objectives_Can_Trigger_Changes_When_Completed() {
      SetupNarrator();
      BuildObjectiveCompleteTriggerQuest();

      Journey.SetQuest(GetSimpleQuest(OBJECTIVE_COMPLETE_TRIGGER));
      Journey.Begin();

      VTrigger trigger = GetTrigger("a");
      Assert.IsTrue(trigger.Variable.Value);
    }

    [Test]
    public void Quests_Can_Trigger_Changes_When_Available() {
      SetupNarrator();
      BuildQuestAvailableTriggerQuest();

      Journey.SetQuest(GetTrivialQuest(QUEST_AVAILABLE_TRIGGER));
      Journey.Begin();

      VTrigger trigger = GetTrigger("a");
      Assert.IsTrue(trigger.Variable.BoolValue);
    }

    [Test]
    public void Quests_Can_Trigger_Changes_When_Started() {
      SetupNarrator();
      BuildQuestStartedTriggerQuest();

      Journey.SetQuest(GetTrivialQuest(QUEST_STARTED_TRIGGER));
      Journey.Begin();

      VTrigger trigger = GetTrigger("a");
      Assert.IsTrue(trigger.Variable.BoolValue);
    }

    [Test]
    public void Quests_Can_Trigger_Changes_When_Completed() {
      SetupNarrator();
      BuildQuestCompletedTriggerQuest();

      Journey.SetQuest(GetTrivialQuest(QUEST_COMPLETE_TRIGGER));
      Journey.Begin();

      VTrigger trigger = GetTrigger("a");
      Assert.IsTrue(trigger.Variable.BoolValue);
    }

    [Test]
    public void Quests_Can_Trigger_Changes_After_Rewards_Collected() {
      SetupNarrator();
      BuildQuestRewardTriggerQuest();

      Journey.SetQuest(GetTrivialQuest(QUEST_REWARD_TRIGGER));
      Journey.Begin();

      VTrigger trigger = GetTrigger("a");
      Assert.IsTrue(trigger.Variable.BoolValue);
    }

    [Test]
    public void Quests_Save_And_Resume_Progress() {
      SetupNarrator();
      BuildMixedParallelQuest();
      Journey.LoadQuest(MakeFullName(MIXED_PARALLEL)+"_quest");
      Journey.Begin();

      Assert.IsFalse(Journey.Finished);

      int expectedCount = Journey.StepCount;
      int expectedNodeCount = Journey.CurrentNodes.Count;

      Journey.SaveProgress();
      Journey.Reset();

      VSave.Reset(true);
      VSave.LoadSlots();
      VSave.ChooseSlot("test");

      Assert.IsFalse(Journey.HasQuest);
      Assert.AreEqual(0, Journey.StepCount);

      SetupNarrator();
      Journey.Resume();

      Assert.IsTrue(Journey.Initialized);
      Assert.IsTrue(Journey.HasQuest);
      Assert.AreEqual(expectedCount, Journey.StepCount);
      Assert.AreEqual(expectedNodeCount, Journey.CurrentNodes.Count);

      foreach (string cond in Conditions.Keys) {
        VCondition condition = GetCondition(cond);
        condition.Variable.Value = true;
      }

      Journey.Step();
      Assert.IsTrue(Journey.Finished);
      Assert.AreEqual(11, Journey.StepCount);
    }

    [Test]
    public void StartQuestNode_Conditions_And_Triggers_Persist() {
      SetupNarrator();
      BuildStartStatePeristenceCheckGraph();
      Journey.LoadQuest(MakeFullName("trivial_quest_"+START_PERSISTENCE));
      Journey.Begin();

      Assert.IsFalse(Journey.Finished);

      VCondition availCond = GetCondition("avail");
      VCondition startCond = GetCondition("start");
      VTrigger availTrig = GetTrigger("avail");
      VTrigger startTrig = GetTrigger("start");

      Assert.IsFalse(availCond.Variable.Value);
      Assert.IsFalse(startCond.Variable.Value);
      Assert.IsFalse(availTrig.Variable.Value);
      Assert.IsFalse(startTrig.Variable.Value);

      Reset();
      Resume();

      Assert.IsTrue(Journey.CurrentNode.GetType() == typeof (QuestStartNode));

      availCond.Variable.Value = true;
      Journey.Step();

      Assert.IsTrue(availCond.IsMet());
      Assert.IsTrue(availTrig.Variable.Value);

      Reset();
      Resume();

      Assert.IsTrue(Journey.CurrentNode.GetType() == typeof (QuestStartNode));

      startCond.Variable.Value = true;
      Journey.Step();

      Assert.IsTrue(startCond.IsMet());
      Assert.IsTrue(availTrig.Variable.Value);
      Assert.IsTrue(Journey.Finished);

      Reset();
      Resume();

      Assert.IsTrue(availCond.IsMet());
      Assert.IsTrue(availTrig.Variable.Value);
      Assert.IsTrue(startCond.IsMet());
      Assert.IsTrue(availTrig.Variable.Value);

      Assert.IsTrue(Journey.Finished);
    }

    [Test]
    public void EndQuestNode_Conditions_And_Triggers_Persist() {
      SetupNarrator();
      BuildEndStatePeristenceCheckGraph();

      Journey.LoadQuest(MakeFullName("trivial_quest_"+END_PERSISTENCE));
      Journey.Begin();

      Assert.IsFalse(Journey.Finished);

      VCondition compCond = GetCondition("comp");
      VCondition rewardCond = GetCondition("reward");
      VTrigger compTrig = GetTrigger("comp");
      VTrigger rewardTrig = GetTrigger("reward_trig");
      VTrigger reward = GetTrigger("reward");

      Assert.IsFalse(compCond.Variable.Value);
      Assert.IsFalse(rewardCond.Variable.Value);
      Assert.IsFalse(compTrig.Variable.Value);
      Assert.IsFalse(rewardTrig.Variable.Value);
      Assert.IsFalse(reward.Variable.Value);

      Reset();
      Resume();

      Assert.IsTrue(Journey.CurrentNode.GetType() == typeof(QuestEndNode));

      compCond.Variable.Value = true;
      Journey.Step();

      Assert.IsTrue(compCond.IsMet());
      Assert.IsTrue(compTrig.Variable.Value);

      Reset();
      Resume();

      Assert.IsTrue(Journey.CurrentNode.GetType() == typeof (QuestEndNode));

      rewardCond.Variable.Value = true;
      Journey.Step();

      Assert.IsTrue(rewardCond.IsMet());
      Assert.IsTrue(rewardTrig.Variable.Value);
      Assert.IsTrue(reward.Variable.Value);
      Assert.IsTrue(Journey.Finished);

      Reset();
      Resume();

      Assert.IsTrue(compCond.Variable.Value);
      Assert.IsTrue(rewardCond.Variable.Value);
      Assert.IsTrue(compTrig.Variable.Value);
      Assert.IsTrue(rewardTrig.Variable.Value);
      Assert.IsTrue(reward.Variable.Value);

      Assert.IsTrue(Journey.Finished);
    }

    [Test]
    public void ObjectiveNode_Conditions_And_Triggers_Persist() {
      SetupNarrator();
      BuildObjectivePersistenceCheckGraph();

      Journey.LoadQuest(MakeFullName("simple_quest"+OBJ_PERSISTENCE));
      Journey.Begin();

      Assert.IsFalse(Journey.Finished);

      VCondition cond = GetCondition("cond");
      VTrigger startTrig = GetTrigger("start");
      VTrigger compTrig = GetTrigger("comp");
      VTrigger reward = GetTrigger("reward");

      Assert.IsFalse(cond.Variable.Value);
      Assert.IsTrue(startTrig.Variable.Value);
      Assert.IsFalse(compTrig.Variable.Value);
      Assert.IsFalse(reward.Variable.Value);

      Reset();
      Resume();

      Assert.IsFalse(cond.Variable.Value);
      Assert.IsTrue(startTrig.Variable.Value);
      Assert.IsFalse(compTrig.Variable.Value);
      Assert.IsFalse(reward.Variable.Value);
      Assert.IsTrue(Journey.CurrentNode.GetType() == typeof(ObjectiveNode));

      cond.Variable.Value = true;
      Journey.Step();

      Assert.IsTrue(cond.Variable.Value);
      Assert.IsTrue(startTrig.Variable.Value);
      Assert.IsTrue(compTrig.Variable.Value);
      Assert.IsTrue(reward.Variable.Value);
      Assert.IsTrue(Journey.Finished);

      Reset();
      Resume();

      Assert.IsTrue(cond.Variable.Value);
      Assert.IsTrue(startTrig.Variable.Value);
      Assert.IsTrue(compTrig.Variable.Value);
      Assert.IsTrue(reward.Variable.Value);

      Assert.IsTrue(Journey.Finished);
    }

    //-------------------------------------------------------------------------
    // Quest Builders
    //-------------------------------------------------------------------------
    private QuestGraph BuildSimpleObjectiveQuest() {
      QuestGraph quest = GetQuest(SIMPLE_OBJECTIVE_QUEST);
      QuestStartNode startNode = quest.FindNode<QuestStartNode>();
      QuestEndNode endNode = quest.FindNode<QuestEndNode>();

      ObjectiveNode objectiveA = quest.AddNode<ObjectiveNode>();
      ObjectiveNode objectiveB = quest.AddNode<ObjectiveNode>();

      startNode.ConnectTo(objectiveA);
      objectiveA.ConnectTo(objectiveB);
      objectiveB.ConnectTo(endNode);

      objectiveA.Condition = GetCondition("a");
      objectiveB.Condition = GetCondition("b");

      return quest;
    }

    private QuestGraph BuildAvailabilityQuest() {
      QuestGraph quest = GetQuest(TRIVIAL_QUEST_WITH_AVAILABILITY_CONDITIONS);
      QuestStartNode startNode = quest.FindNode<QuestStartNode>();

      startNode.AddAvailabilityCondition(GetCondition("a"));
      startNode.AddAvailabilityCondition(GetCondition("b"));

      QuestEndNode endNode = quest.FindNode<QuestEndNode>();
      startNode.ConnectTo(endNode);

      return quest;
    }

    private QuestGraph BuildStartConditionsQuest() {
      QuestGraph quest = GetQuest(TRIVIAL_QUEST_WITH_START_CONDITIONS);
      QuestStartNode startNode = quest.FindNode<QuestStartNode>();

      startNode.AddStartCondition(GetCondition("a"));
      startNode.AddStartCondition(GetCondition("b"));

      QuestEndNode endNode = quest.FindNode<QuestEndNode>();
      startNode.ConnectTo(endNode);

      return quest;
    }

    private QuestGraph BuildAvailStartConditionsQuest() {
      QuestGraph quest = GetQuest(TRIVIAL_QUEST_WITH_AVAIL_START_CONDITIONS);
      QuestStartNode startNode = quest.FindNode<QuestStartNode>();

      startNode.AddAvailabilityCondition(GetCondition("a"));
      startNode.AddStartCondition(GetCondition("b"));

      QuestEndNode endNode = quest.FindNode<QuestEndNode>();
      startNode.ConnectTo(endNode);

      return quest;
    }

    private QuestGraph BuildCompConditionsQuest() {
      QuestGraph quest = GetQuest(TRIVIAL_QUEST_WITH_COMP_CONDITIONS);
      QuestEndNode endNode = quest.FindNode<QuestEndNode>();

      endNode.AddCompletionCondition(GetCondition("a"));
      endNode.AddCompletionCondition(GetCondition("b"));

      QuestStartNode startNode = quest.FindNode<QuestStartNode>();
      startNode.ConnectTo(endNode);

      return quest;
    }

    private QuestGraph BuildRewardConditionsQuest() {
      QuestGraph quest = GetQuest(TRIVIAL_QUEST_WITH_REWARD_CONDITIONS);
      QuestEndNode endNode = quest.FindNode<QuestEndNode>();

      endNode.AddRewardCondition(GetCondition("a"));
      endNode.AddRewardCondition(GetCondition("b"));

      QuestStartNode startNode = quest.FindNode<QuestStartNode>();
      startNode.ConnectTo(endNode);

      return quest;
    }

    private QuestGraph BuildCompRewardsQuest() {
      QuestGraph quest = GetQuest(TRIVIAL_QUEST_WITH_COMP_REWARD_CONDITIONS);
      QuestEndNode endNode = quest.FindNode<QuestEndNode>();

      endNode.AddCompletionCondition(GetCondition("a"));
      endNode.AddRewardCondition(GetCondition("b"));

      QuestStartNode startNode = quest.FindNode<QuestStartNode>();
      startNode.ConnectTo(endNode);

      return quest;
    }

    private QuestGraph BuildSimpleParallelQuest() {
      QuestGraph quest = GetQuest(QUEST_PARALLEL_OBJECTIVES);
      QuestStartNode start = quest.FindNode<QuestStartNode>();
      QuestEndNode end = quest.FindNode<QuestEndNode>();

      ObjectiveNode objectiveA = quest.AddNode<ObjectiveNode>();
      ObjectiveNode objectiveB = quest.AddNode<ObjectiveNode>();
      ObjectiveNode objectiveC = quest.AddNode<ObjectiveNode>();

      start.ConnectTo(objectiveA);
      objectiveA.ConnectTo(objectiveB);

      start.ConnectTo(objectiveC);

      objectiveB.ConnectTo(end);
      objectiveC.ConnectTo(end);

      objectiveA.Condition = GetCondition("a");
      objectiveB.Condition = GetCondition("b");
      objectiveC.Condition = GetCondition("c");

      return quest;
    }

    private QuestGraph BuildNestedQuest() {
      QuestGraph outerQuest = GetQuest(NESTED_QUEST_OUTER);

      QuestNode questNode = outerQuest.AddNode<QuestNode>();
      questNode.ChangeQuest(GetSimpleQuest("inner"));

      QuestStartNode outerStartNode = outerQuest.FindNode<QuestStartNode>();
      QuestEndNode outerEndNode = outerQuest.FindNode<QuestEndNode>();

      outerStartNode.ConnectTo(questNode);
      questNode.ConnectTo(outerEndNode);

      return outerQuest;
    }

    private QuestGraph BuildChainedNestedQuest() {
      QuestGraph outer = GetQuest(CHAINED_NESTED);
      QuestGraph innerA = GetTrivialQuest("a");
      QuestGraph innerB = GetSimpleQuest("b");


      QuestStartNode start = outer.FindNode<QuestStartNode>();

      QuestNode qNodeA = outer.AddNode<QuestNode>();
      qNodeA.ChangeQuest(innerA);

      QuestNode qNodeB = outer.AddNode<QuestNode>();
      qNodeB.ChangeQuest(innerB);

      QuestEndNode end = outer.FindNode<QuestEndNode>();

      start.ConnectTo(qNodeA);
      qNodeA.ConnectTo(qNodeB);
      qNodeB.ConnectTo(end);

      return outer;
    }

    private QuestGraph BuildDeeplyNestedQuest() {
      QuestGraph outer = GetQuest(DEEPLY_NESTED);
      QuestGraph nested = GetQuest(DEEPLY_NESTED + "_inner");
      QuestGraph deepest = GetTrivialQuest("innermost");

      QuestNode outerQNode = outer.AddNode<QuestNode>();
      outerQNode.ChangeQuest(nested);

      QuestNode nestedQNode = nested.AddNode<QuestNode>();
      nestedQNode.ChangeQuest(deepest);

      QuestStartNode outerStart = outer.FindNode<QuestStartNode>();
      QuestEndNode outerEnd = outer.FindNode<QuestEndNode>();
      outerStart.ConnectTo(outerQNode);
      outerQNode.ConnectTo(outerEnd);

      QuestStartNode nestedStart = nested.FindNode<QuestStartNode>();
      QuestEndNode nestedEnd = nested.FindNode<QuestEndNode>();
      nestedStart.ConnectTo(nestedQNode);
      nestedQNode.ConnectTo(nestedEnd);

      return outer;
    }

    private QuestGraph BuildNestedParallelQuest() {
      QuestGraph outer = GetQuest(NESTED_PARALLEL);
      QuestGraph inner = GetQuest(NESTED_PARALLEL + "_inner");

      QuestNode qNode = outer.AddNode<QuestNode>();
      qNode.name = "qnode_outer";
      qNode.ChangeQuest(inner);

      ObjectiveNode objectiveA = inner.AddNode<ObjectiveNode>();
      ObjectiveNode objectiveB = inner.AddNode<ObjectiveNode>();
      ObjectiveNode objectiveC = inner.AddNode<ObjectiveNode>();

      objectiveA.name = "A";
      objectiveB.name = "B";
      objectiveC.name = "C";

      objectiveA.Condition = GetCondition("a");
      objectiveB.Condition = GetCondition("b");
      objectiveC.Condition = GetCondition("c");

      QuestStartNode innerStart = inner.FindNode<QuestStartNode>();
      QuestEndNode innerEnd = inner.FindNode<QuestEndNode>();
      innerStart.ConnectTo(objectiveA);
      innerStart.ConnectTo(objectiveB);
      innerStart.ConnectTo(objectiveC);

      objectiveA.ConnectTo(innerEnd);
      objectiveB.ConnectTo(innerEnd);
      objectiveC.ConnectTo(innerEnd);

      QuestStartNode outerStart = outer.FindNode<QuestStartNode>();
      QuestEndNode outerEnd = outer.FindNode<QuestEndNode>();

      outerStart.ConnectTo(qNode);
      qNode.ConnectTo(outerEnd);

      return outer;
    }

    private QuestGraph BuildParallelNestedQuest() {
      QuestGraph outer = GetQuest(PARALLEL_NESTED);
      outer.name = "name";

      QuestNode qNodeA = outer.AddNode<QuestNode>();
      QuestNode qNodeB = outer.AddNode<QuestNode>();
      QuestNode qNodeC = outer.AddNode<QuestNode>();
      qNodeA.name = "quest_node_a";
      qNodeB.name = "quest_node_b";
      qNodeC.name = "quest_node_c";
      qNodeA.ChangeQuest(GetSimpleQuest("a"));
      qNodeB.ChangeQuest(GetSimpleQuest("b"));
      qNodeC.ChangeQuest(GetSimpleQuest("c"));

      QuestStartNode outerStart = outer.FindNode<QuestStartNode>();
      QuestEndNode outerEnd = outer.FindNode<QuestEndNode>();

      outerStart.ConnectTo(qNodeA);
      outerStart.ConnectTo(qNodeB);
      outerStart.ConnectTo(qNodeC);

      qNodeA.ConnectTo(outerEnd);
      qNodeB.ConnectTo(outerEnd);
      qNodeC.ConnectTo(outerEnd);

      return outer;
    }

    private QuestGraph BuildMixedParallelQuest() {
      QuestGraph outer = GetQuest(MIXED_PARALLEL);
      outer.name = MIXED_PARALLEL;

      QuestGraph innerA = GetTrivialQuest("a");
      QuestGraph innerB = GetSimpleQuest("b");

      QuestNode qNodeA = outer.AddNode<QuestNode>();
      qNodeA.ChangeQuest(innerA);
      qNodeA.name = "qnode_A";

      QuestNode qNodeB = outer.AddNode<QuestNode>();
      qNodeB.ChangeQuest(innerB);
      qNodeB.name = "qnode_B";

      ObjectiveNode outerObjective = outer.AddNode<ObjectiveNode>();
      outerObjective.name = "outer_obj";
      outerObjective.Condition = GetCondition("outer");

      var outerStart = outer.FindNode<QuestStartNode>();
      var outerEnd = outer.FindNode<QuestEndNode>();

      outerStart.ConnectTo(qNodeA);
      outerStart.ConnectTo(qNodeB);
      qNodeA.ConnectTo(outerObjective);
      outerObjective.ConnectTo(outerEnd);
      qNodeB.ConnectTo(outerEnd);

      return outer;
    }

    public QuestGraph BuildRewardQuest() {
      QuestGraph outer = GetQuest(HAS_REWARD);
      QuestGraph inner = GetTrivialQuest("a");
      QuestNode qNode = outer.AddNode<QuestNode>();
      qNode.ChangeQuest(inner);

      QuestStartNode start = outer.FindNode<QuestStartNode>();
      QuestEndNode end = outer.FindNode<QuestEndNode>();
      
      start.ConnectTo(qNode);
      qNode.ConnectTo(end);

      QuestEndNode innerEnd = inner.FindNode<QuestEndNode>();
      VTrigger reward = GetTrigger("reward");
      innerEnd.AddReward(reward);

      return outer;
    }

    public QuestGraph BuildObjectiveRewardQuest() {
      QuestGraph quest = GetSimpleQuest(OBJECTIVE_REWARD);
      ObjectiveNode objective = quest.FindNode<ObjectiveNode>();
      ((VCondition)objective.Condition).Variable.Value = true;

      objective.AddReward(GetTrigger("reward"));
      return quest;
    }

    public QuestGraph BuildObjectiveStartTriggerQuest() {
      QuestGraph quest = GetSimpleQuest(OBJECTIVE_START_TRIGGER);
      ObjectiveNode objective = quest.FindNode<ObjectiveNode>();
      objective.AddStartTrigger(GetTrigger("a"));
      return quest;
    }

    public QuestGraph BuildObjectiveCompleteTriggerQuest() {
      QuestGraph quest = GetSimpleQuest(OBJECTIVE_COMPLETE_TRIGGER);
      ObjectiveNode objective = quest.FindNode<ObjectiveNode>();
      ((VCondition)objective.Condition).Variable.Value = true;
      objective.AddCompletionTrigger(GetTrigger("a"));
      return quest;
    }

    public QuestGraph BuildQuestAvailableTriggerQuest() {
      QuestGraph quest = GetTrivialQuest(QUEST_AVAILABLE_TRIGGER);

      VTrigger setter = GetTrigger("a");
      setter.BoolValue = true;
      quest.AvailabilityTriggers.Add(setter);
      return quest;
    }

    public QuestGraph BuildQuestStartedTriggerQuest() {
      QuestGraph quest = GetTrivialQuest(QUEST_STARTED_TRIGGER);

      VTrigger setter = GetTrigger("a");
      setter.BoolValue = true;
      quest.StartTriggers.Add(setter);
      return quest;
    }

    public QuestGraph BuildQuestCompletedTriggerQuest() {
      QuestGraph quest = GetTrivialQuest(QUEST_COMPLETE_TRIGGER);

      VTrigger setter = GetTrigger("a");
      setter.BoolValue = true;
      quest.CompletionTriggers.Add(setter);
      return quest;
    }

    public QuestGraph BuildQuestRewardTriggerQuest() {
      QuestGraph quest = GetTrivialQuest(QUEST_REWARD_TRIGGER);

      VTrigger setter = GetTrigger("a");
      setter.BoolValue = true;
      quest.RewardTriggers.Add(setter);
      return quest;
    }

    public QuestGraph BuildStartStatePeristenceCheckGraph() {
      QuestGraph quest = GetTrivialQuest(START_PERSISTENCE);

      quest.AvailabilityConditions = new List<VCondition>();
      quest.StartConditions = new List<VCondition>();
      quest.AvailabilityTriggers = new List<VTrigger>();
      quest.StartTriggers = new List<VTrigger>();

      VCondition availCond = GetCondition("avail");
      availCond.Variable = GetVariable("avail_cond");

      VCondition startCond = GetCondition("start");
      startCond.Variable = GetVariable("start_cond");

      VTrigger availTrig = GetTrigger("avail");
      availTrig.Variable = GetVariable("avail_trig");

      VTrigger startTrig = GetTrigger("start");
      startTrig.Variable = GetVariable("start_trig");

      quest.AvailabilityConditions.Add(availCond);
      quest.StartConditions.Add(startCond);
      quest.AvailabilityTriggers.Add(availTrig);
      quest.StartTriggers.Add(startTrig);

      return quest;
    }

    public QuestGraph BuildEndStatePeristenceCheckGraph() {
      QuestGraph quest = GetTrivialQuest(END_PERSISTENCE);
      
      quest.CompletionConditions = new List<VCondition>();
      quest.CompletionTriggers = new List<VTrigger>();
      quest.RewardConditions = new List<VCondition>();
      quest.RewardTriggers = new List<VTrigger>();
      quest.Rewards = new List<VTrigger>();

      VCondition compCond = GetCondition("comp");
      compCond.Variable = GetVariable("comp_cond");

      VCondition rewardCond = GetCondition("reward");
      rewardCond.Variable = GetVariable("reward_cond");

      VTrigger compTrig = GetTrigger("comp");
      compTrig.Variable = GetVariable("comp_trig");

      VTrigger rewardTrig = GetTrigger("reward_trig");
      rewardTrig.Variable = GetVariable("reward_trig");

      VTrigger reward = GetTrigger("reward");
      reward.Variable = GetVariable("reward");

      quest.CompletionConditions.Add(compCond);
      quest.CompletionTriggers.Add(compTrig);
      quest.RewardConditions.Add(rewardCond);
      quest.RewardTriggers.Add(rewardTrig);
      quest.Rewards.Add(reward);

      return quest;
    }

    public QuestGraph BuildObjectivePersistenceCheckGraph() {
      QuestGraph quest = GetSimpleQuest(OBJ_PERSISTENCE);

      ObjectiveNode obj = quest.FindNode<ObjectiveNode>();
      VCondition cond = GetCondition("cond");
      cond.Variable = GetVariable("cond");

      obj.Condition = cond;
      obj.AddStartTrigger(GetTrigger("start"));
      obj.AddCompletionTrigger(GetTrigger("comp"));
      obj.AddReward(GetTrigger("reward"));

      return quest;
    }
    //-------------------------------------------------------------------------
    // Asset Creation / Tagging / Access
    //-------------------------------------------------------------------------
    private Variable GetVariable(string path) {
      if (Variables.Contains(path)) {
        return Resources.Load<Variable>(MakeFullName("variable_"+path));
      } else {
        string fullPath = MakePath("variable_" + path);
        Variables.Add(path);
        Variable v = CreateVariable(fullPath);
        v.Folder = StaticFolders.QUEST_DATA;
        v.Key = path;
        v.name = path;
        return v;
      }
    }

    private VTrigger GetTrigger(string path) {
      if (Triggers.ContainsKey(path)) {
        return Triggers[path];
      } else {
        string fullPath = MakePath("setter_" + path);
        VTrigger setter = CreateSetter(fullPath);

        setter.Variable = GetVariable(path);
        setter.Variable.Folder = "triggers";
        setter.Variable.Key = path;
        setter.Variable.Value = false;

        setter.BoolValue = true;
        // setter.name = path;
        Triggers.Add(path, setter);

        return setter;
      }
    }

    private VCondition GetCondition(string path) {
      if (Conditions.ContainsKey(path)) {
        return Conditions[path];
      } else {
        string fullPath = MakePath("condition_" + path);
        VCondition cond = new VCondition();
        cond.Variable = GetVariable(path);
        Conditions.Add(path, cond);
        return cond;
      }
    }

    private QuestGraph GetTrivialQuest(string path) {
      if (TrivialQuests.Contains(path)) {
        return (QuestGraph) Resources.Load(MakeFullName("trivial_quest_" + path));
      } else {
        string fullPath = MakePath("trivial_quest_" + path);
        TrivialQuests.Add(path);

        QuestGraph quest = CreateQuest(fullPath);
        QuestStartNode startNode = quest.FindNode<QuestStartNode>();
        QuestEndNode endNode = quest.FindNode<QuestEndNode>();
        startNode.ConnectTo(endNode);

        return quest;
      }
    }

    private QuestGraph GetSimpleQuest(string path) {
      if (SimpleQuests.Contains(path)) {
        return (QuestGraph) Resources.Load(MakeFullName("simple_quest" + path));
      } else {
        string fullPath = MakePath("simple_quest" + path);

        SimpleQuests.Add(path);

        QuestGraph quest = CreateQuest(fullPath);
        QuestStartNode startNode = quest.FindNode<QuestStartNode>();
        ObjectiveNode objective = quest.AddNode<ObjectiveNode>();
        objective.name = "obj_" + path;
        objective.Condition = GetCondition(path);
        QuestEndNode endNode = quest.FindNode<QuestEndNode>();
        startNode.ConnectTo(objective);
        objective.ConnectTo(endNode);

        return quest;
      }
    }

    private QuestGraph GetQuest(string path) {
      if (Quests.Contains(path)) {
        return (QuestGraph) Resources.Load(MakeFullName(path + "_quest"));
      } else {
        string fullPath = MakePath(path + "_quest");
        Quests.Add(path);
        return CreateQuest(fullPath);
      }
    }

    private Variable CreateVariable(string path) {
      Variable v = ScriptableObject.CreateInstance<Variable>();
      AssetDatabase.CreateAsset(v, path);
      AssetDatabase.SetLabels(v, new string[] { "journey_test" });
      AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
      return v;
    }

    private VTrigger CreateSetter(string path) {
      // VTrigger v = ScriptableObject.CreateInstance<VTrigger>();
      VTrigger v = new VTrigger();
      // AssetDatabase.CreateAsset(v, path);
      // AssetDatabase.SetLabels(v, new string[] { "journey_test" });
      // AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
      return v;
    }

    private QuestGraph CreateQuest(string path) {
      QuestGraph quest = ScriptableObject.CreateInstance<QuestGraph>();

      quest.Rewards = new List<VTrigger>();
      quest.AvailabilityConditions = new List<VCondition>();
      quest.StartConditions = new List<VCondition>();
      quest.CompletionConditions = new List<VCondition>();
      quest.RewardConditions = new List<VCondition>();
      quest.AvailabilityTriggers = new List<VTrigger>();
      quest.StartTriggers = new List<VTrigger>();
      quest.CompletionTriggers = new List<VTrigger>();
      quest.RewardTriggers = new List<VTrigger>();

      AssetDatabase.CreateAsset(quest, path);
      AssetDatabase.SetLabels(quest, new string[] { "journey_test" });
      AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
      return quest;
    }

    private void SetupNarrator() {
      string name = "narrator";

      Narrator narrator = ScriptableObject.CreateInstance<Narrator>();

      AssetDatabase.CreateAsset(narrator, MakePath(name));
      AssetDatabase.SetLabels(narrator, new string[] { "journey_test" });
      AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

      Journey.LoadNarrator(MakeFullName(name));
    }

    public string MakePath(string name) => Path.Combine(TEST_FOLDER, MakeFullName(name)) + EXTENSION;
    public string MakeFullName(string name) => "journey_test_" + name;

    private void Reset() {
      Journey.SaveProgress();
      Journey.Reset();

      VSave.Reset(true);
    }

    private void Resume() {
      VSave.LoadSlots();
      VSave.ChooseSlot("test");

      SetupNarrator();
      Journey.Resume();
    }
  }
}