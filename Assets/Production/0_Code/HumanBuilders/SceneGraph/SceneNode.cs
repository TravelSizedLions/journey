#if UNITY_EDITOR
using System.Collections.Generic;
using HumanBuilders;
using HumanBuilders.Graphing;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.GUID;
using static XNode.Node;

namespace TSL.SceneGraphSystem {
  public class SceneNode : SerializedScriptableObject {
    public string Name {get => name;}
    public string Path {get => path;}
    private string path;
    public SceneAsset Scene {get => scene;}
    private SceneAsset scene;

    [ReadOnly]
    public Vector2 Position;

    /// <summary>
    /// Maps each transition GameObject's GUID to its name
    /// </summary>
    [ReadOnly]
    public Dictionary<System.Guid, string> transitions = new Dictionary<System.Guid, string>();

    /// <summary>
    /// Maps each spawnpoint GameObject's GUID to its name
    /// </summary>
    [ReadOnly]
    public Dictionary<System.Guid, string> spawnPoints = new Dictionary<System.Guid, string>(); 

    /// <summary>
    /// A list of connections to other scenes
    /// </summary>
    [ReadOnly]
    public List<SceneEdge> Connections; 

    public string Key {get => guid;}
    private string guid;

    public void Construct(string scenePath) {
      guid = GUID.Generate().ToString();
      Position = new Vector2();
      path = scenePath;
      scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(Path);
      name = Path.Split('/')[Path.Split('/').Length-1]; 
    }

    public void AddTransition(TransitionDoor transition) {
      var guid = transition.GetComponent<GuidComponent>().GetGuid();
      transitions.TryAdd(guid, transition.name);
    }

    public void AddSpawnPoint(SpawnPoint spawn) {
      var guid = spawn.GetComponent<GuidComponent>().GetGuid();
      spawnPoints.TryAdd(guid, spawn.name);
    }
  }
}
#endif