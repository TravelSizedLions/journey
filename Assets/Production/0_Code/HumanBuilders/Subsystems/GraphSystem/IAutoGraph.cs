using System.Collections.Generic;
using XNode;

namespace HumanBuilders {

  /// <summary>
  /// The interface that defines AutoGraphs.
  /// </summary>
  public interface IAutoGraph {

    /// <summary>
    /// The name of the graph.
    /// </summary>
    string GraphName { get; }

    /// <summary>
    /// The nodes in this graph.
    /// </summary>
    /// <seealso cref="AutoGraph.AutoNodes" />
    List<IAutoNode> AutoNodes { get; }

    /// <summary>
    /// Start the conversation.
    /// </summary>
    /// <returns>The first dialog node of the conversation.</returns>
    /// <seealso cref="AutoGraphAsset.FindStartingNode" />
    /// <seealso cref="AutoGraph.FindStartingNode" />
    IAutoNode FindStartingNode();
  }
}


