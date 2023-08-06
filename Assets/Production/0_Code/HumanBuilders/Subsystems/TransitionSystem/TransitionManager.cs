using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

using UnityEngine.GUID;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using ReadOnlyAttribute = Sirenix.OdinInspector.ReadOnlyAttribute;

namespace HumanBuilders {

  /// <summary>
  /// This class handles transitioning the player between scenes.
  /// When a scene loads, each instance of a Transition adds its 
  /// tag and the location the player will be placed following the
  /// transition.
  /// </summary>  
  public class TransitionManager : Singleton<TransitionManager> {

    /// <summary>
    /// Unity pauses scene loading progress at 90% when AsyncOperation.allowSceneActivation
    /// </summary>
    private const float ALMOST_DONE = 0.9f;


    /// <summary>
    /// Whether or not there's already a transition going on. This is to prevent
    /// a second transition from occuring accidentally.
    /// </summary>
    private bool transitioning;

    /// <summary>
    /// The list of transitions that can be played. This is for the inspector.
    /// </summary>
    [Space(5)]
    [TableList]
    [Tooltip("The list of transitions that can be played.")]
    public List<TransitionEffect> Transitions;

    /// <summary>
    /// A list of events that will fire between when the previous scene ends and
    /// the next scene begins.
    /// </summary>
    private List<UnityEvent> transitionEvents;

    /// <summary>
    /// The set of transitions that can be played.
    /// </summary>
    private Dictionary<string, TransitionEffect> transitionEffects; 


    [Header("Event Handling", order = 0)]
    [Space(5, order = 1)]
    /** 
     * TODO: it looks like there's no way these are actually being used yet.
     * Eventually we'll either need to add support for transitions with events
     * attached to them, or remove these variables.
     */

    /// <summary>
    /// Allows other code to register events that fire just before the scene transitions
    /// to the next scene.
    /// </summary>
    [Tooltip("Allows other code to register events that fire just before the scene transitions to the next scene.")]
    [SerializeField]
    [ReadOnly]
    private UnityEvent preTransitionEvents;

    /// <summary>
    /// Allows other code to register events that fire just after the transition to another scene.
    /// </summary>
    [Tooltip("Allows other code to register events that fire just after the transition to another scene.")]
    [SerializeField]
    [ReadOnly]
    private UnityEvent postTransitionEvents;

    [Space(10, order = 2)]


    [Header("Spawn Point Debug Info", order = 3)]
    [Space(5, order = 5)]

    /// <summary>
    /// Where the player is currently set to respawn (e.g. If the player dies).
    /// </summary>
    [Tooltip("Where the player is currently set to respawn (e.g. If the player dies).")]
    [SerializeField]
    [ReadOnly]
    private string currentSpawnName;

    private GuidReference currentSpawn;

    /// <summary>
    /// The name of the current unity scene the player is in.
    /// </summary>
    [Tooltip("The name of the current unity scene the player is in.")]
    [SerializeField]
    [ReadOnly]
    private string currentSceneName;

    private string nextSceneName;

    private string previousSceneName;

    /// <summary>
    /// The name of the next unity scene to load.
    /// </summary>
    [Tooltip("The name of the next unity scene to load.")]
    [SerializeField]
    [ReadOnly]

    private Scene previousScene;

    private PlayerCharacter player;

    [Space(10, order = 4)]


    /// <summary>
    /// A dictionary of spawn points that the player can respawn at. When a scene loads, all spawn points
    /// register their positions in this dictionary. Only the current scene's spawn points are kept in this
    /// dictionary.
    /// </summary>
    /// <typeparam name="string">The name of the spawn</typeparam>
    /// <typeparam name="Vector3">The position of the spawn</typeparam>
    private Dictionary<string, Vector3> spawnPoints = new Dictionary<string, Vector3>();

    /// <summary>
    /// A dictionary that maps a spawn point to information about whether the player should spawn facing left
    /// or facing right at a given spawn point.
    /// </summary>
    /// <typeparam name="string">The name of the spawn</typeparam>
    /// <typeparam name="bool">Whether to spawn left or right. True = right, False = left</typeparam>
    private Dictionary<string, bool> spawnLeftRight = new Dictionary<string, bool>();

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected override void Awake() {
      if (preTransitionEvents == null) {
        preTransitionEvents = new UnityEvent();
      }

      if (postTransitionEvents == null) {
        postTransitionEvents = new UnityEvent();
      }


      transitionEffects = new Dictionary<string, TransitionEffect>();
      foreach (TransitionEffect effect in Transitions) {
        transitionEffects.Add(effect.Name, effect);
      }

      base.Awake();
    }

    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Sets the current spawn for the player.
    /// </summary>
    /// <param name="spawnName">The name of the SpawnPoint (In the unity level editor hierarchy).</param>
    public static void SetCurrentSpawn(string spawnName) => Instance.currentSpawnName = spawnName;


