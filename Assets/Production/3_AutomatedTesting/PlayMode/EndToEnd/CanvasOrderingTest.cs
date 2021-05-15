using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace HumanBuilders.Tests {

  public class CanvasOrderingTest {

    [OneTimeSetUp]
    public void LoadScene() {
      SceneManager.LoadScene("crash_level_upper_deck_v2");
    }

    [UnityTest]
    public IEnumerator CanvasOrdering_CorrectWhenPaused() {
      PauseScreen.PauseGame();
      yield return null;
      
      Assert.True(PauseScreen.Canvas.sortingOrder > DialogManager.Canvas.sortingOrder);

      Canvas sceneNameCanv = GameObject.Find("scene_name").transform.GetChild(0).GetComponent<Canvas>();
      Canvas controlInputCanv = TestUtils.Find<Canvas>("control_inputs");

      Assert.True(DialogManager.Canvas.sortingOrder > sceneNameCanv.sortingOrder);
      Assert.True(sceneNameCanv.sortingOrder > controlInputCanv.sortingOrder);

      yield return null;
    }

    [UnityTest]
    public IEnumerator CanvasOrdering_CorrectWhenNotPaused() {
      PauseScreen.ContinueGame();
      yield return null;
      
      Assert.True(DialogManager.Canvas.sortingOrder > PauseScreen.Canvas.sortingOrder);

      Canvas sceneNameCanv = GameObject.Find("scene_name").transform.GetChild(0).GetComponent<Canvas>();
      Canvas controlInputCanv = TestUtils.Find<Canvas>("control_inputs");

      Assert.True(DialogManager.Canvas.sortingOrder > sceneNameCanv.sortingOrder);
      Assert.True(sceneNameCanv.sortingOrder > controlInputCanv.sortingOrder);

      yield return null;
    }
  }
}