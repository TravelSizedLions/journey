using System.Collections;
using System.Collections.Generic;
using Storm.Subsystems.Audio;
using Storm.Cameras;
using Storm.Characters.Player;
using Storm.Subsystems.Dialog;
using Storm.Extensions;
using Storm.Subsystems.Reset;
using Storm.Subsystems.Transitions;
using Storm.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Storm {

  /// <summary>
  /// The top level Game Manager. All other game subsystems can be accessed through here.
  /// </summary>
  public class GameManager : Singleton<GameManager> {

    #region Variables
    #region Subsystems
    [Header("Subsystems", order=0)]
    [Space(5, order=1)]

    /// <summary>
    /// Handles transitioning from scene to scene.
    /// </summary>
    [Tooltip("Handles transitioning from scene to scene.")]
    [ReadOnly]
    public TransitionManager transitions;

    /// <summary>
    /// Handles resetting certain objects/behaviors.
    /// </summary>
    [Tooltip("Handles resetting certain objects/behaviors.")]
    [ReadOnly]
    public ResetManager resets;

    /// <summary>
    /// Handles sound effects.
    /// </summary>
    [Tooltip("Handles sound effects.")]
    [ReadOnly]
    public AudioManager sounds;

    /// <summary>
    /// Handles NPC dialogs.
    /// </summary>
    [Tooltip("Handles NPC dialogs.")]
    [ReadOnly]
    public DialogManager dialogs;

    [Space(10, order=2)]
    #endregion


    #region Player Information
    [Header("Player Information", order=3)]
    [Space(5, order=4)]

    /// <summary>
    /// A reference to the player character.
    /// </summary>
    [Tooltip("The player character.")]
    [ReadOnly]
    public PlayerCharacter player;

    /// <summary>
    /// Where the player starts.
    /// </summary>
    [Tooltip("Where the player starts at game start.")]
    public SpawnPoint initialSpawn;

    [Space(10, order=5)]
    #endregion

    #region Global Settings
    [Header("Global Settings", order=6)]
    [Space(5, order=7)]

    /// <summary>
    /// The game's universal gravity value.
    /// </summary>
    public float gravity;
    #endregion

    /// <summary>
    /// An animator for UI components.
    /// </summary>
    private Animator UIAnimator;

    /// <summary>
    /// The player character's instance ID. Used to rediscover the player on
    /// scene load.
    /// </summary>
    private int playerID;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected override void Awake() {
      base.Awake();
      player = FindObjectOfType<PlayerCharacter>();
      playerID = player.gameObject.GetInstanceID();
      transitions = TransitionManager.Instance;
      resets = ResetManager.Instance;
      sounds = AudioManager.Instance;
      dialogs = DialogManager.Instance;

      Physics2D.gravity = new Vector2(0, -gravity);

      string currentSpawn = transitions.GetCurrentSpawnName();
      if (currentSpawn == null || currentSpawn == "") {
        if (initialSpawn == null) {
          if (player != null) {
            transitions.RegisterSpawn("SCENE_START", GameObject.FindGameObjectWithTag("Player").transform.position, true);
            transitions.SetCurrentSpawn("SCENE_START");
          }
        } else {
          transitions.SetCurrentSpawn(initialSpawn.name);
        }
      }
      transitions.SetCurrentScene(SceneManager.GetActiveScene().name);
    }

    private void Start() {
      var cam = FindObjectOfType<TargettingCamera>();

      if (player != null) {
        player.Die();
        cam.transform.position = player.transform.position;
      }

      UIAnimator = GetComponent<Animator>();
    }


    private void Update() {
      if (Input.GetKeyDown(KeyCode.Escape)) {
        if (SceneManager.GetActiveScene().name != "main_menu") {
          ReturnToMainMenu();
        }
      }
    }

    private void FixedUpdate() {
      if (player == null) {

      }
    }


    private void FindPlayer() {
      Debug.Log("Looking for player character with instance ID: " + playerID);
      PlayerCharacter[] players = FindObjectsOfType<PlayerCharacter>();
      
      foreach (PlayerCharacter p in players) {
        if (p.gameObject.GetInstanceID() == playerID) {
          player = p;
          break;
        }
      }
    }


    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Returns to the main menu.
    /// </summary>
    public void ReturnToMainMenu() {
      TransitionManager.Instance.MakeTransition("main_menu");
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