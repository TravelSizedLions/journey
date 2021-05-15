
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  
  /// <summary>
  /// A dialog node for re-enabling player movement.
  /// </summary>
  [NodeTint(NodeColors.BASIC_COLOR)]
  [NodeWidth(400)]
  [CreateNodeMenu("Player/Unfreeze")]
  public class UnfreezePlayerNode : AutoNode { 


    [Space(4, order=0)]
    [PropertyOrder(100)]
    [Input(connectionType=ConnectionType.Override)]
    public EmptyConnection Input;


    [Space(4, order=0)]
    [PropertyOrder(100)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;
    
    public override void Handle(GraphEngine graphEngine) {
      GameManager.Player.EnableCrouch(graphEngine);
      GameManager.Player.EnableJump(graphEngine);
      GameManager.Player.EnableMove(graphEngine);
    }
  }
}