using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System;

namespace TSL.SceneGraphSystem {

  [Serializable]
  [CreateAssetMenu(fileName = "scene-graph", menuName = "Journey/Create Scene Graph")]
  public class SceneGraph : ScriptableObject {

    public SceneNode rootNode;

    public List<SceneNode> nodes = new List<SceneNode>();
    
    public SceneNode CreateNode(System.Type type) {
      SceneNode node = ScriptableObject.CreateInstance(type) as SceneNode;
      node.name = type.Name;
      node.GUID = GUID.Generate().ToString();
      nodes.Add(node);

      AssetDatabase.AddObjectToAsset(node, this);
      AssetDatabase.SaveAssets();
      return node;
    }

    public void DeleteNode(SceneNode node) {
      nodes.Remove(node);
    }
  }
}
