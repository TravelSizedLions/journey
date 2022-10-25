
using UnityEngine;
using UnityEngine.Events;
using XNode;

namespace HumanBuilders.Graphing {
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [CreateNodeMenu("Note")]
  public class NoteNode : AutoNode {
    /// <summary>
    /// The text being spoken.
    /// </summary>
    [TextArea(5,10)]
    public string Text;
  }
}