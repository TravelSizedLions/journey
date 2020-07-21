
using XNode;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// A dialog node which ends a conversation.
  /// </summary>
  [NodeTint("#a63333")]
  [CreateNodeMenu("Dialog/Terminal/End Node")]
  public class EndDialogNode : DialogNode {

    #region Fields
    //---------------------------------------------------
    // Fields
    //---------------------------------------------------

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;
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

    #region Dialog Node API
    //---------------------------------------------------
    // Dialog Node API
    //---------------------------------------------------
    
    public override void HandleNode() {
      if (manager == null) {
        manager = DialogManager.Instance;
      }
      
      manager.EndDialog();
      manager.SetCurrentNode(null);
    }
    #endregion
  }
}
