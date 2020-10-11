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
using UnityEngine.UI;
using Storm.Subsystems.Save;
using Storm.UI;

namespace Storm {

  /// <summary>
  /// The top level Game Manager. All other game subsystems can be accessed through here.
  /// </summary>
  public class GameManager : Singleton<GameManager> {

    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// A constant reference to the player character.
    /// </summary>
    public static PlayerCharacter Player { 
      get { 
        if (Instance.player == null) {
          Instance.player = FindObjectOfType<PlayerCharacter>();  
        }
        
        return Instance.player; 
      } 
    }

    /// <summary>
    /// A reference to the mouse cursor.
    /// </summary>
    public static Mouse Mouse { get { return Instance.mouse; } }
    #endregion

    #region Variables
    #region Subsystems
    [Header("Subsystems", order=0)]
    [Space(5, order=1)]

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
    private PlayerCharacter player;

    /// <summary>
    /// A reference to the mouse cursor.
    /// </summary>
    private Mouse mouse;

    /// <summary>
    /// Where the player starts.
    /// </summary>
    [Tooltip("Where the player starts at game start.")]
    public SpawnPoint initialSpawn;

    [Space(10, order=5)]
    #endregion

    #region Scene Information

    [Header("Scene Information", order=6)]
    [Space(5, order=7)]
    /// <summary>
    /// The current scene's targetting camera.
    /// </summary>
    [Tooltip("The current scene's targetting camera.")]
    public static TargettingCamera CurrentTargettingCamera;

    /// <summary>
    /// The current scene's camera. This is the camera attached to the
    /// TargettingCamera script.
    /// </summary>
    [Tooltip("The current scene's camera.")]   
    public static Camera CurrentCamera;

    [Space(10, order=8)]
    #endregion

    #region Global Settings
    [Header("Global Settings", order=9)]
    [Space(5, order=10)]

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
    /// Whether or not first-time initilization has been run.
    /// </summary>
    private static bool initialized = false;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected override void Awake() {
      base.Awake();
      player = FindObjectOfType<PlayerCharacter>();
      mouse = GetComponentInChildren<Mouse>(true);

      resets = ResetManager.Instance;
      sounds = AudioManager.Instance;
      dialogs = DialogManager.Instance;

      Physics2D.gravity = new Vector2(0, -gravity);

      string currentSpawn = TransitionManager.GetCurrentSpawnName();
      if (currentSpawn == null || currentSpawn == "") {
        if (initialSpawn == null) {
          if (player != null) {
            TransitionManager.RegisterSpawn("SCENE_START", GameObject.FindGameObjectWithTag("Player").transform.position, true);
            TransitionManager.SetCurrentSpawn("SCENE_START");
          }
        } else {
          TransitionManager.SetCurrentSpawn(initialSpawn.name);
        }
      }

      // Set the current scene in the transition manager;
      if (!initialized) {

        TransitionManager.SetCurrentScene(SceneManager.GetActiveScene().name);
        initialized = true;
      }
    }

    private void Start() {
      var cam = FindObjectOfType<TargettingCamera>();

      if (player != null) {
        player.Respawn();
        cam.transform.position = player.transform.position;
      }

      UIAnimator = GetComponent<Animator>();
    }

    private void FixedUpdate() {
      if (player == null) {
        FindPlayer();
      }
    }


    private void FindPlayer() {
      PlayerCharacter[] players = FindObjectsOfType<PlayerCharacter>();
      if (players.Length == 1) {
        player = players[0];
      }
    }

    #endregion

    #region Public Interface
    #endregion
  }
}