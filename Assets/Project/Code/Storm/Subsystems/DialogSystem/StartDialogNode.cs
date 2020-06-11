
using XNode;

namespace Storm.Dialog {
  
  [NodeTint("#33a643")]
  [CreateNodeMenu("Dialog/Start Node")]
  public class StartDialogNode : Node {
    
    [Output]
    public EmptyConnection output;

    public override object GetValue(NodePort port) {
      return null;
    }
  }
}