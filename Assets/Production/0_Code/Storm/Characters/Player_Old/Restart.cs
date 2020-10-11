using System.Collections;
using System.Collections.Generic;
using Storm;
using Storm.Subsystems.Transitions;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Temporary class meant for Demo purposes only. Restarts the game at the given scene
/// and spawn point.
/// </summary>
public class Restart : MonoBehaviour {
  void Update() {
    if (Input.GetKeyDown(KeyCode.R)) {
      TransitionManager.MakeTransition("VerticalSlice", "Start");
    }
  }
}