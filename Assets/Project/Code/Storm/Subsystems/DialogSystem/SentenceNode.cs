using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using XNode;

namespace Storm.Dialog {
  [NodeWidth(360)]
  [NodeTint("#4a6d8f")]
  [CreateNodeMenu("Dialog/Sentence Node")]
  public class SentenceNode : Node {

    [Input]
    public EmptyConnection input;

    [Space(8, order=0)]

    public string speaker;

    [Space(8, order=1)]
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