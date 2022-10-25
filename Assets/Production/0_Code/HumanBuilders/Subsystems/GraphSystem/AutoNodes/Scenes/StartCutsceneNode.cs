

using UnityEngine;
using UnityEngine.Events;

namespace HumanBuilders.Graphing {

  /// <summary>
  /// A node that ends a graph by loading a cutscene.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.END_NODE)]
  [CreateNodeMenu("Scenes/Load Cutscene")]
  public class StartCutsceneNode : AutoNode {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous nodes(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    /// <summary>
    /// The scene that will be loaded. This field is used for convenience in the Unity inspector.
    /// </summary>
    [SerializeField]
    [Tooltip("The scene that will be loaded.")]
    private SceneField scene = null;
    
    [Tooltip("The list of events to fire before the next scene loads.")]
    public UnityEvent OnSceneLoad;

    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (OnSceneLoad != null) {
        TransitionManager.AddTransitionEvents(OnSceneLoad);
      }

      TransitionManager.MakeTransition(scene.SceneName);
    }

    public override void PostHandle(GraphEngine graphEngine) {
      graphEngine.EndGraph();
    }

    public override bool IsNodeComplete() {
      return base.IsNodeComplete() && scene != null;
    }
  }
}