using UnityEngine;
namespace HumanBuilders {

  [NodeWidth(300)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("Animation/Snap To Position")]
  public class SnapToPositionNode : AutoNode {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    /// <summary>
    /// The Game Object to move.
    /// </summary>
    [Tooltip("The Game Object to move.")]
    public Transform Target;

    /// <summary>
    /// The position to move the Game Object to.
    /// </summary>
    [Tooltip("The position to move the Game Object to.")]
    public Transform Destination;

    /// <summary>
    /// The output connection for this node.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      // If the target is null, default is to assume it's for the player.
      if (Target == null) {
        GameManager.Player.transform.position = Destination.position;
        GameManager.Player.transform.eulerAngles = Destination.eulerAngles;
        GameManager.Player.transform.localScale = Destination.localScale;
      } else {
        Target.position = Destination.position;
        Target.eulerAngles = Destination.eulerAngles;
        Target.localScale = Destination.localScale;
      } 
    }
  }
}