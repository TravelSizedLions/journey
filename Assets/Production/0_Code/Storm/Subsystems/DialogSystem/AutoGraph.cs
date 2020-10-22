using System;
using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using UnityEngine;

using XNode;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// A graph that represents a conversation. 
  /// </summary>
  [CreateAssetMenu]
  public class AutoGraph : NodeGraph, IAutoGraph {

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