    public static void SetCurrentSpawn(GuidReference spawn) => Instance.currentSpawn = spawn;

    /// <summary>
    /// Returns the name of the current spawn.
    /// </summary>
    public static string GetCurrentSpawnName() => Instance.currentSpawnName;

    /// <summary>
    /// Returns whether the player should be facing left or right when respawning at the current spawn.
    /// True = right, False = left.
    /// </summary>
    public static bool GetCurrentSpawnFacing() => Instance.InnerGetCurrentSpawnFacing();
    private bool InnerGetCurrentSpawnFacing() {
      string spawnName = (currentSpawn != null) ? currentSpawn.gameObject.name : currentSpawnName;
      if (spawnLeftRight.ContainsKey(spawnName)) {
        return spawnLeftRight[spawnName];
      }

      throw new UnityException("Could not get current spawn facing information.");
    }

    /// <summary>
    /// Get the position of the current spawn point for the player.
    /// </summary>
    public static Vector3 GetCurrentSpawnPosition() => Instance.InnerGetCurrentSpawnPosition();
    private Vector3 InnerGetCurrentSpawnPosition() {
      string spawnName = (currentSpawn != null) ? currentSpawn.gameObject?.name : currentSpawnName;
      if (spawnPoints.ContainsKey(spawnName)) {
        return spawnPoints[spawnName];
      }

      throw new UnityException("Could not get current spawn location: \"" + currentSpawnName + "\"");
    }

    /// <summary>
    /// Sets the current scene of the game.
    /// </summary>
    /// <param name="sceneName">The name of the scene (path not needed, just the name of the scene itself).</param>
    public static void SetCurrentScene(string sceneName) => Instance.currentSceneName = sceneName;
    
    /// <summary>
    /// Returns the name of the current scene.
    /// </summary>
    public static string GetCurrentScene() => Instance.currentSceneName;

    /// <summary>
    /// Registers the location of a SpawnPoint with the transition manager.
    /// </summary>
    /// <param name="name">The name of the spawn (in editor)</param>
    /// <param name="pos">The spawn's position</param>
    /// <param name="right">Whether or not the player should be facing right or left upon respawn for this SpawnPoint. True = right, False = left</param>
    public static void RegisterSpawn(string name, Vector3 pos, bool right) => Instance.InnerRegisterSpawn(name, pos, right);
    private void InnerRegisterSpawn(string name, Vector3 pos, bool right) {
      if (name == null) return;

      if (!spawnPoints.ContainsKey(name)) {
        spawnPoints.Add(name, pos);
        spawnLeftRight.Add(name, right);
      }
    }

    /// <summary>
    /// Clears the spawn point registry.
    /// </summary>
    public static void ClearSpawnPoints() => Instance.InnerClearSpawnPoints();
    private void InnerClearSpawnPoints() {
      spawnPoints.Clear();
      spawnLeftRight.Clear();
    }


    public static void AddTransitionEvents(UnityEvent events) => Instance.AddTransitionEvents_Inner(events);
    private void AddTransitionEvents_Inner(UnityEvent events) {
      transitionEvents = transitionEvents ?? new List<UnityEvent>();
      transitionEvents.Add(events);
    }


    /// <summary>
    /// Reloads the current scene.
    /// </summary>
    public static void ReloadScene() => MakeTransition(Instance.currentSceneName);

    /// <summary>
    /// Perform transition to another scene. The player will be placed wherever the 
    /// PlayerCharacter is placed in the editor, regardless of SpawnPoints.
    /// </summary>
    /// <param name="scene">The name of the next scene (path not needed, just the name of the scene itself)</param>
    public static void MakeTransition(string scene) => Instance.InnerMakeTransition(scene, "", "");

