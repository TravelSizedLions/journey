
using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using XNodeEditor;

namespace HumanBuilders.Tests {
  public class SojournTests {
    public string QuestRunnerPath { get => Path.Combine(TEST_FOLDER, QUEST_RUNNER_NAME)+EXTENSION; }
    public string BlankQuestPath { get => Path.Combine(TEST_FOLDER, BLANK_QUEST_NAME)+EXTENSION; }
    public string TrivialQuestPath { get => Path.Combine(TEST_FOLDER, TRIVIAL_QUEST_NAME)+EXTENSION; }
    

    private const string EXTENSION = ".asset";
    private const string TEST_FOLDER = "Assets/Production/4_AutomatedTesting/EditMode/Subsystems/Sojourn/Resources";
    private const string QUEST_RUNNER_NAME = "sojourn_test_quest_runner";
    private const string BLANK_QUEST_NAME = "sojourn_test_blank_quest";
    private const string TRIVIAL_QUEST_NAME = "sojourn_test_trivial_quest";

    [TearDown]
    public void TearDown() {
      TearDown(BlankQuestPath);
      TearDown(QuestRunnerPath);
      TearDown(TrivialQuestPath);
    }

    public void TearDown(string path) {
      if (File.Exists(path)) {
        AssetDatabase.DeleteAsset(path);
      }
    }

    [Test]
    public void Editor_Inner_Quest_Gets_Parent() {
      QuestAsset outer = ScriptableObject.CreateInstance<QuestAsset>();

      QuestNode qNode = outer.AddNode<QuestNode>();

      QuestAsset inner = ScriptableObject.CreateInstance<QuestAsset>();
      qNode.Quest = inner;
      qNode.OnQuestChange(); // Normally this would get called by OdinInspector, but that doesn't fire in tests.

      Assert.AreEqual(inner.GetParent(), outer);
    }

    [UnityTest]
    public IEnumerator Editor_Creates_Default_Nodes() {
      QuestAsset quest = SetupBlankQuest();

      yield return null;

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
      QuestAsset outer = ScriptableObject.CreateInstance<QuestAsset>();
      QuestNode qNode = outer.AddNode<QuestNode>();
      QuestAsset inner = ScriptableObject.CreateInstance<QuestAsset>();
      qNode.Quest = inner;
      qNode.OnQuestChange();

      yield return null;

      qNode.Open();

      yield return null;

      Assert.IsTrue(NodeEditorWindow.current.graphEditor.target == inner);
    }

    [UnityTest]
    public IEnumerator Editor_Exits_Subquest_When_Parent_Is_Present() {
      QuestAsset outer = ScriptableObject.CreateInstance<QuestAsset>();
      QuestNode qNode = outer.AddNode<QuestNode>();
      QuestAsset inner = ScriptableObject.CreateInstance<QuestAsset>();
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
      QuestAsset outer = ScriptableObject.CreateInstance<QuestAsset>();

      yield return null;

      NodeEditorWindow.Open(outer);

      yield return null;

      outer.Exit();

      yield return null;

      Assert.IsTrue(NodeEditorWindow.current.graphEditor.target == outer);
    }


    [UnityTest]
    public IEnumerator Can_Start_Quest_Runner() {
      CreateQuestRunner(QuestRunnerPath);
      QuestManager.InitQuestRunner(QUEST_RUNNER_NAME);
      yield return null;

      Assert.IsTrue(QuestManager.Initialized);
    }

    [UnityTest]
    public IEnumerator Can_Start_Quest() {
      CreateQuestRunner(QuestRunnerPath);
      QuestManager.InitQuestRunner(QUEST_RUNNER_NAME);
      yield return null;

      QuestAsset quest = CreateQuest(TrivialQuestPath);
      yield return null;

      BuildTrivialQuest(quest);
      yield return null;

      QuestManager.InitQuest(TRIVIAL_QUEST_NAME);
      yield return null;
      
      Assert.IsTrue(QuestManager.HasQuest);
    }

    //-------------------------------------------------------------------------
    // Quest Builders
    //-------------------------------------------------------------------------
    private QuestAsset SetupBlankQuest() {
      return CreateQuest(BlankQuestPath);
    }

    private QuestAsset BuildTrivialQuest(QuestAsset quest) {
      QuestStartNode startNode = quest.FindNode<QuestStartNode>();
      QuestEndNode endNode = quest.FindNode<QuestEndNode>();

      startNode.Connect(endNode, "Output", "Input");

      return quest;
    }

    private QuestAsset CreateQuest(string path) {
      QuestAsset quest = ScriptableObject.CreateInstance<QuestAsset>();
      AssetDatabase.CreateAsset(quest, path);
      AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
      return quest;
    }

    private QuestRunner CreateQuestRunner(string path) {
      QuestRunner runner = ScriptableObject.CreateInstance<QuestRunner>();
      AssetDatabase.CreateAsset(runner, path);
      AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
      return runner;
    }
  }
}