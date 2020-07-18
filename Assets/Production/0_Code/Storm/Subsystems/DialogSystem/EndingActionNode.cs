
using UnityEngine;
using UnityEngine.Events;
using XNode;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// A dialog node which ends a conversation, then performs a set of actions.
  /// </summary>
  [NodeWidth(400)]
  [NodeTint("#a63333")]
  [CreateNodeMenu("Dialog/Terminal/End & Act Node")]
  public class EndingActionNode : DialogNode {

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

   [Space(8, order=0)]

    /// <summary>
    /// The action(s) to perform.
    /// </summary>
    [Tooltip("The action to perform.")]
    public UnityEvent Action;

    /// <summary>
    /// Get the value of a port.
    /// </summary>
    /// <param name="port">The input/output port.</param>
    /// <returns>The value for the port.</returns>
    public override object GetValue(NodePort port) {
      return null;
    }


    public override void HandleNode() {
      if (manager == null) {
        manager = DialogManager.Instance;
      }

      manager.EndDialog();
      manager.SetCurrentNode(null);
      
      if (Action.GetPersistentEventCount() > 0) {
        Action.Invoke();
      }
    }
  }
}