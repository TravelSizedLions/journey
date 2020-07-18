using UnityEngine;

using XNode;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// The base class for Dialog Nodes. Defines the HandleNode() API.
  /// </summary>
  public abstract class DialogNode : Node, IDialogNode {

    protected DialogManager manager;

    /// <summary>
    /// Injection point for the dialog manager.
    /// </summary>
    /// <param name="manager">The dialog manager to inject.</param>
    public void Inject(DialogManager manager) {
      this.manager = manager;
    }

    public override object GetValue(NodePort port) {
      return null;
    }

    /// <summary>
    /// How to handle this node.
    /// </summary>
    public virtual void HandleNode() {

    }

    /// <summary>
    /// Get the next node.
    /// </summary>
    /// <returns></returns>
    public virtual IDialogNode GetNextNode() {
      return (IDialogNode)GetOutputPort("Output").Connection.node;
    }
  }
}