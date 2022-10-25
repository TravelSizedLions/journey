using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders.Graphing {

  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [NodeWidth(NodeWidths.SHORT)]
  [CreateNodeMenu("Player/Orient Player")]
  public class OrientPlayerNode : AutoNode {

    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    [PropertyOrder(0)]
    [Input(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8)]
    [PropertyOrder(999)]
    [Output(connectionType = ConnectionType.Override)]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    [Space(8)]
    [Tooltip("The place to move the player to.")]
    public Transform target = null;

    [Tooltip("Which direction the player should face.")]
    public Facing Direction;

    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (target != null) {
        GameManager.Player.Physics.Position = target.position;
        GameManager.Player.SetFacing(Direction);
      } else {
        Debug.LogWarning("SetPlayerPosition Node in graph \"" +  graphEngine.GetCurrentGraph().GraphName + "\" is missing a target position. Go into the AutoGraph editor for this graph and find the node with the missing target.");
      }
    }
  }
}