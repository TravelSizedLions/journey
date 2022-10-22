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
      if (!AnalyzeCompletenessInQuests.AllQuestsComplete(out string message)) {
        Debug.Log(message);
        Assert.IsTrue(false);
      }

      Assert.IsTrue(true);
    }
  }
}