using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {

  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [NodeWidth(NodeWidths.SHORT)]
  [CreateNodeMenu("Companion/Set Companion Position")]
  public class SetCompanionPositionNode : AutoNode {

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

    // -------------------------------------------------------------------------
    // Fields
    // -------------------------------------------------------------------------
    [Space(8)]
    [Tooltip("The place to move the companion to.")]
    public Transform target = null;

    // -------------------------------------------------------------------------
    // Auto Node API
    // -------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (target != null && GameManager.Companion != null) {
        GameManager.Companion.transform.position = target.position;
      } else {
        Debug.LogWarning("SetCompanionPosition Node in graph \"" +  graphEngine.GetCurrentGraph().GraphName + "\" is missing a target position. Go into the AutoGraph editor for this graph and find the node with the missing target.");
      }
    }
  }
}