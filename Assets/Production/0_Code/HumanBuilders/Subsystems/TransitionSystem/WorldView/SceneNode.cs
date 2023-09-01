using System.Collections;
using System.Collections.Generic;
using HumanBuilders;
using HumanBuilders.Graphing;
using UnityEngine;
using Sirenix.OdinInspector;
using HumanBuilders.Attributes;
using UnityEngine.GUID;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using TSL.Editor.SceneUtilities;
#endif


namespace TSL.Subsystems.WorldView {
  [NodeWidth(400)]
  public class SceneNode : AutoNode {
    public SceneField Scene {get => scene;}
    private SceneField scene;

    public string Path {get => path;}
    private string path;

    public string Key {get => guid;}
    private string guid;


    /// <summary>
    /// Maps each transition GameObject's GUID to its name
    /// </summary>
    [ReadOnly]
    [AutoTable(typeof(SceneComponentRef))]
    public List<SceneComponentRef> transitions = new List<SceneComponentRef>();

    /// <summary>
    /// Maps each spawnpoint GameObject's GUID to its name
    /// </summary>
    [ReadOnly]
    [AutoTable(typeof(SceneComponentRef))]
    public List<SceneComponentRef> spawnPoints = new List<SceneComponentRef>();

    // /// <summary>
    // /// A list of connections to other scenes
    // /// </summary>
    // [ReadOnly]
    // public List<SceneEdge> Connections; 

#if UNITY_EDITOR
    public void Construct(string scenePath) {
      guid = GUID.Generate().ToString();
      path = scenePath;
      scene = new SceneField();
      scene.SceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
      name = path.Split('/')[path.Split('/').Length-1]; 
    }

    [ContextMenu("Re-Analyze Scene")]
    public void Analyze() {
      if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
        EditorSceneManager.OpenScene(path);
        List<TransitionDoor> transitions = SceneUtils.FindAll<TransitionDoor>();
        transitions.ForEach(transition => AddTransition(transition));
        SceneUtils.FindAll<SpawnPoint>().ForEach(spawn => AddSpawnPoint(spawn));
      }
    }
#endif

    public void AddTransition(TransitionDoor transition) {
      SceneComponentRef entry = new SceneComponentRef();
      entry.GUID = transition.GetComponent<GuidComponent>().GetGuid();
      entry.Name = transition.name;
      if (transitions.IndexOf(entry) == -1) {
        transitions.Add(entry);
      }
    }

    public void AddSpawnPoint(SpawnPoint spawnPoint) {
      SceneComponentRef entry = new SceneComponentRef();
      entry.GUID = spawnPoint.GetComponent<GuidComponent>().GetGuid();
      entry.Name = spawnPoint.name;
      if (spawnPoints.IndexOf(entry) == -1) {
        spawnPoints.Add(entry);
      }
    }

    public void Clear() {
      transitions.Clear();
      spawnPoints.Clear();
    }
  }
}