using System.Collections;
using System.Collections.Generic;
using Storm.Attributes;
using Storm.Subsystems.Transitions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Storm.UI {

  /// <summary>
  /// A UI for allowing the player to select which level they'd like to play.
  /// </summary>
  public class LevelSelect : MonoBehaviour {

    #region Variables

    #region Level Selection
    [Header("Level Selection", order = 0)]
    [Space(5, order = 1)]

    /// <summary>
    /// The list of level options
    /// </summary>
    [Tooltip("The list of level options.")]
    public List<LevelSelectOption> Options;

    /// <summary>
    /// The currently selected level option
    /// </summary>
    [Tooltip("The currently selected level option.")]
    [SerializeField]
    [ReadOnly]
    private int currentOption;

    /// <summary>
    /// The image on the screen being displayed.
    /// </summary>
    private Image display;


    /// <summary>
    /// The name of the level displayed on screen.
    /// </summary>
    private TextMeshProUGUI textMesh;

    [Space(10, order = 2)]
    #endregion

    #region Buttons
    [Header("Buttons", order = 3)]
    [Space(5, order = 4)]

    /// <summary>
    /// The button for scrolling left.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private Button leftButton;

    /// <summary>
    /// The button for scrolling right.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private Button rightButton;
    #endregion
    #endregion

    #region Unity API
    //---------------------------------------------------------------------
    // Unity API
    //---------------------------------------------------------------------

    private void Awake() {
      // Get the necessary components.
      display = GetComponent<Image>();

      textMesh = GetComponentInChildren<TextMeshProUGUI>();

      Button[] buttons = GetComponentsInChildren<Button>();
      leftButton = buttons[0];
      rightButton = buttons[1];


      // Initialize model & on screen UI.
      if (Options == null) {
        Options = new List<LevelSelectOption>();
        currentOption = -1;

        disableLeftButton();
        disableRightButton();
      } else {
        // Start at the center of the list so the player
        // can scroll left or right.
        currentOption = Mathf.FloorToInt(Options.Count / 2 - 1);
        reloadLevelSelection();
        checkButtons();
      }

    }

    private void Update() {
      if (Input.GetButtonDown("Jump")) {
        selectLevel();
      } else {
        if (leftButton.interactable && Input.GetButtonDown("Left")) {
          ScrollLeft();
        } else if (rightButton.interactable && Input.GetButtonDown("Right")) {
          ScrollRight();
        }
      }
    }
    #endregion

    #region Button Click Handlers
    //---------------------------------------------------------------------
    // Button Click Handlers
    //---------------------------------------------------------------------

    /// <summary>
    /// Scroll the level selection list left.
    /// </summary>
    private void ScrollLeft() {
      currentOption--;
      reloadLevelSelection();
      checkButtons();
    }

    /// <summary>
    /// Scroll the level selection list right.
    /// </summary>
    private void ScrollRight() {
      currentOption++;
      reloadLevelSelection();
      checkButtons();
    }
    #endregion

    #region Auxiliary Methods
    //---------------------------------------------------------------------
    // Auxiliary Methods
    //---------------------------------------------------------------------

    private void selectLevel() {
      if (currentOption >= 0) {
        LevelSelectOption option = Options[currentOption];
        TransitionManager.Instance.MakeTransition(option.GetSceneName());
      }
    }

    /// <summary>
    /// 
    /// </summary>
    private void reloadLevelSelection() {
      if (currentOption >= 0) {
        LevelSelectOption option = Options[currentOption];

        display.sprite = option.GetLevelImage();
        textMesh.text = option.GetTitle();
      }
    }


    /// <summary>
    /// Check to see which arrows should be enabled/disabled.
    /// </summary>
    private void checkButtons() {
      checkLeftButton();
      checkRightButton();
    }


    /// <summary>
    /// Check if the left arrow button should start enabled.
    /// </summary>
    private void checkLeftButton() {
      if (currentOption == 0) {
        disableLeftButton();
      } else {
        enableLeftButton();
      }
    }


    /// <summary>
    /// Check if the right arrow button should start enabled.
    /// </summary>
    private void checkRightButton() {
      if (currentOption == Options.Count - 1) {
        disableRightButton();
      } else {
        enableRightButton();
      }
    }
    #endregion

    #region Button Toggles
    //---------------------------------------------------------------------
    // Button Toggles
    //---------------------------------------------------------------------

    /// <summary>
    /// Enable scrolling to the left.
    /// </summary>
    private void enableLeftButton() {
      leftButton.interactable = true;
    }

    /// <summary>
    /// Disable scrolling to the left.
    /// </summary>
    private void disableLeftButton() {
      leftButton.interactable = false;
    }

    /// <summary>
    /// Enable scrolling to the right.
    /// </summary>
    private void enableRightButton() {
      rightButton.interactable = true;
    }

    /// <summary>
    /// Disable scrolling to the right.
    /// </summary>
    private void disableRightButton() {
      rightButton.interactable = false;
    }
    #endregion
  }
}