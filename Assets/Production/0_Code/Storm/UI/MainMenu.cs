﻿using System.Collections;
using System.Collections.Generic;
using Storm.Subsystems.Transitions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using Storm.Subsystems.Save;

namespace Storm.UI {

  /// <summary>
  /// The behavior for the Game's title menu.
  /// </summary>
  /// <seealso cref="LevelSelect" />
  public class MainMenu : MonoBehaviour {

    #region Variables
    [Header("Starting Scene Information", order=0)]
    [Space(5, order=1)]

    /// <summary>
    /// The name of the scene to load.
    /// </summary>
    [Tooltip("The name of the scene to load.")]
    [SerializeField]
    private string sceneName = "";

    /// <summary>
    /// The name of the spawn position for the player.
    /// </summary>
    [Tooltip("The name of the spawn position for the player.")]
    [SerializeField]
    private string spawnName = "";


    /// <summary>
    /// The current button selected by the player. The button may not
    /// necessarily have focus.
    /// </summary>
    public MainMenuButton CurrentButton;
    
    #endregion

    private void Start() {
      MainMenuButton[] buttons = FindObjectsOfType<MainMenuButton>();
      Debug.Log("Numb butts: " + buttons.Length);
      foreach (MainMenuButton butt in buttons) {

        if (butt.name == "PlayButton") {  
          butt.gameObject.SetActive(true);
          butt.Select();
          butt.interactable = true;
          EventSystem.current.SetSelectedGameObject(butt.gameObject);
          break;
        }
      }
    }


    private void Update() {
      bool up = Input.GetButtonDown("Up");
      bool down = Input.GetButtonDown("Down");
      
      if (up || down) {
        CurrentButton.Select();
        EventSystem.current.SetSelectedGameObject(CurrentButton.gameObject);
      }


      bool actionButton = Input.GetButtonDown("Action");
      if (actionButton) {
        CurrentButton.onClick.Invoke();
      }
    }

    #region  Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Start playing the game.
    /// </summary>
    public void PlayGame() {
      VSave.CreateSlot("demo");
      VSave.ChooseSlot("demo");
      TransitionManager.Instance.MakeTransition(sceneName, spawnName);
    }

    /// <summary>
    /// Quit playing the game.
    /// </summary>
    public void QuitGame() {
      Debug.Log("Quitting!");
      VSave.Delete("demo");
      Application.Quit();
    }

    #endregion
  }
}