using HumanBuilders.Graphing;
using UnityEngine;
using TSL.Extensions;

#if UNITY_EDITOR
using TSL.Editor.SceneUtilities;
using UnityEditor;
#endif

namespace TSL.Subsystems.WorldView {
  public class WorldViewGraph : AutoGraphAsset {


#if UNITY_EDITOR
    public void Rebuild() {
      Clear();
      SceneUtils.GetActiveScenesInBuild().ForEach(scenePath => CreateNode(scenePath));
      int rows = (nodes.Count / 10) + (nodes.Count % 10 == 0 ? 0 : 1);
      nodes.Each((node, i) => {
        int row = i / 10;
        int col = i % 10;
        node.position.x = 400*col;
        node.position.y = 400*row;
      });
    }


    public void CreateNode(string path) {
      SceneNode node = AddNode<SceneNode>();
      node.Construct(path);
      AssetDatabase.AddObjectToAsset(node, WorldViewSettings.GRAPH_PATH);
    }
#endif
  }
}