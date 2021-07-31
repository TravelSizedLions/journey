using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace HumanBuilders {

  /// <summary>
  /// A version of the Dialog Graph that you can attach directly to a game object.
  /// </summary>
  [RequireComponent(typeof(GuidComponent))]
  public class AutoGraph : SceneGraph<AutoGraphAsset>, IAutoGraph {
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    public string GraphName { get { return name; } }

    /// <summary>
    /// The nodes in this graph.
    /// </summary>
    public List<IAutoNode> AutoNodes { get { return (nodes == null) ? RefreshNodeList() : nodes; } }

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The nodes in this graph.
    /// </summary>
    private List<IAutoNode> nodes;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      GuidComponent guid = GetComponent<GuidComponent>();
      if (guid != null) {
        GuidManager.Add(guid);
      } else {
        Debug.LogWarning("Dialog Graph \"" + name + "\" is missing a GuidComponent! Add one in the unity editor.");
      }

      RefreshNodeList();
    }

    //---------------------------------------------------
    // Public API
    //---------------------------------------------------

    /// <summary>
    /// Finds the first node of the graph.
    /// </summary>
    /// <returns>The first node of the graph.</returns>
    public IAutoNode FindStartingNode() {
      foreach (var node in graph.nodes) {
        StartNode root = node as StartNode;
        if (root != null) {
          return root;
        }
      } 

      return null;
    }

    private List<IAutoNode> RefreshNodeList() {
      nodes = new List<IAutoNode>();

      if (graph != null) {
        foreach(Node node in graph.nodes) {
          IAutoNode inode = (IAutoNode)node;
          if (inode != null) {
            nodes.Add(inode);
          }
        }
      }

      return nodes;
    }

    public T FindNode<T>() where T : AutoNode => graph.FindNode<T>();

    public List<T> FindNodes<T>() where T : AutoNode => graph.FindNodes<T>();
  }
}


