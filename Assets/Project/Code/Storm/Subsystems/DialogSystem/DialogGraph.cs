using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using UnityEngine;

using XNode;

namespace Storm.Dialog {

  [CreateAssetMenu]
  public class DialogGraph : NodeGraph {

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