    /// <summary>
    /// Perform a transition to another scene.
    /// </summary>
    /// <param name="scene">The name of the next scene (path not needed, just the name of the scene itself)</param>
    /// <param name="spawn">The name of the spawn where the player should start in the next scene.</param>
    public static void MakeTransition(string scene, string spawn) => Instance.InnerMakeTransition(scene, spawn, "");

    /// <summary>
    /// Perform a transition to another scene.
    /// </summary>
    /// <param name="scene">The name of the next scene (path not needed, just the name of the scene itself)</param>
    /// <param name="spawn">The name of the spawn where the player should start in the next scene.</param>
    /// <param name="vcamName">The name virtual camera view that the scene should start focused on.</param>
    public static void MakeTransition(string scene, string spawn, string vcamName) => Instance.InnerMakeTransition(scene, spawn, vcamName);

    /// <summary>
    /// Perform a transition to another scene.
    /// </summary>
    /// <param name="scene">The name of the next scene (path not needed, just the name of the scene itself)</param>
    /// <param name="spawn">The name of the spawn where the player should start in the next scene.</param>
    /// <param name="vcamName">The name virtual camera view that the scene should start focused on.</param>
    private void InnerMakeTransition(string scene, string spawn, string vcamName) {
      if (transitioning) {
        return;
      }

      transitioning = true;

      if (PauseScreen.Paused) {
        PauseScreen.ContinueGame();
      }

      preTransitionEvents.Invoke();
      preTransitionEvents.RemoveAllListeners();
      ClearSpawnPoints();

      nextSceneName = scene;
      SetCurrentSpawn(spawn);

      transitionEffects["fade_to_black"].SetBool("FadeToBlack", true);
    }

    /// <summary>
    /// Animation event callback. Called after the animation triggered in MakeTransition() finishes.
    /// </summary>
    public static void OnTransitionComplete() => Instance.InnerOnTransitionComplete();
    private void InnerOnTransitionComplete() {     
      Time.timeScale = 1; 
      StartCoroutine(MakeTransition());
    }

    /// <summary>
    /// Move a GameObject from the current scene to another scene.
    /// </summary>
    /// <param name="sceneName">Name of the scene you want to load.</param>
    /// <param name="targetGameObject">GameObject you want to move to the new scene.</param>
    public IEnumerator MakeTransition() {
      if (nextSceneName != null) {

        FireTransitionEvents();

        // Force any dialog to end if necessary
        DialogManager.EndDialog();

        // Break any locks on player behavior
        if (player == null) {
          player = GameManager.Player;
          player?.EnableAllBehaviors();
        }

        // get the current active scene
        previousScene = SceneManager.GetActiveScene();
        previousSceneName = previousScene.name;

        SetCurrentScene(nextSceneName);

        if (previousSceneName != nextSceneName) {
          yield return LoadScene();
        } else {
          ResetScene();
        }
      }

      transitionEffects["fade_to_black"].SetBool("FadeToBlack", false);
      transitioning = false;
    }

    private IEnumerator LoadScene() {
      // load the new scene in the background
      AsyncOperation async = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive); 

      while (!async.isDone) {
        yield return null;
      }

      // Move the existing player to the next scene. Since just about every Unity scene has
      // a copy of the player character prefab in it for convenience, and we only ever want one
      // persistent player, we need to destroy the one that starts in the scene.
      if (player != null) {
        
        // Ensure that the player will always be active in the next scene.
        player.gameObject.SetActive(true);
        player.FSM.Resume();
        player.Physics.GravityScale = 1;

        Scene nextScene = SceneManager.GetSceneByName(nextSceneName);
        if (nextScene.IsValid()) {

          foreach (var go in nextScene.GetRootGameObjects()) {
            if (go.CompareTag("Player")) {
              Destroy(go);
            }
          }
        }
        
        SceneManager.MoveGameObjectToScene(player.gameObject, nextScene);
        ResetScene();
      }

      SceneManager.UnloadSceneAsync(previousScene);
    }

    private void ResetScene() {
      ResetManager.Reset();
      player.Respawn();
      TargettingCamera.ClearTarget();
    }

    private void FireTransitionEvents() {
      if (transitionEvents != null) {
        foreach (UnityEvent e in transitionEvents) {
          if (e != null) {
            e.Invoke();
          }
        }

        transitionEvents.Clear();
      }      
    }


    /// <summary>
    /// Play a wipe animation.
    /// </summary>
    public static void Wipe() {
      Instance.transitionEffects["wipe"].SetTrigger("wipe");
    }
  }
}