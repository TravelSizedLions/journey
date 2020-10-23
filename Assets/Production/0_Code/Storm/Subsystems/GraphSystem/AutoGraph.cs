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

    private void Awake() {
      GuidComponent guid = GetComponent<GuidComponent>();
      if (guid != null) {
        GuidManager.Add(guid);
      } else {
        Debug.LogWarning("Dialog Graph \"" + name + "\" is missing a GuidComponent! Add one in the unity editor.");
      }
    }


    #region Public API
    //---------------------------------------------------
    // Public API
    //---------------------------------------------------

    /// <summary>
    /// Start the conversation.
    /// </summary>
    /// <returns>The first dialog node of the conversation.</returns>
    public IAutoNode FindStartingNode() {
      foreach (var node in graph.nodes) {
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


