using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace HumanBuilders {

  /// <summary>
  /// A node that ends a graph by loading a cutscene.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.END_NODE)]
  [CreateNodeMenu("Scenes/Reload Scene")]
  public class ReloadSceneNode : AutoNode {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous nodes(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Tooltip("The list of events to fire before the next scene loads.")]
    public UnityEvent OnSceneReload;


    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (OnSceneReload != null) {
        TransitionManager.AddTransitionEvents(OnSceneReload);
      }
      TransitionManager.ReloadScene();
    }

    public override void PostHandle(GraphEngine graphEngine) {
      graphEngine.EndGraph();
    }
  }
}