#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using TSL.Editor.SceneUtilities;
using TSL.Extensions;

namespace TSL.SceneGraphSystem {

  [Serializable]
  public class SceneGraph : ScriptableObject {

    public List<SceneNode> Nodes = new List<SceneNode>();

    public void Construct() {
      SceneUtils.GetAllScenesInBuild().ForEach(scenePath => Nodes.Add(CreateNode(scenePath)));
      int rows = (Nodes.Count / 10) + (Nodes.Count % 10 == 0 ? 0 : 1);
      Nodes.Each((node, i) => {
        int row = i / 10;
        int col = i % 10;
        node.Position.x = 400*col;
        node.Position.y = 200*row;
      });
    }

    public SceneNode CreateNode(string path) {
      SceneNode node = ScriptableObject.CreateInstance<SceneNode>();
      node.Construct(path);
      AssetDatabase.AddObjectToAsset(node, this);
      return node;
    }

    public void DeleteNode(SceneNode node) {
      Nodes.Remove(node);
    }
  }
}
#endif