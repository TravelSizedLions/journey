using System.Collections;
using System.Collections.Generic;
using Storm.Attributes;
using Storm.Cameras;
using Storm.Characters;
using Storm.Characters.Player;
using Storm.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Storm.Subsystems.Transitions {

  /// <summary>
  /// This class handles transitioning the player between scenes.
  /// When a scene loads, each instance of a Transition adds its 
  /// tag and the location the player will be placed following the
  /// transition.
  /// </summary>  
  public class TransitionManager : Singleton<TransitionManager> {

    #region Variables
    /// <summary>
    /// Unity pauses scene loading progress at 90% when AsyncOperation.allowSceneActivation
    /// </summary>
    private const float ALMOST_DONE = 0.9f;

    /// <summary>
    /// This animator handles fading in and out of the scene.
    /// </summary>
    private Animator transitionAnim = null;


    #region Event Handling
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
    #endregion

    #region Spawn Point Debug Info
    [Header("Spawn Point Debug Info", order = 3)]
    [Space(5, order = 5)]

    /// <summary>
    /// Where the player is currently set to respawn (e.g. If the player dies).
    /// </summary>
    [Tooltip("Where the player is currently set to respawn (e.g. If the player dies).")]
    [SerializeField]
    [ReadOnly]
    private string currentSpawnName;

    /// <summary>
    /// The name of the current unity scene the player is in.
    /// </summary>
    [Tooltip("The name of the current unity scene the player is in.")]
    [SerializeField]
    [ReadOnly]
    private string currentSceneName;

    private string nextSceneName;

    private string prevousSceneName;

    /// <summary>
    /// The name of the next unity scene to load.
    /// </summary>
    [Tooltip("The name of the next unity scene to load.")]
    [SerializeField]
    [ReadOnly]

    private Scene previousScene;

    private PlayerCharacter player;

    [Space(10, order = 4)]
    #endregion


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
    #endregion

    #region Unity API
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

      transitionAnim = GetComponent<Animator>();

      base.Awake();
    }

    #endregion

    #region  Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    #region Getters/Setters

    /// <summary>
    /// Sets the current spawn for the player.
    /// </summary>
    /// <param name="spawnName">The name of the SpawnPoint (In the unity level editor hierarchy).</param>
    public void SetCurrentSpawn(string spawnName) {
      currentSpawnName = spawnName;
    }

    /// <summary>
    /// Returns the name of the current spawn.
    /// </summary>
    /// <returns></returns>
    public string GetCurrentSpawnName() {
      return currentSpawnName;
    }

    /// <summary>
    /// Returns whether the player should be facing left or right when respawning at the current spawn.
    /// True = right, False = left.
    /// </summary>
    public bool GetCurrentSpawnFacing() {
      if (spawnLeftRight.ContainsKey(currentSpawnName)) {
        return spawnLeftRight[currentSpawnName];
      }

      throw new UnityException("Could not get current spawn facing information.");
    }

    /// <summary>
    /// Get the position of the current spawn point for the player.
    /// </summary>
    public Vector3 GetCurrentSpawnPosition() {
      if (spawnPoints.ContainsKey(currentSpawnName)) {
        return spawnPoints[currentSpawnName];
      }

      throw new UnityException("Could not get current spawn location.");
    }


    /// <summary>
    /// Sets the current scene of the game.
    /// </summary>
    /// <param name="sceneName">The name of the scene (path not needed, just the name of the scene itself).</param>
    public void SetCurrentScene(string sceneName) {
      currentSceneName = sceneName;
    }

    /// <summary>
    /// Returns the name of the current scene.
    /// </summary>
    /// <returns></returns>
    public static string GetCurrentScene() {
      return Instance.currentSceneName;
    }

    #endregion

    #region Spawn Point Management

    /// <summary>
    /// Registers the location of a SpawnPoint with the transition manager.
    /// </summary>
    /// <param name="name">The name of the spawn (in editor)</param>
    /// <param name="pos">The spawn's position</param>
    /// <param name="right">Whether or not the player should be facing right or left upon respawn for this SpawnPoint. True = right, False = left</param>
    public void RegisterSpawn(string name, Vector3 pos, bool right) {
      if (name == null) return;

      if (!spawnPoints.ContainsKey(name)) {
        spawnPoints.Add(name, pos);
        spawnLeftRight.Add(name, right);
      }
    }

    /// <summary>
    /// Clears the spawn point registry.
    /// </summary>
    public void ClearSpawnPoints() {
      spawnPoints.Clear();
      spawnLeftRight.Clear();
    }

    #endregion

    #region Scene Transition Management

    /// <summary>
    /// Reloads the current scene.
    /// </summary>
    public void ReloadScene() {
      MakeTransition(currentSceneName);
    }


    /// <summary>
    /// Perform transition to another scene. The player will be placed wherever the 
    /// PlayerCharacter is placed in the editor, regardless of SpawnPoints.
    /// </summary>
    /// <param name="scene">The name of the next scene (path not needed, just the name of the scene itself)</param>
    public void MakeTransition(string scene) {
      MakeTransition(scene, "");
    }

    /// <summary>
    /// Perform a transition to another scene.
    /// </summary>
    /// <param name="scene">The name of the next scene (path not needed, just the name of the scene itself)</param>
    /// <param name="spawn">The name of the spawn where the player should start in the next scene.</param>
    public void MakeTransition(string scene, string spawn) {
      MakeTransition(scene, spawn, "");
    }

    /// <summary>
    /// Perform a transition to another scene.
    /// </summary>
    /// <param name="scene">The name of the next scene (path not needed, just the name of the scene itself)</param>
    /// <param name="spawn">The name of the spawn where the player should start in the next scene.</param>
    /// <param name="vcamName">The name virtual camera view that the scene should start focused on.</param>
    public void MakeTransition(string scene, string spawn, string vcamName) {
      preTransitionEvents.Invoke();
      preTransitionEvents.RemoveAllListeners();
      ClearSpawnPoints();

      Debug.Log(spawn);

      nextSceneName = scene;
      SetCurrentSpawn(spawn);

      transitionAnim.SetBool("FadeToBlack", true);
      TargettingCamera.SetTarget(vcamName);
    }

    /// <summary>
    /// Animation event callback. Called after the animation triggered in MakeTransition() finishes.
    /// </summary>
    public void OnTransitionComplete() {      
      StartCoroutine(LoadScene());
    }

    /// <summary>
    /// Move a GameObject from the current scene to another scene.
    /// </summary>
    /// <param name="sceneName">Name of the scene you want to load.</param>
    /// <param name="targetGameObject">GameObject you want to move to the new scene.</param>
    public IEnumerator LoadScene() {
      if (player == null) {
        player = GameManager.Instance.player;
      }

      // get the current active scene
      previousScene = SceneManager.GetActiveScene();
      prevousSceneName = previousScene.name;

      SetCurrentScene(nextSceneName);

      // load the new scene in the background
      AsyncOperation async = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);

      while (!async.isDone) {
        yield return null;
      }

      // Move the existing player to the next scene. Since just about every Unity scene has
      // a copy of the player character prefab in it for convenience, and we only ever want one
      // persistent player, we need to destroy the one that starts in the scene.
      if (player != null) {
        Scene nextScene = SceneManager.GetSceneByName(nextSceneName);
        foreach (var go in nextScene.GetRootGameObjects()) {
          if (go.CompareTag("Player")) {
            Destroy(go);
          }
        }

        SceneManager.MoveGameObjectToScene(player.gameObject, nextScene);

        player.Die();
      }

      SceneManager.UnloadSceneAsync(previousScene);

      transitionAnim.SetBool("FadeToBlack", false);
    }
    #endregion

    #endregion
  }
}