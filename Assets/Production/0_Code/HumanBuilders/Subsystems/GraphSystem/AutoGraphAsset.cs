using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

using XNode;

namespace HumanBuilders.Graphing {

  /// <summary>
  /// A graph that represents a conversation. 
  /// </summary>
  [CreateAssetMenu(fileName="New AutoGraph", menuName="AutoGraph/AutoGraph")]
  public class AutoGraphAsset : NodeGraph, IAutoGraph {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    public string GraphName { get { return name; } }

    /// <summary>
    /// The nodes in this graph.
    /// </summary>
    public List<IAutoNode> AutoNodes { get { return (autoNodes != null) ? autoNodes : RefreshNodeList(); } }

    /// <summary>
    /// The nodes in this graph.
    /// </summary>
    private List<IAutoNode> autoNodes;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      autoNodes = new List<IAutoNode>();
      foreach(Node node in autoNodes) {
        autoNodes.Add((IAutoNode)node);
      }
    }

    //---------------------------------------------------
    // Public API
    //---------------------------------------------------

    /// <summary>
    /// Start the conversation.
    /// </summary>
    /// <returns>The first dialog node of the conversation.</returns>
    public virtual IAutoNode FindStartingNode() {
      foreach (var node in nodes) {
        StartNode root = node as StartNode;
        if (root != null) {
          return root;
        }
      } 

      return null;
    }

    private List<IAutoNode> RefreshNodeList() {
      autoNodes = new List<IAutoNode>();
      foreach(Node node in nodes) {
        autoNodes.Add((IAutoNode)node);
      }
      return autoNodes;
    }

    public virtual T FindNode<T>() where T : AutoNode {
      foreach (Node node in nodes) {
        if (node is T n) {
          return n;
        }
      }

      return null;
    }

    public virtual List<T> FindNodes<T>() where T : AutoNode {
      List<T> found = new List<T>();

      foreach (Node node in nodes) {
        if (node is T n) {
          found.Add(n);
        }
      }

      return found;
    }
  }
}


