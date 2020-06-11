
using UnityEngine;
using XNode;

namespace Storm.Dialog {
  [NodeWidth(360)]
  [NodeTint("#4a6d8f")]
  [CreateNodeMenu("Dialog/Text Node")]
  public class TextNode : Node {
    [Input]
    public EmptyConnection input;
    
    [Space(8, order=0)]
    [TextArea(3,10)]
    public string text;

    [Space(8, order=1)]
    [Output]
    public EmptyConnection output;

    public override object GetValue(NodePort port) {
      return null;
    }
  }
}