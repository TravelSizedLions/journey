
using UnityEngine;

using XNode;

namespace Storm.Subsystems.Dialog {

  public interface IAutoNode {


    /// <summary>
    /// Template method, called by the DialogManager to handle the current node.
    /// </summary>
    /// <seealso cref="DialogManager.ContinueDialog" />
    /// <seealso cref="DialogNode.HandleNode" />
    void HandleNode();

    /// <summary>
    /// Hook method: How the dialog manager should handle this node.
    /// </summary>
    void Handle();

    /// <summary>
    /// Hook method: What to do after the node has been handled.
    /// </summary>
    void PostHandle();

    /// <summary>
    /// Get the next node in the graph.
    /// </summary>
    /// <returns>The next node.</returns>
    IAutoNode GetNextNode();
  }
}