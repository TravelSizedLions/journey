using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace HumanBuilders.Tests {

  public class CanvasOrderingTest {

    [OneTimeSetUp]
    public void LoadScene() {
      Application.targetFrameRate = 120; 
      Time.timeScale = 2f; 
      SceneManager.LoadScene("crash_level_upper_deck_v2");
    }

    [UnityTest]
    public IEnumerator Canvas_Ordering_Is_Correct() {
      PauseScreen.ContinueGame();
      yield return null;
      

      Canvas sceneNameCanv = GameObject.Find("scene_name").transform.GetComponentInChildren<Canvas>();
      Canvas controlInputCanv = TestUtils.Find<Canvas>("control_inputs");
      Canvas transitionCanv = TestUtils.Find<TransitionManager>("transition_manager").GetComponentInChildren<Canvas>();
      Canvas walletsCanv = GameObject.Find("wallets").GetComponent<Canvas>();

      Assert.True(PauseScreen.Canvas.sortingOrder > DialogManager.Canvas.sortingOrder);
      Assert.True(DialogManager.Canvas.sortingOrder > walletsCanv.sortingOrder);
      Assert.True(walletsCanv.sortingOrder > sceneNameCanv.sortingOrder);
      Assert.True(sceneNameCanv.sortingOrder > controlInputCanv.sortingOrder);
      Assert.True(controlInputCanv.sortingOrder > transitionCanv.sortingOrder);
      yield return null;
    }

    // JPH NOTE: I don't know why I thought the order should be different
    // between paused and not paused, but I don't think it effects the
    // selectability of dialog choices, so it should be fine to keep them the
    // same now...I think?
    // [UnityTest]
    // public IEnumerator CanvasOrdering_CorrectWhenNotPaused() {
    //   PauseScreen.ContinueGame();
    //   yield return null;
      
    //   Assert.True(DialogManager.Canvas.sortingOrder > PauseScreen.Canvas.sortingOrder);

    //   Canvas sceneNameCanv = GameObject.Find("scene_name").transform.GetChild(0).GetComponent<Canvas>();
    //   Canvas controlInputCanv = TestUtils.Find<Canvas>("control_inputs");

    //   Assert.True(DialogManager.Canvas.sortingOrder > sceneNameCanv.sortingOrder);
    //   Assert.True(sceneNameCanv.sortingOrder > controlInputCanv.sortingOrder);

    //   yield return null;
    // }
  }
}