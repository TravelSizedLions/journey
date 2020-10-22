using XNode;

namespace Storm.Subsystems.Dialog {

  public interface IDialog {

    /// <summary>
    /// Start the conversation.
    /// </summary>
    /// <returns>The first dialog node of the conversation.</returns>
    /// <seealso cref="DialogGraph.StartDialog" />
    IAutoNode StartDialog();
  }
}


