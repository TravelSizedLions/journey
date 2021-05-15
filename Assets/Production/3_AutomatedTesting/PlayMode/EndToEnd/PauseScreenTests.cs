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
    public IEnumerator PauseScreen_DisabledOnStart() {
      yield return null;
      Assert.False(PauseScreen.Instance.gameObject.activeSelf);
    }
  }
}