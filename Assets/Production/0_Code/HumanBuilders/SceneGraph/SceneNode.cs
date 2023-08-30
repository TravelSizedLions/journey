using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TSL.SceneGraphSystem {
  public class SceneNode : ScriptableObject {
    public string Name {get => name;}
    private string _name;
    public string Path {get => path;}
    private string path;
    public SceneAsset Scene {get => scene;}
    private SceneAsset scene;

    public Vector2 Position;

    public string Key {get => guid;}
    private string guid;

    private List<string> spawns = new List<string>();

    private List<string> transition = new List<string>();

    public void Construct(string scenePath) {
      guid = GUID.Generate().ToString();
      Position = new Vector2();
      path = scenePath;
      scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(Path);
      _name = Path.Split('/')[Path.Split('/').Length-1]; 
    }
  }
}