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
      var graphs = EditorUtils.FindAssetsByType<QuestGraph>().ConvertAll(graph => (IAutoGraph)graph);
      var report = new MultiGraphCompletenessReport(graphs);
      
      if (report.AllComplete) {
        Debug.Log("All quest graphs complete!");
      } else {
        Debug.Log(report.Message);
      }

      Assert.IsTrue(report.AllComplete);
    }
  }
}