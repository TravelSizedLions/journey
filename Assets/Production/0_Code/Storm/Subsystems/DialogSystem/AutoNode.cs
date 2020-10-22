using System.Collections;
using Storm.Characters.Player;
using UnityEngine;

using XNode;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// The base class for Graph Nodes. Follows the "Template Method" pattern.
  /// </summary>
  /// <remarks>
  /// Template Method Pattern: https://sourcemaking.com/design_patterns/template_method
  /// </remarks>
  public abstract class AutoNode : Node, IAutoNode {

    #region Fields
    //---------------------------------------------------------------------
    // Fields
    //---------------------------------------------------------------------
    
    /// <summary>
    /// A reference to the Player.
    /// </summary>
    protected static PlayerCharacter player;
    #endregion

    #region XNode API
    //---------------------------------------------------------------------
    // XNode API
    //---------------------------------------------------------------------
    
    public override object GetValue(NodePort port) {
      return null;
    }

    #endregion
      
    #region Dialog Node API
    //---------------------------------------------------------------------
    // Dialog Node API
    //---------------------------------------------------------------------
      
    /// <summary>
    /// Handle this node.
    /// </summary>
    /// <remarks>
    /// This is a template method (see
    /// https://sourcemaking.com/design_patterns/template_method). Sub-class
    /// from this class and override Handle() and PostHandle() to create your
    /// own custom behavior.
    /// </remarks>
    public void HandleNode() {

      if (player == null) {
        player = GameManager.Player;
      }

      if (DialogManager.StartHandlingNode()) {

        // Hook method. Implement this in a sub-class.
        Handle();

        /** 
         * Some nodes spin off coroutines in their Handle() method. 
         * 
         * When this is the case, it's possible for the node to lock "finishing" the node
         * until the coroutine is done. In this case, we spin up a second
         * coroutine to wait until the node truly is finished.
        */
        if (DialogManager.FinishHandlingNode()) {

          // Hook method. Implement this in a sub-class.
          PostHandle();
        } else {
          DialogManager.StartThread(_WaitUntilFinished());
        }
      }
    }

    /// <summary>
    /// How to handle this node.
    /// </summary>
    /// <remarks>
    /// This is a hook method. Override this in a sub-class of DialogNode in
    /// order to write the actual behavior of the node. 
    /// </remarks>
    public virtual void Handle() {

    }

    /// <summary>
    /// What to do after handling this node.
    /// </summary>
    /// <remarks>
    /// Usually, this will either be "go to the next node in the graph" or 
    /// "do nothing (and wait for the next player input)." The default behavior
    /// handles the first case.
    /// 
    /// This is a hook method. Override this in a sub-class of DialogNode in
    /// order to write the actual behavior of the node. 
    /// </remarks>
    public virtual void PostHandle() {
      IAutoNode node = GetNextNode();
      DialogManager.SetCurrentNode(node);
      DialogManager.ContinueDialog();
    }

    /// <summary>
    /// Get the next node in the dialog graph.
    /// </summary>
    /// <returns>The next node in the dialog graph.</returns>
    public virtual IAutoNode GetNextNode() {
      return (IAutoNode)GetOutputPort("Output").Connection.node;
    }

    /// <summary>
    /// Waits to call the PostHandle hook until after the node has actually
    /// finished handling itself.
    /// </summary>
    /// <returns></returns>
    private IEnumerator _WaitUntilFinished() {
      while(!DialogManager.FinishHandlingNode()) {
        yield return null;
      }

      PostHandle();
    }
    #endregion
  }
}
