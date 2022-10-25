using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders.Graphing {
  [NodeWidth(NodeWidths.SHORT)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("Animation/Show or Hide Sprite")]
  public class ShowHideSpriteNode : AutoNode {
    //---------------------------------------------------
    // Fields
    //---------------------------------------------------
    [PropertyOrder(0)]
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [PropertyOrder(999)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    [Space(8)]
    [Tooltip("The sprite to flip")]
    public SpriteRenderer sprite;

    [Tooltip("Whether or not to show or hide this sprite.")]
    public bool Show;

    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (sprite != null) {
        sprite.enabled = Show;
      }
    }

    private void Reset() {
      name = "Show/Hide Sprite";
    }
  }
}