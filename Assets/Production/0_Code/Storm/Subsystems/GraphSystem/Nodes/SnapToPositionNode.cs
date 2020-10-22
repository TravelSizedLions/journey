using XNode;

using UnityEngine;

namespace Storm.Subsystems.Graph {

  /// <summary>
  /// A dialog node for switching the animation on a controller in the scene.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("Dialog/Animation/Snap To Position")]
  public class SnapToPositionNode : AutoNode {
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

    public override void Handle() {
      Target.position = Destination.position;
    }
  }

}