
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders.Graphing {
  
  /// <summary>
  /// A dialog node for disabling player movement.
  /// </summary>
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [CreateNodeMenu("Player/Freeze")]
  public class FreezePlayerNode : AutoNode { 

    [Space(4, order=0)]
    [PropertyOrder(100)]
    [Input(connectionType=ConnectionType.Override)]
    public EmptyConnection Input;


    [Space(4, order=0)]
    [PropertyOrder(100)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;
    
    public override void Handle(GraphEngine graphEngine) {
      GameManager.Player.DisableCrouch(graphEngine);
      GameManager.Player.DisableJump(graphEngine);
      GameManager.Player.DisableMove(graphEngine);
    }
  }
}