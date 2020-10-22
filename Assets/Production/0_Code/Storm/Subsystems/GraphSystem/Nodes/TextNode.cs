
using Storm.Subsystems.Dialog;
using UnityEngine;
using XNode;

namespace Storm.Subsystems.Graph {
  /// <summary>
  /// A dialog node representing a single screen of text without a speaker.
  /// </summary>
  [NodeWidth(360)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [CreateNodeMenu("Dialog/Basic/Text Node")]
  public class TextNode : AutoNode {

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;
    
    [Space(8, order=0)]
    [TextArea(3,10)]

    /// <summary>
    /// The text to display.
    /// </summary>
    public string Text;

    /// <summary>
    /// Output connection for the next node.
    /// </summary>
    [Space(8, order=1)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    /// <summary>
    /// Get the value of a port.
    /// </summary>
    /// <param name="port">The input/output port.</param>
    /// <returns>The value for the port.</returns>
    public override object GetValue(NodePort port) {
      return null;
    }


    public override void Handle() {
      DialogManager.Type(Text);
    }

    public override void PostHandle(GraphEngine graphEngine) {
      
    }
  }
}