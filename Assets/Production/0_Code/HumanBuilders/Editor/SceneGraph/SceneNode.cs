using UnityEditor;
using UnityEngine;

namespace TSL.SceneGraphSystem {
  public class SceneNode : ScriptableObject {
    public SceneAsset scene;
    public Vector2 position;
    public string GUID;
  }
}