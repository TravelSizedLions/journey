using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
namespace HumanBuilders {

  /// <summary>
  /// A node that ends a graph by loading a cutscene.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.END_NODE)]
  [CreateNodeMenu("Scenes/Load New Scene")]
  public class LoadNewSceneNode : AutoNode {
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

    /// <summary>
    /// The name of the spawn point the player will be set at.
    /// </summary>
    [SerializeField]
    [Tooltip("The name of the spawn point the player will be set at.")]
    [ValueDropdown("GetSceneSpawnPoints")]
    private string spawnPoint = "";

    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      TransitionManager.MakeTransition(scene.SceneName, spawnPoint);
    }

    public override void PostHandle(GraphEngine graphEngine) {
      graphEngine.EndGraph();
    }
  
    //-------------------------------------------------------------------------
    // Odin Inspector
    //-------------------------------------------------------------------------

    #if UNITY_EDITOR
    /// <summary>
    /// Gets the list of possible spawn points in the destination scene.
    /// </summary>
    private IEnumerable<string> GetSceneSpawnPoints() => EditorUtils.GetSceneSpawnPoints(scene);
    #endif
  }
}