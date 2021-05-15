


using UnityEngine;
using UnityEngine.SceneManagement;

namespace HumanBuilders {

  public class PauseScreen : Singleton<PauseScreen> {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    public static Canvas Canvas { get { return pauseScreenCanvas; } }

    /// <summary>
    /// Whether or not the game is paused.
    /// </summary>
    public static bool Paused { get { return paused; } }

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
      pauseScreen = transform.GetChild(0).gameObject;
      pauseScreenCanvas = pauseScreen.GetComponent<Canvas>();
      base.Awake();
      ContinueGame();
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
    public static void PauseGame() => Instance.PauseGame_Inner();
    private void PauseGame_Inner() {
      if (pauseScreen != null) {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
        paused = true;
        pauseScreenCanvas.sortingOrder = DialogManager.Canvas.sortingOrder+1;
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
        pauseScreenCanvas.sortingOrder = DialogManager.Canvas.sortingOrder-1;
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
