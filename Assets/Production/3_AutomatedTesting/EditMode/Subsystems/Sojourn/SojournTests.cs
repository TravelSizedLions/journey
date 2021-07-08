
using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using XNodeEditor;

namespace HumanBuilders.Tests {
  public class SojournTests {

    public string MainQuestPath { get => Path.Combine(TEST_FOLDER, MAIN_QUEST_NAME); }

    private const string TEST_FOLDER = "Assets/Production/3_AutomatedTesting/EditMode/Subsystems/Sojourn";
    private const string MAIN_QUEST_NAME = "main.asset";

    private void SetupTest() {
      
    }

    private void BuildSimpleQuest() {
      
    }

    [TearDown]
    public void TearDown() {
      if (File.Exists(MainQuestPath)) {
        AssetDatabase.DeleteAsset(MainQuestPath);
      }
    }

    [Test]
    public void Inner_Quest_Gets_Parent() {
      QuestAsset outer = ScriptableObject.CreateInstance<QuestAsset>();

      QuestNode qNode = outer.AddNode<QuestNode>();

      QuestAsset inner = ScriptableObject.CreateInstance<QuestAsset>();
      qNode.Quest = inner;
      qNode.OnQuestChange(); // Normally this would get called by OdinInspector, but that doesn't fire in tests.

      Assert.AreEqual(inner.GetParent(), outer);
    }

    [UnityTest]
    public IEnumerator Creates_Default_Nodes() {
      QuestAsset quest = ScriptableObject.CreateInstance<QuestAsset>();
      AssetDatabase.CreateAsset(quest, MainQuestPath);
      AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

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
    public IEnumerator Opens_Subquest() {
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
    public IEnumerator Exits_Subquest_When_Parent_Is_Present() {
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
    public IEnumerator Does_Not_Exit_Quest_Without_Parent() {
      QuestAsset outer = ScriptableObject.CreateInstance<QuestAsset>();

      yield return null;

      NodeEditorWindow.Open(outer);

      yield return null;

      outer.Exit();

      yield return null;

      Assert.IsTrue(NodeEditorWindow.current.graphEditor.target == outer);
    }
  }
}