
using UnityEngine;

namespace HumanBuilders {
  
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
        DialogManager.CloseDialogBox();
      }
    }
  }
}