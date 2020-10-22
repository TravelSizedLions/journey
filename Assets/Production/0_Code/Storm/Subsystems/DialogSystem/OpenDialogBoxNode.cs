using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using XNode;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// A dialog node representing list of decisions.
  /// </summary>
  [NodeTint(NodeColors.DBOX_COLOR)]
  [NodeWidth(360)]
  [CreateNodeMenu("Dialog/Dialog Box/Open Dialog Box")]
  public class OpenDialogBoxNode : AutoNode {
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]

    /// <summary>
    /// The dialog box to open.
    /// </summary>
    [ValueDropdown("GetListOfDialogBoxes", DropdownWidth=400, DropdownTitle="Dialog Boxes Available")]
    [OnValueChanged("UpdateDialogBoxName")]
    [Tooltip("The dialog box to open.")]
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

    #region XNode API
    //---------------------------------------------------
    // XNode API
    //---------------------------------------------------
    
    /// <summary>
    /// Get the value of a port.
    /// </summary>
    /// <param name="port">The input/output port.</param>
    /// <returns>The value for the port.</returns>
    public override object GetValue(NodePort port) {
      return null;
    }
    #endregion


    #region Dialog Node API
    //---------------------------------------------------
    // Dialog Node API
    //---------------------------------------------------
    
    /// <summary>
    /// Invoke the events in the list.
    /// </summary>
    public override void Handle() {
      DialogManager.OpenDialogBox(dialogBoxName);
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