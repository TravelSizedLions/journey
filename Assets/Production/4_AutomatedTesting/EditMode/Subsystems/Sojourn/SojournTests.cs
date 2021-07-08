
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using XNodeEditor;

namespace HumanBuilders.Tests {
  public class SojournTests {
    public string NarratorPath { get => Path.Combine(TEST_FOLDER, NARRATOR_NAME)+EXTENSION; }
    public string BlankQuestPath { get => Path.Combine(TEST_FOLDER, BLANK_QUEST_NAME)+EXTENSION; }
    public string TrivialQuestPath { get => Path.Combine(TEST_FOLDER, TRIVIAL_QUEST_NAME)+EXTENSION; }
    public string SimpleObjectiveQuestPath { get => Path.Combine(TEST_FOLDER, SIMPLE_OBJECTIVE_QUEST_NAME)+EXTENSION; }
    

    private const string EXTENSION = ".asset";
    private const string TEST_FOLDER = "Assets/Production/4_AutomatedTesting/EditMode/Subsystems/Sojourn/Resources";
    private const string NARRATOR_NAME = "sojourn_test_narrator";
    private const string BLANK_QUEST_NAME = "sojourn_test_blank_quest";
    private const string TRIVIAL_QUEST_NAME = "sojourn_test_trivial_quest";
    private const string SIMPLE_OBJECTIVE_QUEST_NAME = "sojourn_simple_objective_quest";

    [TearDown]
    public void TearDown() {
      TearDown(BlankQuestPath);
      TearDown(NarratorPath);
      TearDown(TrivialQuestPath);
      TearDown(SimpleObjectiveQuestPath);
    }

    public void TearDown(string path) {
      if (File.Exists(path)) {
        AssetDatabase.DeleteAsset(path);
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
      CreateNarrator(NarratorPath);
      Sojourn.LoadNarrator(NARRATOR_NAME);

      Assert.IsTrue(Sojourn.Initialized);
    }

    [Test]
    public void Can_Initialize_Quest() {
      CreateNarrator(NarratorPath);
      Sojourn.LoadNarrator(NARRATOR_NAME);

      BuildTrivialQuest();

      Sojourn.LoadQuest(TRIVIAL_QUEST_NAME);
      
      Assert.IsTrue(Sojourn.HasQuest);
    }

    [Test]
    public void Can_Traverse_Trivial_Quest() {
      CreateNarrator(NarratorPath);
      Sojourn.LoadNarrator(NARRATOR_NAME);

      BuildTrivialQuest();
      Sojourn.LoadQuest(TRIVIAL_QUEST_NAME);
      
      Sojourn.Commence();

      Assert.IsTrue(Sojourn.Finished);
    }

    [Test]
    public void Can_Traverse_Quest_With_Objectives() {
      CreateNarrator(NarratorPath);
      Sojourn.LoadNarrator(NARRATOR_NAME);

      BuildSimpleObjectiveQuest();
      Sojourn.LoadQuest(SIMPLE_OBJECTIVE_QUEST_NAME);

      Sojourn.Commence();

      Assert.IsFalse(Sojourn.Finished);

      foreach (ObjectiveNode n in Sojourn.Quest.FindNodes<ObjectiveNode>()) {
        ((DummyCondition)n.Condition).Met = true;
      }

      Sojourn.CheckProgress();
      Assert.IsTrue(Sojourn.Finished);
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

      objectiveA.Condition = ScriptableObject.CreateInstance<DummyCondition>();
      objectiveB.Condition = ScriptableObject.CreateInstance<DummyCondition>();

      return quest;
    }

    private QuestGraph CreateQuest(string path) {
      QuestGraph quest = ScriptableObject.CreateInstance<QuestGraph>();
      AssetDatabase.CreateAsset(quest, path);
      AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
      return quest;
    }

    private Narrator CreateNarrator(string path) {
      Narrator runner = ScriptableObject.CreateInstance<Narrator>();
      AssetDatabase.CreateAsset(runner, path);
      AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
      return runner;
    }
  }
}