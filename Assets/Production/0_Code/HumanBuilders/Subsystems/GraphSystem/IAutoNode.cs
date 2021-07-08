
using System.Collections.Generic;

namespace HumanBuilders {

  /// <summary>
  /// Used to indicate which direction ports should be connected. 
  /// - Forward : Source Output -> Destination Input
  /// - Backward : Source Input -> Destination Output
  /// </summary>
  public enum ConnectionDirection { Forward, Backward }

  /// <summary>
  /// Interface for the AutoNode "Template Method" class. Defines the HandleNode() and GetNextNode API.
  /// </summary>
  /// <seealso cref="AutoNode" />
  /// <remarks>
  /// Template Method Pattern: https://sourcemaking.com/design_patterns/template_method
  /// </remarks>
  public interface IAutoNode {

    /// <summary>
    /// Check any transition conditions registered on this node. 
    /// </summary>
    /// <returns>True if any condition was met. False otherwise.</returns>
    /// <seealso cref="AutoNode.CheckConditions" />
    bool CheckConditions();

    /// <summary>
    /// Template method, called by the DialogManager to handle the current node.
    /// </summary>
    /// <param name="graphEngine">
    /// The graph engine that called this into this node.
    /// </param>
    /// <seealso cref="DialogManager.ContinueDialog" />
    /// <seealso cref="AutoNode.HandleNode" />
    void HandleNode(GraphEngine graphEngine);

    /// <summary>
    /// Hook method: How the dialog manager should handle this node.
    /// </summary>
    /// <param name="graphEngine">The graph traversal engine that called this node</param>
    /// <seealso cref="AutoNode.Handle" />
    void Handle(GraphEngine graphEngine);

    /// <summary>
    /// Hook method: What to do after the node has been handled.
    /// </summary>
    /// <param name="graphEngine">
    /// The graph engine that called this into this node.
    /// </param>
    /// <seealso cref="AutoNode.PostHandle" />
    void PostHandle(GraphEngine graphEngine);

    /// <summary>
    /// Get the next node in the graph.
    /// </summary>
    /// <returns>The next node.</returns>
    /// <seealso cref="AutoNode.GetNextNode" />
    IAutoNode GetNextNode();

    /// <summary>
    /// Add a list of conditional transitions to this node. These conditions
    /// will be checked each frame.
    /// </summary>
    /// <param name="conditions">The list of conditions to register.</param>
    /// <param name="outputPort">The name of the output port these conditions
    /// <seealso cref="AutoNode.RegisterConditions" />
    void RegisterConditions(List<Condition> conditions, string portName);

    /// <summary>
    /// Connect this node to another node.
    /// </summary>
    /// <param name="dstNode">The node to connect to.</param>
    /// <param name="srcPort">The name of the port on this node to create a
    /// connection from. </param>
    /// <param name="dstPort">The name of the port on the destination node to
    /// create a connection to.</param>
    /// <param name="direction">Whether the connection is going from output
    /// (source) to input (dest), or from input (source) to output (dest).</param>
    /// <seealso cref="AutoNode.ConnectTo" />
    void ConnectTo(AutoNode dstNode, string srcPort, string dstPort, ConnectionDirection direction);
  }
}