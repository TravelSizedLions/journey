using System.Collections.Generic;
using HumanBuilders.Graphing;
using TSL.Editor.SceneUtilities;
using TSL.Extensions;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XNode;
#endif

namespace TSL.Subsystems.WorldView {
  public class WorldViewGraph : AutoGraphAsset {

    public ProjectSceneData Scenes;

    public SceneNode this [string name] {
      get {
        foreach (var node in AutoNodes) {
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
      SpreadNewNodes(nodes);
      nodes.ForEach(node => ((SceneNode)node).RebuildTransitions());
      AssetDatabase.SaveAssets();
    }

    public SceneNode CreateNode(SceneData data) {
      SceneNode node = AddNode<SceneNode>();
      node.Construct(data);
      AssetDatabase.AddObjectToAsset(node, WorldViewSettings.GRAPH_PATH);
      return node;
    }

    public bool Sync() {
      bool changed = false;
      if (Scenes.Sync()) {
        RemoveUnusedNodes();
        AddNewNodes();
        changed = true;
      }

      AutoNodes.ForEach(node => {
        SceneNode sceneNode = (SceneNode)node;
        changed |= sceneNode.Sync();
      });

      return changed;
    }
#endif

    public bool Contains(SceneData sceneInfo) => AutoNodes.Find(node => node.NodeName == sceneInfo.Name) != null;
    
    public void RemoveUnusedNodes() {
      AutoNodes.ForEach(node => {
        SceneNode sceneNode = (SceneNode)node;
        if (!Scenes.Paths.Contains(sceneNode.Path)) {
          RemoveNode(sceneNode);
        }
      });
    }

    public void AddNewNodes() {
      List<Node> nodes = new List<Node>();
      Scenes.Scenes.ForEach(sceneInfo => {
        if (AutoNodes.Find(node => ((SceneNode)node).Path == sceneInfo.Path) == null) {
          nodes.Add(CreateNode(sceneInfo));
        }
      });

      SpreadNewNodes(nodes);
    }

    public void SpreadNewNodes(List<Node> nodeList) {
      int rows = (nodeList.Count / 10) + (nodeList.Count % 10 == 0 ? 0 : 1);
      nodeList.Each((node, i) => {
        int row = i / 10;
        int col = i % 10;
        node.position.x = 800 * col;
        node.position.y = 800 * row;
      });
    }
  }
}