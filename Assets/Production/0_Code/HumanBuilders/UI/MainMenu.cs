using UnityEngine;
using UnityEngine.EventSystems;
namespace HumanBuilders {
  public class MainMenu : MonoBehaviour {

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
    public MenuButton CurrentButton;
    

    private void Start() {
      MenuButton[] buttons = FindObjectsOfType<MenuButton>();
      foreach (MenuButton butt in buttons) {

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
      if (actionButton && CurrentButton != null) {
        CurrentButton.onClick.Invoke();
      }
    }

    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Start playing the game.
    /// </summary>
    public void PlayGame() {
      VSave.Reset();
      VSave.CreateSlot("demo");
      VSave.ChooseSlot("demo");
      GameManager.Inventory.Clear();
      TransitionManager.MakeTransition(sceneName, spawnName);
    }

    /// <summary>
    /// Quit playing the game.
    /// </summary>
    public void QuitGame() {
      VSave.Delete("demo");
      Application.Quit();
    }
  }
}