#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TSL.SceneGraphSystem {
  public class SceneNode : ScriptableObject {
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
    public Dictionary<GUID, string> Transitions = new Dictionary<GUID, string>();

    /// <summary>
    /// Maps each spawnpoint GameObject's GUID to its name
    /// </summary>
    public Dictionary<GUID, string> SpawnPoints = new Dictionary<GUID, string>(); 

    /// <summary>
    /// A list of connections to other scenes
    /// </summary>
    [ReadOnly]
    public List<SceneEdge> Connections; 

    public string Key {get => guid;}
    private string guid;

    private List<string> spawns = new List<string>();

    private List<string> transition = new List<string>();

    public void Construct(string scenePath) {
      guid = GUID.Generate().ToString();
      Position = new Vector2();
      path = scenePath;
      scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(Path);
      name = Path.Split('/')[Path.Split('/').Length-1]; 
    }
  }
}
#endif