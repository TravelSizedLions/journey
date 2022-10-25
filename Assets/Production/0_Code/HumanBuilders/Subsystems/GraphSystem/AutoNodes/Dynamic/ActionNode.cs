
using UnityEngine;
using UnityEngine.Events;
using XNode;

namespace HumanBuilders.Graphing {

  /// <summary>
  /// A dialog node for performing a UnityEvent.
  /// </summary>
  [NodeTint(NodeColors.DYNAMIC_COLOR)]
  [NodeWidth(400)]
  [CreateNodeMenu("Dynamic/Unity Action")]
  public class ActionNode : AutoNode {

    #region Fields
    //---------------------------------------------------
    // Fields
    //---------------------------------------------------
    
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

   [Space(8, order=0)]

    /// <summary>
    /// The action to perform.
    /// </summary>
    [Tooltip("The action to perform.")]
    public UnityEvent Action;

    /// <summary>
    /// Output connection for the next node.
    /// </summary>
    [Space(8, order=1)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;
  
    #endregion
  
    #region XNode API
    //---------------------------------------------------
    // XNode API
    //---------------------------------------------------
    
    /// <summary>
    /// Get the value of a port.
    /// </summary>
    /// <param name="port">The input/output port.</param>
    /// <returns>The value for the port.</returns>
    public override object GetValue(NodePort port) {
      return null;
    }
    #endregion

    #region
    //---------------------------------------------------
    // Dialog Node API
    //---------------------------------------------------
    
    /// <summary>
    /// Invoke the events in the list.
    /// </summary>
    public override void Handle(GraphEngine graphEngine) {
      if (Action.GetPersistentEventCount() > 0) {
        Action.Invoke();
      }
    }

    #endregion
  }
}
