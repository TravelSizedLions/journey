using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using UnityEngine;

using XNode;

namespace Storm.Dialog {

  /// <summary>
  /// A graph that represents a conversation.
  /// </summary>
  [CreateAssetMenu]
  public class DialogGraph : NodeGraph {

    /// <summary>
    /// Start the conversation.
    /// </summary>
    /// <returns>The first dialog node of the conversation.</returns>
    public Node StartDialog() {
      foreach (var node in nodes) {
        StartDialogNode root = node as StartDialogNode;
        if (root != null) {
          return root.GetOutputPort("output").Connection.node;
        }
      } 

      return null;
    }
  }

}


