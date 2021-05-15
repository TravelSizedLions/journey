using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace HumanBuilders.Tests {
  public class PauseScreenTests {

    [OneTimeSetUp]
    public void LoadScene() {
      SceneManager.LoadScene("crash_opening_cutscene");
    }

    [UnityTest]
    public IEnumerator PauseScreen_CanvasDisabledOnStart() {
      yield return null;
      Assert.False(PauseScreen.Canvas.gameObject.activeSelf);
    }

    [UnityTest]
    public IEnumerator PauseScreen_RootObjectEnabledOnStart() {
      yield return null;
      Assert.False(PauseScreen.Canvas.gameObject.activeSelf);
    }
  }
}
