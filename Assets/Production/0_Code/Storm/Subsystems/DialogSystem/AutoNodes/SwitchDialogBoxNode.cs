using System.Collections.Generic;
using Sirenix.OdinInspector;
using Storm.Subsystems.Dialog;
using Storm.Subsystems.Graph;
using UnityEngine;
using UnityEngine.EventSystems;
using XNode;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// A node that switches which dialog box UI element is open.
  /// </summary>
  [NodeTint(NodeColors.DBOX_COLOR)]
  [NodeWidth(360)]
  [CreateNodeMenu("Dialog/Switch Dialog Box")]
  public class SwitchDialogBoxNode : AutoNode {
    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]

    /// <summary>
    /// The dialog box to switch to.
    /// </summary>
    [ValueDropdown("GetListOfDialogBoxes", DropdownWidth=400, DropdownTitle="Dialog Boxes Available")]
    [OnValueChanged("UpdateDialogBoxName")]
    [Tooltip("The dialog box to switch to.")]
    public DialogBox DialogBox;

    /// <summary>
    /// The name of the dialog box to open. This is used instead of the dialog
    /// box instance, since the dialog box instance may be destroyed on scene
    /// load due to the DialogManager singleton.
    /// </summary>
    private string dialogBoxName;

    [Space(8, order=1)]

    /// <summary>
    /// Output connection for the next node.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;
    #endregion

    #region Dialog Node API
    //---------------------------------------------------
    // Dialog Node API
    //---------------------------------------------------
    
    /// <summary>
    /// Switches to a different dialog box. UI element.
    /// </summary>
    public override void Handle(GraphEngine graphEngine) {
      DialogManager.SwitchToDialogBox(dialogBoxName);
    }

    #endregion

    #region Odin Inspector Related API
    //---------------------------------------------------
    // Odin Inspector Related API
    //---------------------------------------------------
    
    /// <summary>
    /// Finds the list of all available DialogBoxes in the scene.
    /// </summary>
    private IEnumerable<DialogBox> GetListOfDialogBoxes() {
      return GameObject.FindObjectsOfType<DialogBox>();
    }

    /// <summary>
    /// Updates the dialog box name.
    /// </summary>
    private void UpdateDialogBoxName(DialogBox box) {
      if (box != null) {
        dialogBoxName = box.name;
      } else {
        dialogBoxName = "";
      }
    }
    #endregion
  }

}