using Storm.Subsystems.Graph;

namespace Storm.Subsystems.Dialog {
  
  /// <summary>
  /// A dialog node representing the start of a conversation.
  /// </summary>
  [NodeTint(NodeColors.START_COLOR)]
  [CreateNodeMenu("Start/Start Dialog")]
  public class StartDialogNode : StartNode { }
}