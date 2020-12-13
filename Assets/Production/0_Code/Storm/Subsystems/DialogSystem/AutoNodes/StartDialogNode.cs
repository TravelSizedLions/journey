using Storm.Subsystems.Graph;
using UnityEngine;

namespace Storm.Subsystems.Dialog {
  
  /// <summary>
  /// A dialog node representing the start of a conversation.
  /// </summary>
  [NodeTint(NodeColors.START_COLOR)]
  [CreateNodeMenu("Start/Start Dialog")]
  public class StartDialogNode : StartNode { 

    /// <summary>
    /// Close the dialog box.
    /// </summary>
    [Tooltip("Start with the dialog box closed.")]
    public bool StartClosed;
    
    public override void Handle(GraphEngine graphEngine) {
      if (StartClosed) {
        Debug.Log("Closing!");
        DialogManager.CloseDialogBox();
      }
    }
  }
}