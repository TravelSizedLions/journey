using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace HumanBuilders {
  public class SlideShowPlanning : MonoBehaviour {
    
    public TextMeshProUGUI textDisplay;

    [Space(8)]
    [TableList]
    public List<SlideShowTextEntry> Sentences;

    [Space(10)]
    [Tooltip("The next scene to load after the cutscene has finished.")]
    public SceneField Scene;

    [Tooltip("The next spawn point to place the player at when the cutscene has finished.")]
    [ValueDropdown("GetSceneSpawnPoints")]
    public string NextSpawn;

    [PropertySpace(10)]
    [Tooltip("The event to fire before loading the next scene.")]
    public UnityEvent OnSlideshowFinished;

    private int currentSentence;

    private void Awake() {
      if (Sentences == null) {
        Sentences = new List<SlideShowTextEntry>();
      }

      if (Sentences.Count > 0) {
        textDisplay.text = Sentences[0].Text;
      }
    }

    private void Update() {
      if (PauseScreen.Paused) {
        return;
      }

      if (Input.GetButtonDown("Action") || Input.GetButtonDown("Jump")) {
        if (currentSentence != Sentences.Count - 1) {
          NextSentence();
        } else {
          ChangeScenes();
        }
      }
    }

    public void NextSentence() {
      currentSentence++;
      textDisplay.text = Sentences[currentSentence].Text;
    }

    /// <summary>
    /// Move on to the next scene.
    /// </summary>
    public void ChangeScenes() {
      if (OnSlideshowFinished != null) {
        OnSlideshowFinished.Invoke();
      }
      
      TransitionManager.MakeTransition(Scene.SceneName, NextSpawn);
    }

    #if UNITY_EDITOR
    /// <summary>
    /// Gets the list of possible spawn points in the destination scene.
    /// </summary>
    private IEnumerable<string> GetSceneSpawnPoints() => EditorUtils.GetSceneSpawnPoints(Scene);
    #endif
  }
}