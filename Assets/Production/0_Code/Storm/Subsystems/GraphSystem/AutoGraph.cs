using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using UnityEngine;

using XNode;

namespace Storm.Subsystems.Graph {

  /// <summary>
  /// A version of the Dialog Graph that you can attach directly to a game object.
  /// </summary>
  [RequireComponent(typeof(GuidComponent))]
  public class AutoGraph : SceneGraph<AutoGraphAsset>, IAutoGraph {

    /// <summary>
    /// The nodes in this graph.
    /// </summary>
    public List<IAutoNode> Nodes { get { return nodes; } }
    private List<IAutoNode> nodes;

    private void Awake() {
      GuidComponent guid = GetComponent<GuidComponent>();
      if (guid != null) {
        GuidManager.Add(guid);
      } else {
        Debug.LogWarning("Dialog Graph \"" + name + "\" is missing a GuidComponent! Add one in the unity editor.");
      }

      nodes = new List<IAutoNode>();
      foreach(Node node in graph.nodes) {
        nodes.Add((IAutoNode)node);
      }
    }


    #region Public API
    //---------------------------------------------------
    // Public API
    //---------------------------------------------------

    /// <summary>
    /// Finds the first node of the graph.
    /// </summary>
    /// <returns>The first node of the graph.</returns>
    public IAutoNode FindStartingNode() {
      foreach (var node in graph.nodes) {
        StartDialogNode root = node as StartDialogNode;
        if (root != null) {
          return root;
        }
      } 

      return null;
    }

    #endregion
    
  }
}


