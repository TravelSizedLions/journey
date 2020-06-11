
using XNode;

namespace Storm.Dialog {

  [NodeTint("#a63333")]
  [CreateNodeMenu("Dialog/End Node")]
  public class EndDialogNode : Node {

    [Input]
    public EmptyConnection input;


    public override object GetValue(NodePort port) {
      return null;
    }

  }
}