

using UnityEngine;
using UnityEngine.Events;
using XNode;

namespace HumanBuilders.Graphing {

  /// <summary>
  /// A dialog node which marks the end of a graph, then performs a set of actions.
  /// </summary>
  [NodeWidth(400)]
  [NodeTint(NodeColors.END_NODE)]
  [CreateNodeMenu("End/End + Act")]
  public class EndingActionNode : AutoNode {

    #region Fields
    //---------------------------------------------------------------------
    // Fields
    //---------------------------------------------------------------------
    
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

   [Space(8, order=0)]

    /// <summary>
    /// The action(s) to perform.
    /// </summary>
    [Tooltip("The action to perform.")]
    public UnityEvent Action;
    #endregion
    
    #region Dialog Node API
    //---------------------------------------------------------------------
    // Dialog Node API
    //---------------------------------------------------------------------
    
    public override void Handle(GraphEngine graphEngine) {
      graphEngine.EndGraph();
      
      if (Action.GetPersistentEventCount() > 0) {
        Action.Invoke();
      }
    }

    public override void PostHandle(GraphEngine graphEngine) {
      
    }
    #endregion
  }
}
