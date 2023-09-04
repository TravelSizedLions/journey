using HumanBuilders.Graphing;
using TSL.Extensions;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace TSL.Subsystems.WorldView {
  public class WorldViewGraph : AutoGraphAsset {

    public ProjectSceneData Scenes;

    public SceneNode this [string name] {
      get {
        foreach (var node in AutoNodes) {
          Debug.Log(node.NodeName);
          if (node.NodeName == name) {
            return node as SceneNode;
          }
        }
        return null;
      }
    }


#if UNITY_EDITOR
    public void Rebuild() {
      Clear();
      Scenes = new ProjectSceneData();
      Scenes.Construct();
      Scenes.Scenes.ForEach(data => CreateNode(data));
      int rows = (nodes.Count / 10) + (nodes.Count % 10 == 0 ? 0 : 1);
      nodes.Each((node, i) => {
        int row = i / 10;
        int col = i % 10;
        node.position.x = 800 * col;
        node.position.y = 800 * row;
      });
    }


    public void CreateNode(SceneData data) {
      SceneNode node = AddNode<SceneNode>();
      node.Construct(data);
      AssetDatabase.AddObjectToAsset(node, WorldViewSettings.GRAPH_PATH);
    }
#endif
  }
}