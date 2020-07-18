using UnityEngine;

using XNode;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// The base class for Dialog Nodes. Defines the HandleNode() API.
  /// </summary>
  public abstract class BaseDialogNode : Node {

    public override object GetValue(NodePort port) {
      return null;
    }

    /// <summary>
    /// How to handle this node.
    /// </summary>
    public abstract void HandleNode();
  }
}