
using Storm.Subsystems.Dialog;
using XNode;

namespace Storm.Subsystems.Graph {

  /// <summary>
  /// A node which marks the end of a graph.
  /// </summary>
  /// <remarks>
  /// For graphs with a more specific context such as dialog, subclass from this node to
  /// define more specific graph-end behavior.
  /// </remarks>
  [NodeTint(NodeColors.END_NODE)]
  [CreateNodeMenu("End/End")]
  public class EndNode : AutoNode {

    #region Fields
    //---------------------------------------------------
    // Fields
    //---------------------------------------------------

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;
    #endregion

    #region Dialog Node API
    //---------------------------------------------------
    // Dialog Node API
    //---------------------------------------------------
    
    public override void Handle(GraphEngine graphEngine) {
      graphEngine.EndGraph();
    }

    public override void PostHandle(GraphEngine graphEngine) {
      
    }
    #endregion
  }
}
