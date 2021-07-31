using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using FJSON;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor.Callbacks;
using UnityEditor;
#endif

namespace HumanBuilders {

  public class ScenesMenu : MonoBehaviour {
    //-------------------------------------------------------------------------
    // Constants
    //-------------------------------------------------------------------------
    public const string MAP_PATH = "StaticData/scene_map.json";

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The prefab to use to display available scenes.
    /// </summary>
    [Tooltip("The prefab to use to display available scenes.")]
    public GameObject SceneMenuItemPrefab;

    /// <summary>
    /// The game object to place scene menu items into.
    /// </summary>
    [Tooltip("The game object to place scene menu items into.")]
    public Transform ScenesContainer;

    /// <summary>
    /// The scene and spawn data for the game.
    /// </summary>
    public static Dictionary<string, List<string>> MapData;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void OnAwake() {
      foreach (Transform child in ScenesContainer) {
        Destroy(child.gameObject);
      }
    }

    private void OnEnable() {
      if (MapData == null) {
        string filePath = Path.Combine(Application.persistentDataPath, MAP_PATH);
        
        // Editor: Just create new map data.
        // Player: Copy map data from working directory to persistent path.
        #if UNITY_EDITOR
        if (!File.Exists(filePath)) {
          GenerateMapData();
        }
        #else
        if (!File.Exists(filePath)) {
          Debug.Log("HumanBuilders: Migrating Map Data");

          string installPath = Path.Combine(Directory.GetCurrentDirectory(), MAP_PATH);
          new FileInfo(filePath).Directory?.Create();
          File.Copy(installPath, filePath, true);
        }
        #endif

        StreamReader file = new StreamReader(filePath);
        string json = file.ReadToEnd();
        MapData = JSON.ToObject<Dictionary<string, List<string>>>(json);
        file.Close();
      }

      foreach (string scene in MapData.Keys) {
        GameObject go = Instantiate(SceneMenuItemPrefab, ScenesContainer);
        SceneMenuItem menuItem = go.GetComponentInChildren<SceneMenuItem>(true);

        menuItem.SetScene(scene);
      }
    }

    private void OnDisable() {
      foreach (Transform child in ScenesContainer) {
        Destroy(child.gameObject);
      }
    }

    #if UNITY_EDITOR
    [MenuItem("Journey/Generate Map Data")]
    [Button("Generate Map Data")]
    public static void GenerateMapData() {

      Debug.Log("Generating scene map data...");
      Dictionary<string, List<string>> mapData = new Dictionary<string, List<string>>();
      
      foreach (EditorBuildSettingsScene settingScene in EditorBuildSettings.scenes) {
        if (!settingScene.enabled) {
          continue;
        }

        string scene = settingScene.path;
        if (scene.ToLower().Contains("test")) {
          continue;
        }

        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene);
        List<string> spawns = new List<string>();
        
        if (!Application.isPlaying) {
          Scene selectedScene = EditorSceneManager.GetSceneByName(sceneName);

          if (!selectedScene.IsValid()) {
            if (!string.IsNullOrEmpty(scene)) {
              Debug.Log(scene);
              Scene openedScene = EditorSceneManager.OpenScene(scene, OpenSceneMode.Additive);

              foreach (GameObject obj in openedScene.GetRootGameObjects()) {
                foreach (var spawn in obj.GetComponentsInChildren<SpawnPoint>(true)) {
                  spawns.Add(spawn.name);
                }
              }
              
              EditorSceneManager.CloseScene(openedScene, true);
            }
          } else {
            foreach (GameObject obj in selectedScene.GetRootGameObjects()) {
              foreach (var spawn in obj.GetComponentsInChildren<SpawnPoint>(true)) {
                spawns.Add(spawn.name);
              }
            }
          }
        }

        mapData.Add(sceneName, spawns);
      }

      Debug.Log("Finished: " + mapData.Keys.Count + " scenes processed.");
      string outJSON = JSON.ToNiceJSON(mapData);
      string filePath = Path.Combine(Application.persistentDataPath, MAP_PATH);

      // Create any necessary directories.
      new FileInfo(filePath).Directory?.Create();

      StreamWriter file = new StreamWriter(filePath, append: false);
      file.Write(outJSON);
      file.Flush();
      file.Close();
    }
    #endif
  }
}