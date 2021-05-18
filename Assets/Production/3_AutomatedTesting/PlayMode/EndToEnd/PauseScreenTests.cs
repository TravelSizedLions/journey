using System.Collections;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace HumanBuilders.Tests {
  public class PauseScreenTests {

    /// <summary>
    /// Before testing starts.
    /// </summary>
    [OneTimeSetUp]
    public void LoadScene() {
      Application.targetFrameRate = 120; 
      Time.timeScale = 2f; 
      
    }

    /// <summary>
    /// Before each test.
    /// </summary>
    [UnitySetUp]
    public IEnumerator SetupTest() {
      AsyncOperation op = SceneManager.LoadSceneAsync("crash_opening_cutscene");

      while (!op.isDone) {
        yield return null;
      }
    }


    //-------------------------------------------------------------------------
    // Tests
    //-------------------------------------------------------------------------

    [UnityTest]
    public IEnumerator PauseScreen_CanvasDisabledOnStart() {
      yield return null;
      Assert.False(PauseScreen.Canvas.gameObject.activeSelf);
    }

    [UnityTest]
    public IEnumerator PauseScreen_RootObjectEnabledOnStart() {
      yield return null;
      Assert.True(PauseScreen.Instance.gameObject.activeSelf);
    }

    [UnityTest]
    public IEnumerator PauseScreen_Pause_On_Main_Options() {
      PauseScreen.PauseGame();

      yield return null;

      Assert.True(PauseScreen.MainPauseMenu.activeSelf);
      Assert.True(PauseScreen.MainPauseMenu.activeInHierarchy);
    }

    [UnityTest]
    public IEnumerator PauseScreen_Pause_Scenes_Disabled() {
      PauseScreen.PauseGame();
      yield return null;
      Assert.False(PauseScreen.ScenesMenu.activeSelf);
    }

    [UnityTest]
    public IEnumerator PauseScreen_Can_Select_Scenes_Menu() {
      PauseScreen.PauseGame();
      yield return null;

      MenuButton[] buttons = PauseScreen.Instance.GetComponentsInChildren<MenuButton>();
      foreach (MenuButton button in buttons) {
        if (button.name.Contains("scenes")) {
          button.onClick.Invoke();
          break;
        }
      }

      yield return null;

      Assert.False(PauseScreen.MainPauseMenu.activeSelf);
      Assert.True(PauseScreen.ScenesMenu.activeSelf);
    }

    [UnityTest]
    public IEnumerator PauseScreen_Can_Return_From_Scenes_Menu() {
      PauseScreen.PauseGame();
      yield return null;

      PauseScreen.OpenScenesMenu();
      yield return null;

      // The test is only valid if we know for a fact we made it to the scenes menu.
      Assert.True(PauseScreen.ScenesMenu.activeSelf);

      MenuButton[] buttons = PauseScreen.ScenesMenu.GetComponentsInChildren<MenuButton>();
      MenuButton found = null;
      foreach (MenuButton button in buttons) {
        if (button.name.Contains("back")) {
          found = button;
          break;
        }
      }

      if (found != null) {
        found.onClick.Invoke();
      } else {
        Assert.Fail();
      }

      yield return null;

      Assert.True(PauseScreen.MainPauseMenu.activeSelf);
      Assert.False(PauseScreen.ScenesMenu.activeSelf);
    }
  }
}
