using XNode;

using UnityEngine;

namespace Storm.Subsystems.Graph {

  /// <summary>
  /// A node for switching the animation on a controller in the scene.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("Animation/Snap To Position")]
  public class SnapToPositionNode : AutoNode {
    #region Fields
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
    #endregion

    #region Auto Node API
    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      // If the target is null, default is to assume it's for the player.
      if (Target == null) {
        GameManager.Player.Physics.Position = Destination.position;
      } else {
        Target.position = Destination.position;
      } 
    }
    #endregion
  }

}