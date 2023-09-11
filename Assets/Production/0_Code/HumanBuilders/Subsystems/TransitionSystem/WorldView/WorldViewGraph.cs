using System.Collections.Generic;
using System.Linq;
using HumanBuilders.Graphing;
using TSL.Editor.SceneUtilities;
using TSL.Extensions;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XNode;
#endif

namespace TSL.Subsystems.WorldView {
  public class WorldViewGraph : NodeGraph {

    public ProjectSceneData Scenes;

    private bool callbacksEnabled = true;

    public bool CallbacksEnabled => callbacksEnabled;

    public SceneNode this [string name] => (SceneNode)nodes.Find(n => n.name == name);


#if UNITY_EDITOR
    public void Rebuild() {
      DisableCallbacks();
      Undo.RecordObject(this, "Rebuild world view");
      Clear();
      
      Scenes = new ProjectSceneData();
      Scenes.Construct();
      Scenes.Scenes.ForEach(data => CreateNode(data));
      SpreadNewNodes(nodes);
      nodes.ForEach(node => ((SceneNode)node).RebuildTransitions());
      EnableCallbacks();

      AssetDatabase.SaveAssets();
    }

    public SceneNode CreateNode(SceneData data) {
      SceneNode node = AddNode<SceneNode>();
      node.Construct(data);
      AssetDatabase.AddObjectToAsset(node, WorldViewSettings.GRAPH_PATH);
      return node;
    }

    public bool FullSync() {
      // Can't one-line this due to short circuit.
      bool changed = SyncScenes(false);
      changed = SyncConnections() || changed;
      return changed;
    }

    public bool SyncConnections() {
      bool changed = false;
      Undo.RecordObjects(nodes.ToArray(), "Sync world view connections");
      nodes.ForEach(node => {
        SceneNode sceneNode = (SceneNode)node;
        changed = sceneNode.Sync() || changed;
      });

      return changed;
    }

    public bool SyncScenes(bool rebuildTransitions = true) {
      Undo.RecordObject(this, "World view sync scenes");
      if (Scenes.Sync()) {
        RemoveUnusedNodes();
        List<Node> newNodes = AddNewNodes();
        if (rebuildTransitions) {
          newNodes.ForEach(n => ((SceneNode)n).RebuildTransitions());
        }
        
        return true;
      }

      return false;
    }
#endif

    public bool Contains(SceneData sceneInfo) => nodes.Find(node => node.name == sceneInfo.Name) != null;
    
    public void RemoveUnusedNodes() {
      DisableCallbacks();
      List<SceneNode> list = nodes.ConvertAll(n => (SceneNode)n);
      list.ForEach(node => {
        if (!Scenes.Paths.Contains(node.Path)) {
          RemoveNode(node);
        }
      });
      EnableCallbacks();
    }

    public List<Node> AddNewNodes() {
      List<Node> newNodes = new List<Node>();
      Scenes.Scenes.ForEach(sceneInfo => {
        if (nodes.Find(node => ((SceneNode)node).Path == sceneInfo.Path) == null) {
          newNodes.Add(CreateNode(sceneInfo));
        }
      });

      SpreadNewNodes(newNodes);
      return newNodes;
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

    public void DisableCallbacks() {
      callbacksEnabled = false;
    }

    public void EnableCallbacks() {
      callbacksEnabled = true;
    }
  }
}