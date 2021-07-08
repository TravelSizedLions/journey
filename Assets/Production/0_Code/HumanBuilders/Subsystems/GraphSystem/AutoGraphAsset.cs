using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using XNode;

namespace HumanBuilders {

  /// <summary>
  /// A graph that represents a conversation. 
  /// </summary>
  [CreateAssetMenu]
  public class AutoGraphAsset : NodeGraph, IAutoGraph {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    public string Name { get { return name; } }

    /// <summary>
    /// The nodes in this graph.
    /// </summary>
    public List<IAutoNode> Nodes { get { return (autoNodes != null) ? autoNodes : RefreshNodeList(); } }

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
    public IAutoNode FindStartingNode() {
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
      foreach(Node node in autoNodes) {
        autoNodes.Add((IAutoNode)node);
      }
      return autoNodes;
    }
  }
}


