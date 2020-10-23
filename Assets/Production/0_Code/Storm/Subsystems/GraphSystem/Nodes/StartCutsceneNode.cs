using System.Collections;
using System.Collections.Generic;
using Storm.Subsystems.Dialog;
using Storm.Subsystems.Transitions;
using UnityEngine;

using XNode;

namespace Storm.Subsystems.Graph {

  /// <summary>
  /// A dialog node for ending the conversation by showing a cutscene.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.END_NODE)]
  [CreateNodeMenu("Dialog/Terminal/Cutscene Node")]
  public class StartCutsceneNode : AutoNode {

    /// <summary>
    /// Input connection from the previous nodes(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    /// <summary>
    /// The name of the cutscene to play.
    /// </summary>
    public string Cutscene;

    public override object GetValue(NodePort port) {
      return null;
    }
    
    public override void Handle() {
      TransitionManager.MakeTransition(Cutscene);
    }

    public override void PostHandle(GraphEngine graphEngine) {
      DialogManager.EndDialog();
      DialogManager.SetCurrentNode(null);
    }
  }
}