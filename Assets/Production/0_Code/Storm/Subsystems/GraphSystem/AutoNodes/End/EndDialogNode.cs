
using Storm.Subsystems.Dialog;
using XNode;

namespace Storm.Subsystems.Graph {

  /// <summary>
  /// A dialog node which ends a conversation.
  /// </summary>
  [NodeTint(NodeColors.END_NODE)]
  [CreateNodeMenu("End/End Dialog")]
  public class EndDialogNode : EndNode {

    #region Auto Node API
    //---------------------------------------------------
    // Auto Node API
    //---------------------------------------------------
    
    public override void Handle(GraphEngine graphEngine) {
      DialogManager.EndDialog();
      graphEngine.EndGraph();
    }

    public override void PostHandle(GraphEngine graphEngine) {
      
    }
    #endregion
  }
}
