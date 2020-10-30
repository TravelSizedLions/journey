using System;
using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using UnityEngine;

using XNode;

namespace Storm.Subsystems.Graph {

  /// <summary>
  /// A graph that represents a conversation. 
  /// </summary>
  [CreateAssetMenu]
  public class AutoGraphAsset : NodeGraph, IAutoGraph {

    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The nodes in this graph.
    /// </summary>
    public List<IAutoNode> Nodes { get { return autoNodes; } }
    private List<IAutoNode> autoNodes;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      autoNodes = new List<IAutoNode>();
      foreach(Node node in autoNodes) {
        autoNodes.Add((IAutoNode)node);
      }
    }
    #endregion

    #region Public API
    //---------------------------------------------------
    // Public API
    //---------------------------------------------------

    /// <summary>
    /// Start the conversation.
    /// </summary>
    /// <returns>The first dialog node of the conversation.</returns>
    public IAutoNode FindStartingNode() {
      foreach (var node in nodes) {
        StartDialogNode root = node as StartDialogNode;
        if (root != null) {
          return root.GetNextNode();
        }
      } 

      return null;
    }

    #endregion
  }
}


