using UnityEngine;

using XNode;

namespace Storm.Subsystems.Dialog {

  public interface IDialogNode {
    /// <summary>
    /// How the dialog manager should handle this node.
    /// </summary>
    /// <seealso cref="DialogManager.ContinueDialog" />
    /// <seealso cref="DialogNode.HandleNode" />
    /// <seealso cref="SentenceNode.HandleNode" />
    /// <seealso cref="TextNode.HandleNode" />
    void HandleNode();

    /// <summary>
    /// Get the next node in the graph.
    /// </summary>
    /// <returns>The next node.</returns>
    IDialogNode GetNextNode();
  }
}