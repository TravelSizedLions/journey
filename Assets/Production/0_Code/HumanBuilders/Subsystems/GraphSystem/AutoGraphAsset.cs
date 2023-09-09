using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XNode;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HumanBuilders.Graphing {

  /// <summary>
  /// A graph that represents a conversation. 
  /// </summary>
  [CreateAssetMenu(fileName = "New AutoGraph", menuName = "AutoGraph/AutoGraph")]
  public class AutoGraphAsset : NodeGraph, IAutoGraph {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    public string GraphName => name;

    /// <summary>
    /// The nodes in this graph.
    /// </summary>
    public List<IAutoNode> AutoNodes => (List<IAutoNode>)nodes.Cast<IAutoNode>();

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

#if UNITY_EDITOR
    public override void Clear() {
      nodes?.ForEach(n => {
        if (n != null && AssetDatabase.Contains(n)) {
          AssetDatabase.RemoveObjectFromAsset(n);
        }
      });
      base.Clear();
    }
#endif
  }
}