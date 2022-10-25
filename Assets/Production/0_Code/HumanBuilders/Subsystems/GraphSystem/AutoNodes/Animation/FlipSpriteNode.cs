using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders.Graphing {
  [NodeWidth(NodeWidths.SHORT)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("Animation/Flip Sprite")]
  public class FlipSpriteNode : AutoNode {
    //---------------------------------------------------
    // Fields
    //---------------------------------------------------

    [PropertyOrder(0)]
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8)]
    [Tooltip("The sprite to flip")]
    public SpriteRenderer sprite;

    [BoxGroup("Flip")]
    public bool X;

    [BoxGroup("Flip")]
    public bool Y;

    [PropertyOrder(999)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (sprite != null) {
        sprite.flipX = X;
        sprite.flipY = Y;
      }
    }
  }
}