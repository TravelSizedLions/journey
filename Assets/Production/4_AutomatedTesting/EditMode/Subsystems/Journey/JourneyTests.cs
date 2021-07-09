
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

    // --- Paths ---------------------------------------------------------------------------------------------------------------

    public string NarratorPath { get => MakePath(NARRATOR); }
    public string BlankQuestPath { get => MakePath(BLANK_QUEST); }
    public string TrivialQuestPath { get => MakePath(TRIVIAL_QUEST); }
    public string SimpleObjectiveQuestPath { get => MakePath(SIMPLE_OBJECTIVE_QUEST); }
    public string TrivialQuestWithAvailabilityConditionsPath { get => MakePath(TRIVIAL_QUEST_WITH_AVAILABILITY_CONDITIONS); }
    public string TrivialQuestWithStartConditionsPath { get => MakePath(TRIVIAL_QUEST_WITH_START_CONDITIONS); }
    public string TrivialQuestWithAvailStartConditionsPath { get => MakePath(TRIVIAL_QUEST_WITH_AVAIL_START_CONDITIONS); }
    public string TrivialQuestWithCompConditionsPath { get => MakePath(TRIVIAL_QUEST_WITH_COMP_CONDITIONS); }
    public string TrivialQuestWithRewardConditionsPath { get => MakePath(TRIVIAL_QUEST_WITH_REWARD_CONDITIONS); }
    public string TrivialQuestWithCompRewardConditionsPath { get => MakePath(TRIVIAL_QUEST_WITH_COMP_REWARD_CONDITIONS); }
    public string QuestParallelObjectivesPath { get => MakePath(QUEST_PARALLEL_OBJECTIVES); }
    public string NestedQuestOuterPath { get => MakePath(NESTED_QUEST_OUTER); }
    public string NestedQuestInnerPath { get => MakePath(NESTED_QUEST_INNER); }
    
    public string MakePath(string name) => Path.Combine(TEST_FOLDER, MakeName(name))+EXTENSION;
    public string MakeName(string name) => "journey_test_"+name;

    // --- Names ---------------------------------------------------------------------------------------------------------------
    private const string EXTENSION = ".asset";
    private const string TEST_FOLDER = "Assets/Production/4_AutomatedTesting/EditMode/Subsystems/Journey/Resources";

    private const string NARRATOR = "narrator";
    private const string BLANK_QUEST = "blank_quest";
    private const string TRIVIAL_QUEST = "trivial_quest";
    private const string SIMPLE_OBJECTIVE_QUEST = "simple_objective_quest";
    private const string TRIVIAL_QUEST_WITH_AVAILABILITY_CONDITIONS = "trivial_quest_with_availability_conditions";
    private const string TRIVIAL_QUEST_WITH_START_CONDITIONS = "trivial_quest_with_start_conditions";
    private const string TRIVIAL_QUEST_WITH_AVAIL_START_CONDITIONS = "trivial_quest_with_avail_start_conditions";
    private const string TRIVIAL_QUEST_WITH_COMP_CONDITIONS = "trivial_quest_with_comp_conditions";
    private const string TRIVIAL_QUEST_WITH_REWARD_CONDITIONS = "trivial_quest_with_reward_conditions";
    private const string TRIVIAL_QUEST_WITH_COMP_REWARD_CONDITIONS = "trivial_quest_with_comp_reward_conditions";
    private const string QUEST_PARALLEL_OBJECTIVES = "quest_parallel_objectives";
    private const string NESTED_QUEST_OUTER = "nested_quest_outer";
    private const string NESTED_QUEST_INNER = "nested_quest_inner";

    [OneTimeSetUp]
    public void OneTimeSetup() {
      Conditions = new List<string>();
    }


    [TearDown]
    public void TearDown() {
      DeleteAssets();
      Conditions.Clear();
    }

    public void DeleteAssets() {
      foreach(string guid in AssetDatabase.FindAssets("l:journey_test")) {
        AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));
      }
    }

    [Test]
    public void Editor_Inner_Quest_Gets_Parent() {
      QuestGraph outer = ScriptableObject.CreateInstance<QuestGraph>();

      QuestNode qNode = outer.AddNode<QuestNode>();

      QuestGraph inner = ScriptableObject.CreateInstance<QuestGraph>();
      qNode.Quest = inner;
      qNode.OnQuestChange(); // Normally this would get called by OdinInspector, but that doesn't fire in tests.

      Assert.AreEqual(inner.GetParent(), outer);
    }

    [Test]
    public void Editor_Creates_Default_Nodes() {
      QuestGraph quest = SetupBlankQuest();
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
      qNode.Quest = inner;
      qNode.OnQuestChange();

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
      qNode.Quest = inner;
      qNode.OnQuestChange();

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

      BuildTrivialQuest();
      Journey.LoadQuest(MakeName(TRIVIAL_QUEST));
      
      Assert.IsTrue(Journey.HasQuest);
    }

    [Test]
    public void Can_Traverse_Trivial_Quest() {
      SetupNarrator();

      BuildTrivialQuest();
      Journey.LoadQuest(MakeName(TRIVIAL_QUEST));
      
      Journey.Begin();

      Assert.IsTrue(Journey.Finished);
    }

    [Test]
    public void Can_Traverse_Quest_With_Objectives() {
      SetupNarrator();

      BuildSimpleObjectiveQuest();
      Journey.LoadQuest(MakeName(SIMPLE_OBJECTIVE_QUEST));

      Journey.Begin();
      Assert.IsFalse(Journey.Finished);

      foreach (ObjectiveNode n in Journey.Quest.FindNodes<ObjectiveNode>()) {
        ((BoolCondition)n.Condition).Met = true;
      }

      Journey.Step();
      Assert.IsTrue(Journey.Finished);
    }

    [Test]
    public void Quest_Marked_As_Started() {
      SetupNarrator();

      BuildSimpleObjectiveQuest();
      Journey.LoadQuest(MakeName(SIMPLE_OBJECTIVE_QUEST));

      Journey.Begin();

      Assert.IsFalse(Journey.Finished);
      Assert.IsTrue(Journey.Quest.Progress == QuestProgress.Started);
    }

    [Test]
    public void Can_Traverse_Quest_With_Availability_Conditions() {
      SetupNarrator();
      BuildAvailabilityQuest();

      Journey.LoadQuest(MakeName(TRIVIAL_QUEST_WITH_AVAILABILITY_CONDITIONS));
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

      Journey.LoadQuest(MakeName(TRIVIAL_QUEST_WITH_START_CONDITIONS));
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

      Journey.LoadQuest(MakeName(TRIVIAL_QUEST_WITH_AVAIL_START_CONDITIONS));
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

      Journey.LoadQuest(MakeName(TRIVIAL_QUEST_WITH_COMP_CONDITIONS));
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

      Journey.LoadQuest(MakeName(TRIVIAL_QUEST_WITH_REWARD_CONDITIONS));
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

      Journey.LoadQuest(MakeName(TRIVIAL_QUEST_WITH_COMP_REWARD_CONDITIONS));
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

      Journey.LoadQuest(MakeName(QUEST_PARALLEL_OBJECTIVES));
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

      Journey.LoadQuest(MakeName(QUEST_PARALLEL_OBJECTIVES));
      Journey.Begin();

      GetCondition<BoolCondition>("a").Met = true;
      GetCondition<BoolCondition>("b").Met = true;
      GetCondition<BoolCondition>("c").Met = true;

      Journey.Step();

      Assert.IsTrue(Journey.Finished);
    }

    public void Can_Traverse_Quest_With_Nested_Quest() {
      SetupNarrator();

      BuildNestedQuest();
      Journey.LoadQuest(MakeName(NESTED_QUEST_OUTER));

      Journey.Begin();
    }

    //-------------------------------------------------------------------------
    // Quest Builders
    //-------------------------------------------------------------------------
    private QuestGraph SetupBlankQuest() {
      return CreateQuest(BlankQuestPath);
    }

    private QuestGraph BuildTrivialQuest() {
      QuestGraph quest = CreateQuest(TrivialQuestPath);
      QuestStartNode startNode = quest.FindNode<QuestStartNode>();
      QuestEndNode endNode = quest.FindNode<QuestEndNode>();

      startNode.ConnectTo(endNode);

      return quest;
    }

    private QuestGraph BuildSimpleObjectiveQuest() {
      QuestGraph quest = CreateQuest(SimpleObjectiveQuestPath);
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
      QuestGraph quest = CreateQuest(TrivialQuestWithAvailabilityConditionsPath);
      QuestStartNode startNode = quest.FindNode<QuestStartNode>();

      startNode.AddAvailabilityCondition(GetCondition<BoolCondition>("a"));
      startNode.AddAvailabilityCondition(GetCondition<BoolCondition>("b"));

      QuestEndNode endNode = quest.FindNode<QuestEndNode>();
      startNode.ConnectTo(endNode);

      return quest;
    }

    private QuestGraph BuildStartConditionsQuest() {
      QuestGraph quest = CreateQuest(TrivialQuestWithStartConditionsPath);
      QuestStartNode startNode = quest.FindNode<QuestStartNode>();

      startNode.AddStartCondition(GetCondition<BoolCondition>("a"));
      startNode.AddStartCondition(GetCondition<BoolCondition>("b"));

      QuestEndNode endNode = quest.FindNode<QuestEndNode>();
      startNode.ConnectTo(endNode);

      return quest;    
    }

    private QuestGraph BuildAvailStartConditionsQuest() {
      QuestGraph quest = CreateQuest(TrivialQuestWithAvailStartConditionsPath);
      QuestStartNode startNode = quest.FindNode<QuestStartNode>();

      startNode.AddAvailabilityCondition(GetCondition<BoolCondition>("a"));
      startNode.AddStartCondition(GetCondition<BoolCondition>("b"));

      QuestEndNode endNode = quest.FindNode<QuestEndNode>();
      startNode.ConnectTo(endNode);

      return quest;      
    }

    private QuestGraph BuildCompConditionsQuest() {
      QuestGraph quest = CreateQuest(TrivialQuestWithCompConditionsPath);
      QuestEndNode endNode = quest.FindNode<QuestEndNode>();

      endNode.AddCompletionCondition(GetCondition<BoolCondition>("a"));
      endNode.AddCompletionCondition(GetCondition<BoolCondition>("b"));

      QuestStartNode startNode = quest.FindNode<QuestStartNode>();
      startNode.ConnectTo(endNode);

      return quest;   
    }

    private QuestGraph BuildRewardConditionsQuest() {
      QuestGraph quest = CreateQuest(TrivialQuestWithRewardConditionsPath);
      QuestEndNode endNode = quest.FindNode<QuestEndNode>();

      endNode.AddRewardCondition(GetCondition<BoolCondition>("a"));
      endNode.AddRewardCondition(GetCondition<BoolCondition>("b"));

      QuestStartNode startNode = quest.FindNode<QuestStartNode>();
      startNode.ConnectTo(endNode);

      return quest;   
    }

    private QuestGraph BuildCompRewardsQuest() {
      QuestGraph quest = CreateQuest(TrivialQuestWithCompRewardConditionsPath);
      QuestEndNode endNode = quest.FindNode<QuestEndNode>();

      endNode.AddCompletionCondition(GetCondition<BoolCondition>("a"));
      endNode.AddRewardCondition(GetCondition<BoolCondition>("b"));

      QuestStartNode startNode = quest.FindNode<QuestStartNode>();
      startNode.ConnectTo(endNode);

      return quest;   
    }

    private QuestGraph BuildSimpleParallelQuest() {
      QuestGraph quest = CreateQuest(QuestParallelObjectivesPath);
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
      QuestGraph innerQuest = CreateQuest(NestedQuestInnerPath);
      QuestStartNode startNode = innerQuest.FindNode<QuestStartNode>();
      QuestEndNode endNode = innerQuest.FindNode<QuestEndNode>();
      startNode.ConnectTo(endNode);

      QuestGraph outerQuest = CreateQuest(NestedQuestOuterPath);
      
      QuestNode questNode = outerQuest.AddNode<QuestNode>();
      questNode.ChangeQuest(innerQuest);

      QuestStartNode outerStartNode = outerQuest.FindNode<QuestStartNode>();
      QuestEndNode outerEndNode = outerQuest.FindNode<QuestEndNode>();

      outerStartNode.ConnectTo(questNode);
      questNode.ConnectTo(endNode);

      return outerQuest;
    }


    //-------------------------------------------------------------------------
    // Asset Creation / Tagging
    //-------------------------------------------------------------------------
    private T GetCondition<T>(string path) where T : ScriptableCondition {
      if (Conditions.Contains(path)) {
        return (T)Resources.Load(MakeName("condition_"+path));
      } else {
        string fullPath = MakePath("condition_"+path);
        Conditions.Add(path);
        return CreateCondition<T>(fullPath);
      }
    }

    private T CreateCondition<T>(string path) where T : ScriptableCondition {
      T condition = ScriptableObject.CreateInstance<T>();
      AssetDatabase.CreateAsset(condition, path);
      AssetDatabase.SetLabels(condition, new string[] {"journey_test"});
      AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
      return condition;
    }

    private QuestGraph CreateQuest(string path) {
      QuestGraph quest = ScriptableObject.CreateInstance<QuestGraph>();
      AssetDatabase.CreateAsset(quest, path);
      AssetDatabase.SetLabels(quest, new string[] {"journey_test"});
      AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
      return quest;
    }

    private void SetupNarrator() {
      CreateNarrator(NarratorPath);
      Journey.LoadNarrator(MakeName(NARRATOR));
    }

    private Narrator CreateNarrator(string path) {
      Narrator runner = ScriptableObject.CreateInstance<Narrator>();
      AssetDatabase.CreateAsset(runner, path);
      AssetDatabase.SetLabels(runner, new string[] {"journey_test"});
      AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
      return runner;
    }
  }
}