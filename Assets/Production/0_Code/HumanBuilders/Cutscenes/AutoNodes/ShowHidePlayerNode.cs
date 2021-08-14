using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  [NodeWidth(NodeWidths.SHORT)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("Player/Show or Hide Player")]
  public class ShowHidePlayerNode : AutoNode {
    //---------------------------------------------------
    // Fields
    //---------------------------------------------------
    [PropertyOrder(0)]
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [PropertyOrder(999)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    [Tooltip("Whether or not to show or hide this sprite.")]
    public bool Show;

    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (GameManager.Player != null && GameManager.Player.Sprite != null) {
        GameManager.Player.Sprite.enabled = Show;
      }
    }

    private void Reset() {
      name = "Show/Hide Player";
    }
  }
}