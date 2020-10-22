using XNode;

namespace Storm.Subsystems.Graph {

  public interface IAutoGraph {

    /// <summary>
    /// Start the conversation.
    /// </summary>
    /// <returns>The first dialog node of the conversation.</returns>
    /// <seealso cref="AutoGraph.FindStartingNode" />
    IAutoNode FindStartingNode();
  }
}


