using System.Collections;
using System.Collections.Generic;
using Storm.Subsystems.Audio;
using Storm.Cameras;
using Storm.Characters.PlayerOld;
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
    public PlayerCharacterOld player;

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

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected override void Awake() {
      base.Awake();
      player = FindObjectOfType<PlayerCharacterOld>();
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
        RespawnPlayer(player);
        cam.transform.position = player.transform.position;
      }

      UIAnimator = GetComponent<Animator>();
    }


    private void FixedUpdate() {
      if (player == null) {
        player = FindObjectOfType<PlayerCharacterOld>();
      }
    }

    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Kill the player.
    /// </summary>
    /// <param name="player">A reference to the player character</param>
    public void KillPlayer(PlayerCharacterOld player) {

      // reset everything in the level that can be reset.
      resets.Reset();
      RespawnPlayer(player);
    }


    /// <summary>
    /// Moves the player back to his last spawn point.
    /// </summary>
    /// <param name="player">A reference to the player character.</param>
    public void RespawnPlayer(PlayerCharacterOld player) {
      //TODO: Add spawn particles
      if (player == null) {
        return;
      }
      player.transform.position = transitions.GetCurrentSpawnPosition();
      if (player.Rigidbody != null) {
        player.Rigidbody.velocity = Vector3.zero;
      }
    }

    /// <summary>
    /// Returns to the main menu.
    /// </summary>
    public void ReturnToMainMenu() {
      TransitionManager.Instance.MakeTransition("MainMenu");
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