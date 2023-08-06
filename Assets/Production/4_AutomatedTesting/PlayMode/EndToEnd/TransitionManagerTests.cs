using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace HumanBuilders.Tests {
  public class TransitionManagerTests {

    private const string PRE_SCENE_NAME = "transition-manager-pre";
    private const string POST_SCENE_NAME = "transition-manager-post";

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
      AsyncOperation op = SceneManager.LoadSceneAsync(PRE_SCENE_NAME);

      while (!op.isDone) {
        yield return null;
      }
    }


    //-------------------------------------------------------------------------
    // Tests
    //-------------------------------------------------------------------------
    [UnityTest]
    public IEnumerator TransitionManager_Enables_Movement() {
      GameManager.Player.Interact();
      Assert.True(DialogManager.IsDialogBoxOpen());
      Assert.False(DialogManager.IsDialogFinished());
      Assert.False(GameManager.Player.CanMove());

      TransitionManager.MakeTransition(POST_SCENE_NAME);
      while(TransitionManager.Transitioning) {
        yield return null;
      }

      Assert.True(SceneManager.GetActiveScene().name == POST_SCENE_NAME);
      Assert.True(GameManager.Player.CanMove());
    }

    [UnityTest]
    public IEnumerator TransitionManager_Enables_Jumping() {
      GameManager.Player.Interact();
      Assert.True(DialogManager.IsDialogBoxOpen());
      Assert.False(DialogManager.IsDialogFinished());
      Assert.False(GameManager.Player.CanJump());

      TransitionManager.MakeTransition(POST_SCENE_NAME);
      while(TransitionManager.Transitioning) {
        yield return null;
      }

      Assert.True(SceneManager.GetActiveScene().name == POST_SCENE_NAME);
      Assert.True(GameManager.Player.CanJump());
    }

    [UnityTest]
    public IEnumerator TransitionManager_Enables_Crouching() {
      GameManager.Player.Interact();
      Assert.True(DialogManager.IsDialogBoxOpen());
      Assert.False(DialogManager.IsDialogFinished());
      Assert.False(GameManager.Player.CanCrouch());

      TransitionManager.MakeTransition(POST_SCENE_NAME);
      while(TransitionManager.Transitioning) {
        yield return null;
      }

      Assert.True(SceneManager.GetActiveScene().name == POST_SCENE_NAME);
      Assert.True(GameManager.Player.CanCrouch());
    }

    [UnityTest]
    public IEnumerator TranisitonManager_Ends_Dialog() {
      GameManager.Player.Interact();
      Assert.True(DialogManager.IsDialogBoxOpen());
      Assert.False(DialogManager.IsDialogFinished());

      TransitionManager.MakeTransition(POST_SCENE_NAME);
      while(TransitionManager.Transitioning) {
        yield return null;
      }

      Assert.True(SceneManager.GetActiveScene().name == POST_SCENE_NAME);
      Assert.False(DialogManager.IsDialogBoxOpen());
      Assert.True(DialogManager.IsDialogFinished());
    }
  }
}
