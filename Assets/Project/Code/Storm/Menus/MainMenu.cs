using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Storm.TransitionSystem;

namespace Storm.Menus {
    public class MainMenu : MonoBehaviour {
        public void PlayGame() {
            TransitionManager.Instance.MakeTransition("Cutscene");
        }

        public void QuitGame() {
            Debug.Log("Quitting!");
            Application.Quit();
        }
    }
}
