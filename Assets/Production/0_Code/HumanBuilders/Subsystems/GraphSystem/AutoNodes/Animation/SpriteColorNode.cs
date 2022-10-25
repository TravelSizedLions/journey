using XNode;

using UnityEngine;
using Sirenix.OdinInspector;

namespace HumanBuilders.Graphing {

  /// <summary>
  /// A node for switching the animation on a controller in the scene.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("Animation/Sprite Color")]
  public class SpriteColorNode : AutoNode {
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
    /// The sprite.
    /// </summary>
    [Tooltip("The sprite renderer to change.")]
    [OnValueChanged("UpdateColor")]
    public SpriteRenderer Target;

    /// <summary>
    /// The color to change the sprite to.
    /// </summary>
    [Tooltip("The color to change the sprite to.")]
    public Color Color;

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
        GameManager.Player.Sprite.color = Color;
      } else {
        Target.color = Color;
      } 
    }
    #endregion


    
    #region Odin Inspector Stuff
    //-------------------------------------------------------------------------
    // Odin Inspector
    //-------------------------------------------------------------------------
    /// <summary>
    /// Update the color value to be whatever the target's current value is, for
    /// convenience.
    /// </summary>
    private void UpdateColor() {
      if (Target == null) {
        Color = GameManager.Player.Sprite.color;
      } else {
        Color = Target.color;
      }
    }
    #endregion
  }

}