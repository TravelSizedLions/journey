


using System;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace HumanBuilders.Graphing {
  /// <summary>
  /// A dialog node representing a single screen of text without a speaker.
  /// </summary>
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [Obsolete("Use DialogNode instead.")]
  [CreateNodeMenu("")]
  public class TextNode : AutoNode {
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

    [Space(8)]

    /// <summary>
    /// How long to wait before advancing automatically.
    /// </summary>
    [ShowIf("AutoAdvance")]
    public float Delay;

    /// <summary>
    /// Output connection for the next node.
    /// </summary>
    [Space(8, order=1)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (!DialogManager.IsDialogBoxOpen()) {
        DialogManager.OpenDialogBox();
      }
      
      DialogManager.Type(Text, "", AutoAdvance, Delay);
    }

    public override void PostHandle(GraphEngine graphEngine) {}
  }
}