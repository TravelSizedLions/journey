using XNode;

using UnityEngine;
using Sirenix.OdinInspector;

namespace HumanBuilders.Graphing {

  /// <summary>
  /// A node for switching the animation on a controller in the scene.
  /// </summary>
  [NodeWidth(NodeWidths.SHORT)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("Player/Set Player Facing")]
  public class SetFacingNode : AutoNode {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    [PropertyOrder(0)]
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    /// <summary>
    /// Which direction the player should face.
    /// </summary>
    [Tooltip("Which direction the player should face.")]
    public Facing Direction;

    [PropertyOrder(999)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;
    
    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      GameManager.Player.SetFacing(Direction);
    }

    private void Reset() {
      name = "Set Player Facing";
    }
  }

}