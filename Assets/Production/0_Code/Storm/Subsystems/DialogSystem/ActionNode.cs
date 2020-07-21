
using UnityEngine;
using UnityEngine.Events;
using XNode;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// A dialog node for performing a UnityEvent between spoken dialog.
  /// </summary>
  [NodeTint("#996e39")]
  [NodeWidth(400)]
  [CreateNodeMenu("Dialog/Dynamic/Action Node")]
  public class ActionNode : DialogNode {

    #region Fields
    //---------------------------------------------------
    // Fields
    //---------------------------------------------------
    
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

   [Space(8, order=0)]

    /// <summary>
    /// The action to perform.
    /// </summary>
    [Tooltip("The action to perform.")]
    public UnityEvent Action;

    /// <summary>
    /// Output connection for the next node.
    /// </summary>
    [Space(8, order=1)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;
  
     #endregion
  
    #region XNode API
    //---------------------------------------------------
    // XNode API
    //---------------------------------------------------
    
    /// <summary>
    /// Get the value of a port.
    /// </summary>
    /// <param name="port">The input/output port.</param>
    /// <returns>The value for the port.</returns>
    public override object GetValue(NodePort port) {
      return null;
    }
    #endregion

    #region
    //---------------------------------------------------
    // Dialog Node API
    //---------------------------------------------------
    
    /// <summary>
    /// Invoke the events in the list.
    /// </summary>
    public override void HandleNode() {
      if (Action.GetPersistentEventCount() > 0) {
        Action.Invoke();
      }

      if (manager == null) {
        manager = DialogManager.Instance;
      }

      manager.SetCurrentNode(GetNextNode());
      manager.ContinueDialog();
    }
    #endregion
  }
}
