using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using XNode;

using Storm.Flexible;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// A dialog node for switching a conversation graph in the given scene. When
  /// this node is hit, the target dialog holder will have its active dialog
  /// switched with the given dialog. The target does not need to be the same
  /// object the node is sitting on.
  /// </summary>
  [NodeTint(NodeColors.DYNAMIC_COLOR)]
  [NodeWidth(400)]
  [CreateNodeMenu("Dialog/Dynamic/Switch Dialog Node")]
  public class SwitchDialogNode : DialogNode {
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]

    /// <summary>
    /// The person/object to switch up the dialog with.
    /// </summary>
    public Talkative Target;

    [Space(16, order=1)]

    /// <summary>
    /// The dialog graph in the scene that will be used in the conversation. Use
    /// this instead of the asset dialog if you need the graph to reference
    /// objects in the scene. This will be used instead of the asset graph if
    /// both are populated.
    /// </summary>
    [Tooltip("The dialog graph in the scene that will be used in the conversation.\n\nUse this instead of the asset dialog if you need the graph to reference objects in the scene. This will be used instead of the asset graph if both are populated.")]
    public SceneDialogGraph SceneDialog;

    /// <summary>
    /// The dialog graph asset that will be used in the conversation. This does
    /// not support referencing objects in the scene. If both dialogs are
    /// populated, the scene dialog graph will be used instead.
    /// </summary>
    [Tooltip("The dialog graph asset that will be used in the conversation.\n\nThis does not support referencing objects in the scene. If both dialogs are populated, the scene dialog graph will be used instead.")]
    public DialogGraph AssetDialog;


    [Space(8, order=2)]

    /// <summary>
    /// Output connection for the next node.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;


    public override void Handle() {
      if (AssetDialog != null && SceneDialog != null) {
        Debug.LogWarning("DialogSwitch object should only have one dialog graph attached. The scene dialog will be preferred over the asset dialog.");
      }


      if (Target != null) {
        if (SceneDialog != null) {
          Target.SceneDialog = SceneDialog;
        } else if (AssetDialog != null) {
          Target.AssetDialog = AssetDialog;
        } else {
          Debug.LogWarning("DialogSwitch object has no graphs attached to switch.");
        }
      } else {
        Debug.LogWarning("DialogSwitch object has no target!");
      }
    }

  }
}