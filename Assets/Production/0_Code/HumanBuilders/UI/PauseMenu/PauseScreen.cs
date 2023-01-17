


using UnityEngine;
using UnityEngine.SceneManagement;

namespace HumanBuilders {

  public class PauseScreen : Singleton<PauseScreen> {

    //-------------------------------------------------------------------------
    // Read-Only Props
    //-------------------------------------------------------------------------

    /// <summary>
    /// The canvas component that contains the whole pause menu.
    /// </summary>
    public static Canvas Canvas { get { return pauseScreenCanvas; } }

    /// <summary>
    /// Whether or not the game is paused.
    /// </summary>
    public static bool Paused { get { return paused; } }

    /// <summary>
    /// The screen that contains the main pause menu.
    /// </summary>
    public static GameObject MainPauseMenu { get { return Instance.mainPauseMenu; } }

    /// <summary>
    /// The screen that contains the scenes index.
    /// </summary>
    public static GameObject ScenesMenu { get { return Instance.scenesMenu; } }

    /// <summary>
    /// The screen that contains the scenes index.
    /// </summary>
    public static GameObject SettingsMenu { get { return Instance.scenesMenu; } }

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The screen that contains the main pause menu.
    /// </summary>
    [Tooltip("The screen that contains the main pause menu.")]
    [SerializeField]
    private GameObject mainPauseMenu = null;

    /// <summary>
    /// The screen that contains the scenes index.
    /// </summary>
    [Tooltip("The screen that contains the scenes index.")]
    [SerializeField]
    private GameObject scenesMenu = null;

    /// <summary>
    /// The screen that contains the scenes index.
    /// </summary>
    [Tooltip("The screen that contains the scenes index.")]
    [SerializeField]
    private GameObject settingsMenu = null;

    /// <summary>
    /// The the game object that hosts the pause screen.
    /// </summary>
    private GameObject pauseScreen;

    /// <summary>
    /// The UI canvas for the pause screen;
    /// </summary>
    private static Canvas pauseScreenCanvas;

    /// <summary>
    /// Whether or not the game is paused.
    /// </summary>
    private static bool paused = false;


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected new void Awake() {
      pauseScreen = transform.GetComponentsInChildren<Canvas>(true)[0].gameObject;
      pauseScreenCanvas = pauseScreen.GetComponentInChildren<Canvas>(true);
      pauseScreen.SetActive(false);
      ContinueGame();
      base.Awake();
    }

    private void Update() {
      if (Input.GetButtonDown("Cancel")) {
        if (SceneManager.GetActiveScene().name != "main_menu") {
          if (pauseScreen.activeSelf) {
            if (mainPauseMenu != null && mainPauseMenu.activeSelf) {
              ContinueGame();
            } else if (scenesMenu != null && scenesMenu.activeSelf) {
              ReturnToMainPauseMenuFromScenes();
            }
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
    public static void PauseGame() => Instance.PauseGame_Inner();
    private void PauseGame_Inner() {
      if (pauseScreen != null) {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
        paused = true;
        mainPauseMenu.SetActive(true);
        scenesMenu.SetActive(false);
      }
    }

    /// <summary>
    /// Unpause the game!
    /// </summary>
    public static void ContinueGame() => Instance.ContinueGame_Inner();
    private void ContinueGame_Inner() {
      if (pauseScreen != null) {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
        paused = false;
        mainPauseMenu.SetActive(true);
        scenesMenu.SetActive(false);
      }
    }


    public static void OpenScenesMenu() => Instance.OpenScenesMenu_Inner();
    private void OpenScenesMenu_Inner() {
      if (paused && mainPauseMenu != null && scenesMenu != null) {
        mainPauseMenu.SetActive(false);
        scenesMenu.SetActive(true);
      }
    }

    public static void OpenSettingsMenu() => Instance.OpenSettingsMenu_Inner();
    private void OpenSettingsMenu_Inner() {
      if (paused && mainPauseMenu != null && settingsMenu != null) {
        mainPauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
      }
    }

    public static void ReturnToMainPauseMenuFromScenes() => Instance.ReturnToMainPauseMenuFromScenes_Inner();
    private void ReturnToMainPauseMenuFromScenes_Inner() {
      if (paused & mainPauseMenu != null && scenesMenu != null) {
        scenesMenu.SetActive(false);
        mainPauseMenu.SetActive(true);
      }
    }

    public static void ReturnToMainPauseMenuFromSettings() => Instance.ReturnToMainPauseMenuFromSettings_Inner();
    private void ReturnToMainPauseMenuFromSettings_Inner() {
      if (paused & mainPauseMenu != null && settingsMenu != null) {
        settingsMenu.SetActive(false);
        mainPauseMenu.SetActive(true);
      }
    }

    /// <summary>
    /// Returns to the main menu.
    /// </summary>
    public void ReturnToMainMenu() {
      Time.timeScale = 1;
      TransitionManager.MakeTransition("main_menu");
      pauseScreen.SetActive(false);
      DialogManager.EndDialog();
      paused = false;
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
