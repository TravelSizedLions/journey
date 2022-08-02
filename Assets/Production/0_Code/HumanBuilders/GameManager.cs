using UnityEngine;
using UnityEngine.SceneManagement;

using Sirenix.OdinInspector;

namespace HumanBuilders {

  /// <summary>
  /// The top level Game Manager. All other game subsystems can be accessed through here.
  /// </summary>
  public class GameManager : Singleton<GameManager> {

    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// A constant reference to the player character.
    /// </summary>
    public static PlayerCharacter Player { 
      get { 
        if (Instance == null) {
          return null;
        }
        
        if (Instance.player == null) {
          Instance.player = FindObjectOfType<PlayerCharacter>();  
        }
        
        return Instance.player; 
      } 
    }

    /// <summary>
    /// A constant reference to the player character's companion.
    /// </summary>
    public static PlayerCompanion Companion {
      get {
        if (Instance == null) {
          Debug.LogWarning("Returning null due to uninstantiated GameManager!");
          return null;
        }

        if (Instance.companion == null) {
          Instance.companion = FindObjectOfType<PlayerCompanion>(true);
        }

        return Instance.companion;
      }
    }

    /// <summary>
    /// A reference to the mouse cursor.
    /// </summary>
    public static Mouse Mouse { 
      get { 
        if (Instance.mouse == null) {
          Instance.mouse = Instance.GetComponentInChildren<Mouse>(true);
        }

        return Instance.mouse; 
      } 
    }

    /// <summary>
    /// A virtual gamepad to control the player through script.
    /// </summary>
    public static VirtualGamepad GamePad { 
      get { 
        if (Instance.gamepad == null) {
          Instance.gamepad = Instance.GetComponentInChildren<VirtualGamepad>(true);
          if (Instance.gamepad == null) {
            GameObject go = new GameObject("gamepad");
            Instance.gamepad = go.AddComponent<VirtualGamepad>();
          }
          Instance.gamepad.Setup();
        }

        return Instance.gamepad;
      } 
    }

    private IInventory inventory;
    public static IInventory Inventory {
      get {
        if (Instance.inventory == null) {
          Instance.inventory = Instance.gameObject.GetOrAddComponent<PlayerInventory>();
        }

        return Instance.inventory;
      }
    }


    public static JourneySettings QuestSettings {
      get { 
        return JourneySettings.Instance;
      } 
    }

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

    [Header("Player Information", order=3)]
    [Space(5, order=4)]

    /// <summary>
    /// A reference to the player character.
    /// </summary>
    [Tooltip("The player character.")]
    [ReadOnly]
    private PlayerCharacter player;

    [Tooltip("The player's following companion.")]
    [ReadOnly]
    private PlayerCompanion companion;

    /// <summary>
    /// A reference to the mouse cursor.
    /// </summary>
    private Mouse mouse;

    /// <summary>
    /// A virtual gamepad to control the player through script.
    /// </summary>
    private VirtualGamepad gamepad;

    /// <summary>
    /// Where the player starts.
    /// </summary>
    [Tooltip("Where the player starts at game start.")]
    public SpawnPoint initialSpawn;

    [Space(10, order=5)]


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

    [Header("Global Settings", order=9)]
    [Space(5, order=10)]

    /// <summary>
    /// The game's universal gravity value.
    /// </summary>
    public float gravity;
    
    /// <summary>
    /// An animator for UI components.
    /// </summary>
    private Animator UIAnimator;

    /// <summary>
    /// Whether or not first-time initilization has been run.
    /// </summary>
    private static bool initialized = false;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    protected override void Awake() {
      base.Awake();

      #if UNITY_EDITOR
      // TODO: These two settings help prevent coil whine while in the Unity
      // Editor. Basically, when the frame rate is too high, any continuous
      // sound playing will cause a high pitched background noise
      QualitySettings.vSyncCount = 0;
      Application.targetFrameRate = 90;
      #endif

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
      // Set vsync to monitor refresh rate.
      if (!initialized) {
        QualitySettings.vSyncCount = 1;
        TransitionManager.SetCurrentScene(SceneManager.GetActiveScene().name);
        initialized = true;
      }

      Journey.SetNarrator(QuestSettings.Narrator);
      Journey.SetQuest(QuestSettings.MainQuest);
      Journey.Begin();
    }

    private void Start() {

      if (player != null) {
        player.Respawn();

        var cam = FindObjectOfType<TargettingCamera>();
        if (cam != null) {
          cam.transform.position = player.transform.position;
        }
      }

      UIAnimator = GetComponent<Animator>();

      InitializeVolumeSettings();
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

    private void InitializeVolumeSettings() {
      Debug.Log("Initializing");
      SoundSettings settings = Resources.Load<SoundSettings>("sound_settings");
      settings.InitializeMixer();
    }
  }
}