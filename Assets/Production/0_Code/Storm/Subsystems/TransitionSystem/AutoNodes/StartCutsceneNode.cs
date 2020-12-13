using Snippets;
using Storm.Subsystems.Dialog;
using Storm.Subsystems.Graph;
using UnityEngine;

namespace Storm.Subsystems.Transitions {

  /// <summary>
  /// A node that ends a graph by loading a cutscene.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.END_NODE)]
  [CreateNodeMenu("Scenes/Load Cutscene")]
  public class StartCutsceneNode : AutoNode {
    #region Fields
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
    #endregion

    #region Auto Node API
    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      TransitionManager.MakeTransition(scene.SceneName);
    }

    public override void PostHandle(GraphEngine graphEngine) {
      graphEngine.EndGraph();
    }
    #endregion
  }
}