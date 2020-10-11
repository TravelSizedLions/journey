using System.Collections;
using System.Collections.Generic;
using Storm.Extensions;
using Storm.Subsystems.Dialog;
using Storm.Subsystems.Transitions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Storm.UI {

  public class PauseScreen : Singleton<PauseScreen> {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The the game object that hosts the pause screen.
    /// </summary>
    private GameObject pauseScreen;


    /// <summary>
    /// The button for resuming the game. Set this in the inspector
    /// </summary>
    [Tooltip("The button for resuming the game.")]
    public MenuButton resumeButton;

    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected new void Awake() {
      base.Awake();
      pauseScreen = transform.GetChild(0).gameObject;
      pauseScreen.SetActive(false);
    }

    private void Update() {
      if (Input.GetKeyDown(KeyCode.Escape)) {
        if (SceneManager.GetActiveScene().name != "main_menu") {
          if (pauseScreen.activeSelf) {
            ContinueGame();
          } else {
            PauseGame();
          }
        }
      }
    }


    #endregion


    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Pause the game!
    /// </summary>
    public void PauseGame() {
      if (pauseScreen != null) {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
      }
    }

    /// <summary>
    /// Unpause the game!
    /// </summary>
    public void ContinueGame() {
      if (pauseScreen != null) {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
      }
    }

    /// <summary>
    /// Returns to the main menu.
    /// </summary>
    public void ReturnToMainMenu() {
      TransitionManager.MakeTransition("main_menu");
      pauseScreen.SetActive(false);
      DialogManager.Instance.EndDialog();
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void ExitApplication() {
      Application.Quit();
    }

    #endregion
  }
}
