
using System.Collections.Generic;
using UnityEngine;

using XNode;

namespace Storm.Subsystems.Graph {

  /// <summary>
  /// Interface for the AutoNode "Template Method" class. Defines the HandleNode() and GetNextNode API.
  /// </summary>
  /// <seealso cref="AutoNode" />
  /// <remarks>
  /// Template Method Pattern: https://sourcemaking.com/design_patterns/template_method
  /// </remarks>
  public interface IAutoNode {


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
    /// <seealso cref="AutoNode.Handle" />
    void Handle();

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
    /// <seealso cref="AutoNode.RegisterConditions" />
    void RegisterConditions(List<Condition> conditions);
  }
}