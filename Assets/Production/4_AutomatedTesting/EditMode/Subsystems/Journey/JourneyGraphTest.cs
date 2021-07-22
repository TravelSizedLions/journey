using System.Collections.Generic;
using HumanBuilders.Editor;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HumanBuilders.ExpensiveTests {
  public class JourneyGraphTest {

    [Test]
    public void Verify_Quest_Graphs() {
      if (!QuestVerify.VerifyQuestsInProject(out string message)) {
        Debug.Log(message);
        Assert.IsTrue(false);
      }

      Assert.IsTrue(true);
    }

    public List<string> GetAllScenesInBuild() {
      List<string> scenes = new List<string>();

      int sceneCount = SceneManager.sceneCountInBuildSettings;
      for (int i = 0; i < sceneCount; i++) {
        string path = SceneUtility.GetScenePathByBuildIndex(i);
        if (!string.IsNullOrEmpty(path)) {
          scenes.Add(path);
        }
      }

      return scenes;
    }
  }
}