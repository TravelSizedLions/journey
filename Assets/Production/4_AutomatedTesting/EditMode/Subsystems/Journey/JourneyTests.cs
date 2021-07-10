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
    public List<string> Conditions;
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

    // --- Setup / Tear Down ---------------------------------------------------------------------------------------------------

    [OneTimeSetUp]
    public void OneTimeSetup() {
      Conditions = new List<string>();
      TrivialQuests = new List<string>();
      SimpleQuests = new List<string>();
      Quests = new List<string>();
    }

    [TearDown]
    public void TearDown() {
      DeleteAssets();
      Conditions.Clear();
      TrivialQuests.Clear();
      SimpleQuests.Clear();
      Quests.Clear();
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
        ((BoolCondition) n.Condition).Met = true;
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

      BoolCondition conditionA = GetCondition<BoolCondition>("a");
      BoolCondition conditionB = GetCondition<BoolCondition>("b");

      conditionA.Met = true;

      Journey.Step();
      Assert.IsTrue(Journey.CurrentNode?.GetType() == typeof(QuestStartNode));
      Assert.IsTrue(Journey.CurrentNode?.Progress == QuestProgress.Unavailable);
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Unavailable);

      conditionB.Met = true;
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

      BoolCondition conditionA = GetCondition<BoolCondition>("a");
      BoolCondition conditionB = GetCondition<BoolCondition>("b");

      conditionA.Met = true;

      Journey.Step();
      Assert.IsTrue(Journey.CurrentNode?.GetType() == typeof(QuestStartNode));
      Assert.IsTrue(Journey.CurrentNode?.Progress == QuestProgress.Available);
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Available);

      conditionB.Met = true;

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

      BoolCondition conditionA = GetCondition<BoolCondition>("a");
      BoolCondition conditionB = GetCondition<BoolCondition>("b");

      conditionA.Met = true;

      Journey.Step();
      Assert.IsTrue(Journey.CurrentNode?.GetType() == typeof(QuestStartNode));
      Assert.IsTrue(Journey.CurrentNode?.Progress == QuestProgress.Available);
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Available);

      conditionB.Met = true;

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

      BoolCondition conditionA = GetCondition<BoolCondition>("a");
      BoolCondition conditionB = GetCondition<BoolCondition>("b");

      conditionA.Met = true;

      Journey.Step();
      Assert.IsTrue(Journey.CurrentNode?.GetType() == typeof(QuestEndNode));
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Started);

      conditionB.Met = true;

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

      BoolCondition conditionA = GetCondition<BoolCondition>("a");
      BoolCondition conditionB = GetCondition<BoolCondition>("b");

      conditionA.Met = true;

      Journey.Step();
      Assert.IsTrue(Journey.CurrentNode?.GetType() == typeof(QuestEndNode));
      Assert.IsTrue(Journey.CurrentNode?.Progress == QuestProgress.Completed);
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Completed);

      conditionB.Met = true;

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

      BoolCondition conditionA = GetCondition<BoolCondition>("a");
      BoolCondition conditionB = GetCondition<BoolCondition>("b");

      conditionA.Met = true;

      Journey.Step();
      Assert.IsTrue(Journey.CurrentNode?.GetType() == typeof(QuestEndNode));
      Assert.IsTrue(Journey.CurrentNode?.Progress == QuestProgress.Completed);
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Completed);

      conditionB.Met = true;

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

      GetCondition<BoolCondition>("a").Met = true;
      Journey.Step();

      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Started);
      Assert.IsTrue(Journey.CurrentNodes.Count == 2);

      GetCondition<BoolCondition>("c").Met = true;
      Journey.Step();

      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Started);
      Assert.IsTrue(Journey.CurrentNodes.Count == 2);
      Assert.IsTrue(Journey.CurrentNodes.Contains(Journey.Quest.FindNode<QuestEndNode>()));

      GetCondition<BoolCondition>("b").Met = true;
      Journey.Step();

      Assert.IsTrue(Journey.Finished);
    }

    [Test]
    public void Can_Traverse_Parallel_Objective_Quest_Single_Step() {
      SetupNarrator();
      BuildSimpleParallelQuest();

      Journey.SetQuest(GetQuest(QUEST_PARALLEL_OBJECTIVES));
      Journey.Begin();

      GetCondition<BoolCondition>("a").Met = true;
      GetCondition<BoolCondition>("b").Met = true;
      GetCondition<BoolCondition>("c").Met = true;

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

      foreach (string cond in Conditions) {
        BoolCondition condition = GetCondition<BoolCondition>(cond);
        condition.Met = true;
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

      foreach (string cond in Conditions) {
        BoolCondition condition = GetCondition<BoolCondition>(cond);
        condition.Met = true;
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

      foreach (string cond in Conditions) {
        BoolCondition condition = GetCondition<BoolCondition>(cond);
        condition.Met = true;
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

      foreach (string cond in Conditions) {
        BoolCondition condition = GetCondition<BoolCondition>(cond);
        condition.Met = true;
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

      foreach (string cond in Conditions) {
        BoolCondition condition = GetCondition<BoolCondition>(cond);
        condition.Met = true;
      }

      Journey.Step();
      Assert.IsTrue(Journey.Finished);
      Assert.AreEqual(11, Journey.StepCount);
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

      objectiveA.Condition = ScriptableObject.CreateInstance<BoolCondition>();
      objectiveB.Condition = ScriptableObject.CreateInstance<BoolCondition>();

      return quest;
    }

    private QuestGraph BuildAvailabilityQuest() {
      QuestGraph quest = GetQuest(TRIVIAL_QUEST_WITH_AVAILABILITY_CONDITIONS);
      QuestStartNode startNode = quest.FindNode<QuestStartNode>();

      startNode.AddAvailabilityCondition(GetCondition<BoolCondition>("a"));
      startNode.AddAvailabilityCondition(GetCondition<BoolCondition>("b"));

      QuestEndNode endNode = quest.FindNode<QuestEndNode>();
      startNode.ConnectTo(endNode);

      return quest;
    }

    private QuestGraph BuildStartConditionsQuest() {
      QuestGraph quest = GetQuest(TRIVIAL_QUEST_WITH_START_CONDITIONS);
      QuestStartNode startNode = quest.FindNode<QuestStartNode>();

      startNode.AddStartCondition(GetCondition<BoolCondition>("a"));
      startNode.AddStartCondition(GetCondition<BoolCondition>("b"));

      QuestEndNode endNode = quest.FindNode<QuestEndNode>();
      startNode.ConnectTo(endNode);

      return quest;
    }

    private QuestGraph BuildAvailStartConditionsQuest() {
      QuestGraph quest = GetQuest(TRIVIAL_QUEST_WITH_AVAIL_START_CONDITIONS);
      QuestStartNode startNode = quest.FindNode<QuestStartNode>();

      startNode.AddAvailabilityCondition(GetCondition<BoolCondition>("a"));
      startNode.AddStartCondition(GetCondition<BoolCondition>("b"));

      QuestEndNode endNode = quest.FindNode<QuestEndNode>();
      startNode.ConnectTo(endNode);

      return quest;
    }

    private QuestGraph BuildCompConditionsQuest() {
      QuestGraph quest = GetQuest(TRIVIAL_QUEST_WITH_COMP_CONDITIONS);
      QuestEndNode endNode = quest.FindNode<QuestEndNode>();

      endNode.AddCompletionCondition(GetCondition<BoolCondition>("a"));
      endNode.AddCompletionCondition(GetCondition<BoolCondition>("b"));

      QuestStartNode startNode = quest.FindNode<QuestStartNode>();
      startNode.ConnectTo(endNode);

      return quest;
    }

    private QuestGraph BuildRewardConditionsQuest() {
      QuestGraph quest = GetQuest(TRIVIAL_QUEST_WITH_REWARD_CONDITIONS);
      QuestEndNode endNode = quest.FindNode<QuestEndNode>();

      endNode.AddRewardCondition(GetCondition<BoolCondition>("a"));
      endNode.AddRewardCondition(GetCondition<BoolCondition>("b"));

      QuestStartNode startNode = quest.FindNode<QuestStartNode>();
      startNode.ConnectTo(endNode);

      return quest;
    }

    private QuestGraph BuildCompRewardsQuest() {
      QuestGraph quest = GetQuest(TRIVIAL_QUEST_WITH_COMP_REWARD_CONDITIONS);
      QuestEndNode endNode = quest.FindNode<QuestEndNode>();

      endNode.AddCompletionCondition(GetCondition<BoolCondition>("a"));
      endNode.AddRewardCondition(GetCondition<BoolCondition>("b"));

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

      objectiveA.Condition = GetCondition<BoolCondition>("a");
      objectiveB.Condition = GetCondition<BoolCondition>("b");
      objectiveC.Condition = GetCondition<BoolCondition>("c");

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

      objectiveA.Condition = GetCondition<BoolCondition>("a");
      objectiveB.Condition = GetCondition<BoolCondition>("b");
      objectiveC.Condition = GetCondition<BoolCondition>("c");

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
      outerObjective.Condition = GetCondition<BoolCondition>("outer");

      var outerStart = outer.FindNode<QuestStartNode>();
      var outerEnd = outer.FindNode<QuestEndNode>();

      outerStart.ConnectTo(qNodeA);
      outerStart.ConnectTo(qNodeB);
      qNodeA.ConnectTo(outerObjective);
      outerObjective.ConnectTo(outerEnd);
      qNodeB.ConnectTo(outerEnd);

      return outer;
    }

    //-------------------------------------------------------------------------
    // Asset Creation / Tagging / Access
    //-------------------------------------------------------------------------
    private T GetCondition<T>(string path) where T : ScriptableCondition {
      if (Conditions.Contains(path)) {
        return (T) Resources.Load(MakeFullName("condition_" + path));
      } else {
        string fullPath = MakePath("condition_" + path);
        Conditions.Add(path);
        return CreateCondition<T>(fullPath);
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
        objective.Condition = GetCondition<BoolCondition>(path);
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

    private T CreateCondition<T>(string path) where T : ScriptableCondition {
      T condition = ScriptableObject.CreateInstance<T>();
      AssetDatabase.CreateAsset(condition, path);
      AssetDatabase.SetLabels(condition, new string[] { "journey_test" });
      AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
      return condition;
    }

    private QuestGraph CreateQuest(string path) {
      QuestGraph quest = ScriptableObject.CreateInstance<QuestGraph>();
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
  }
}