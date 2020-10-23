
using Storm.Subsystems.Dialog;
using UnityEngine;
using UnityEngine.Events;
using XNode;

namespace Storm.Subsystems.Graph {

  /// <summary>
  /// A dialog node which ends a conversation, then performs a set of actions.
  /// </summary>
  [NodeWidth(400)]
  [NodeTint(NodeColors.END_NODE)]
  [CreateNodeMenu("Dialog/Terminal/End + Act Node")]
  public class EndingActionNode : AutoNode {

    #region Fields
    //---------------------------------------------------------------------
    // Fields
    //---------------------------------------------------------------------
    
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
    #endregion

    #region XNode API
    //---------------------------------------------------------------------
    // XNode API
    //---------------------------------------------------------------------
    
    /// <summary>
    /// Get the value of a port.
    /// </summary>
    /// <param name="port">The input/output port.</param>
    /// <returns>The value for the port.</returns>
    public override object GetValue(NodePort port) {
      return null;
    }
    #endregion
    
    #region Dialog Node API
    //---------------------------------------------------------------------
    // Dialog Node API
    //---------------------------------------------------------------------
    
    public override void Handle() {
      DialogManager.EndDialog();
      DialogManager.SetCurrentNode(null);
      
      if (Action.GetPersistentEventCount() > 0) {
        Action.Invoke();
      }
    }

    public override void PostHandle(GraphEngine graphEngine) {
      
    }
    #endregion
  }
}
