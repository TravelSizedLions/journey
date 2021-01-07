
using Storm.Subsystems.Dialog;
using Storm.Subsystems.Graph;
using UnityEngine;
using XNode;

namespace Storm.Subsystems.Dialog {
  /// <summary>
  /// A dialog node representing a single screen of text without a speaker.
  /// </summary>
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [CreateNodeMenu("Dialog/Sentence (without speaker)")]
  public class TextNode : AutoNode {
    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
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

    [Space(8)]

    /// <summary>
    /// Whether or not to wait for the the player to advance the dialog.
    /// </summary>
    [Tooltip("Whether or not to wait for the the player to advance the dialog.")]
    public bool AutoAdvance;

    /// <summary>
    /// Output connection for the next node.
    /// </summary>
    [Space(8, order=1)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    #endregion


    #region Auto Node API
    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (!DialogManager.IsDialogBoxOpen()) {
        DialogManager.OpenDialogBox();
      }
      
      DialogManager.Type(Text, "", AutoAdvance);
    }

    public override void PostHandle(GraphEngine graphEngine) {

    }
    #endregion

  }
}