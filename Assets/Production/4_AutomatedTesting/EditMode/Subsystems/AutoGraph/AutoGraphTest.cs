
using System.Collections.Generic;
using HumanBuilders.Editor;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HumanBuilders.ExpensiveTests {
  public class AutoGraphTest {

    [Test]
    public void Verify_All_Scene_AutoGraphs() {
      bool pass = true;
      List<string> scenes = GetAllScenesInBuild();
      foreach (string scenePath in scenes) {
        string fileName = scenePath.Split('/')[scenePath.Split('/').Length-1].Split('.')[0];
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        bool isGood = AnalyzeCompletenessInScene.AnalyzeGraphs(out string message);
        
        if (!isGood) {
          message = "----- " + fileName + " -----\n" + message;
          Debug.Log(message);
        }

        pass &= isGood;
      }

      Assert.IsTrue(pass);
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