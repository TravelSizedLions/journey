using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

using Storm.Extensions;
using Storm.Characters.Player;
using Storm.Cameras;

namespace Storm.TransitionSystem {

    /// <summary>
    /// This class handles transitioning the player between scenes.
    /// When a scene loads, each instance of a Transition adds its 
    /// tag and the location the player will be placed following the
    /// transition.
    /// </summary>  
    public class TransitionManager : Singleton<TransitionManager> {

        /// <summary>
        /// This animator handles fading in and out of the scene.
        /// </summary>
        public Animator transitionAnim;

        /// <summary>
        /// Allows other code to register events that fire just before the scene transitions
        /// to the next scene.
        /// </summary>
        public UnityEvent preTransitionEvents;

        /// <summary>
        /// Allows other code to register events that fire just after the transition to another scene.
        /// </summary>
        public UnityEvent postTransitionEvents;

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

        /// <summary>
        /// Where the player is currently set to respawn (e.g. If the player dies).
        /// </summary>
        [SerializeField]
        private string currentSpawn;

        /// <summary>
        /// The name of the current unity scene the player is in.
        /// </summary>
        [SerializeField]
        private string currentScene;



        protected override void Awake() {
            if (preTransitionEvents == null) {
                preTransitionEvents = new UnityEvent();
            }

            if (postTransitionEvents == null) {
                postTransitionEvents = new UnityEvent();
            }
            
            base.Awake();
        }

        /// <summary>
        /// Sets the current spawn for the player.
        /// </summary>
        /// <param name="spawnName">The name of the SpawnPoint (In the unity level editor hierarchy).</param>
        public void SetCurrentSpawn(string spawnName) {
            Debug.Log(spawnName);
            currentSpawn = spawnName;
        }

        /// <summary>
        /// Returns the name of the current spawn.
        /// </summary>
        /// <returns></returns>
        public string GetCurrentSpawnName() {
            return currentSpawn;
        }

        /// <summary>
        /// Sets the current scene of the game.
        /// </summary>
        /// <param name="sceneName">The name of the scene (path not needed, just the name of the scene itself).</param>
        public void SetCurrentScene(string sceneName)
        {
            currentScene = sceneName;
        }

        /// <summary>
        /// Returns the name of the current scene.
        /// </summary>
        /// <returns></returns>
        public string GetCurrentScene() {
            return currentScene;
        }

        /// <summary>
        /// Registers the location of a SpawnPoint with the transition manager.
        /// </summary>
        /// <param name="name">The name of the spawn (in editor)</param>
        /// <param name="pos">The spawn's position</param>
        /// <param name="right">Whether or not the player should be facing right or left upon respawn for this SpawnPoint. True = right, False = left</param>
        public void RegisterSpawn(string name, Vector3 pos, bool right)
        {
            if (name == null) return;
            if (!spawnPoints.ContainsKey(name))
            {
                spawnPoints.Add(name, pos);
                spawnLeftRight.Add(name, right);
            }
        }

        /// <summary>
        /// Clears the spawn point registry.
        /// </summary>
        public void ClearSpawnPoints()
        {
            spawnPoints.Clear();
            spawnLeftRight.Clear();
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
        public void MakeTransition(string scene, string spawn)
        {
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
            transitionAnim.SetBool("FadeToBlack", true);
            SetCurrentScene(scene);
            SetCurrentSpawn(spawn);

            TargettingCamera.SetTarget(vcamName);
        }

        /// <summary>
        /// Animation event callback. Called after the animation triggered in MakeTransition finishes.
        /// </summary>
        public void OnTransitionComplete() {
            transitionAnim.SetBool("FadeToBlack", false);
            SceneManager.LoadScene(currentScene);
            postTransitionEvents.Invoke();
            postTransitionEvents.RemoveAllListeners();
        }


        /// <summary>
        /// Reloads the current scene.
        /// </summary>
        public void ReloadScene() {
            MakeTransition(currentScene);
        }


        /// <summary>
        /// Get the position of the current spawn point for the player.
        /// </summary>
        public Vector3 GetCurrentSpawnPosition()
        {
            if (spawnPoints.ContainsKey(currentSpawn)) {
                return spawnPoints[currentSpawn];
            }

            throw new UnityException("Could not get current spawn location.");
        }


        /// <summary>
        /// Returns whether the player should be facing left or right when respawning at the current spawn.
        /// True = right, False = left.
        /// </summary>
        public bool GetCurrentSpawnFacing()
        {
            if (spawnLeftRight.ContainsKey(currentSpawn)) {
                return spawnLeftRight[currentSpawn];
            }

            throw new UnityException("Could not get current spawn facing information.");
        }
    }

}