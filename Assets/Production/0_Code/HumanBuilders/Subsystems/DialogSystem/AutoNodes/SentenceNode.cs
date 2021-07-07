using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// A node representing a single screen of text with a speaker.
  /// </summary>
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [CreateNodeMenu("Dialog/Sentence (with speaker)")]
  public class SentenceNode : AutoNode {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]

    /// <summary>
    /// The person saying the sentence.
    /// </summary>
    public string Speaker;

    [Space(8, order=1)]

    /// <summary>
    /// The text being spoken.
    /// </summary>
    [TextArea(3,10)]
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

    [Space(8, order=1)]

    /// <summary>
    /// Output connection for the next node.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    public override void Handle(GraphEngine graphEngine) {
      if (!DialogManager.IsDialogBoxOpen()) {
        DialogManager.OpenDialogBox();
      }
      
      DialogManager.Type(Text, Speaker, AutoAdvance, Delay);
    }

    public override void PostHandle(GraphEngine graphEngine) {}
  }
}