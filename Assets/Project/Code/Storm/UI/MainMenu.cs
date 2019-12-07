using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Storm.TransitionSystem;

namespace Storm.UI {
    public class MainMenu : MonoBehaviour {

        /// <summary>
        /// The name of the scene to load.
        /// </summary>
        [Tooltip("The name of the scene to load.")]
        public string SceneName;

        /// <summary>
        /// The name of the spawn position for the player.
        /// </summary>
        [Tooltip("The name of the spawn position for the player.")]
        public string SpawnName;


        /// <summary>
        /// Start playing the game.
        /// </summary>
        public void PlayGame() {
            TransitionManager.Instance.MakeTransition(SceneName, SpawnName);
        }

        /// <summary>
        /// Quit playing the game.
        /// </summary>
        public void QuitGame() {
            Debug.Log("Quitting!");
            Application.Quit();
        }
    }
}
