using System;
using System.Collections;
using System.Collections.Generic;
using Storm.Flexible;
using Storm.Subsystems.Save;
using Storm.Subsystems.Transitions;
using UnityEngine;

namespace Storm.Subsystems.Dialog {
  public class SwitchDialog : MonoBehaviour, IStorable {
    
    /// <summary>
    /// The target to switch the dialog on.
    /// </summary>
    [Tooltip("The GUID reference to the target to switch the dialog on.")]
    public GuidReference Target;

    [Space(10, order=0)]

    /// <summary>
    /// The GUID reference to the dialog graph in the scene that will be used in the conversation.
    /// </summary>
    [Tooltip("The GUID reference to the dialog graph in the scene that will be used in the conversation.")]
    public GuidReference Dialog;


    public void Switch() {
      if (Target != null) {

        // If the game object exists, that means we're in the scene the object
        // belongs in.
        if (Target.gameObject != null) {
          Talkative talkative = Target.gameObject.GetComponent<Talkative>();
          SceneAutoGraph dialog = Dialog.gameObject.GetComponent<SceneAutoGraph>();
          
          talkative.Dialog = dialog;
        }

        // Tell the save system that we're switching out dialogs.
        Store();

      } else {
        Debug.LogWarning("DialogSwitch object has no target!");
      }
    }


    #region Storable API
    public void Store() {
      VSave.Set(StaticFolders.DIALOGS, Target.ToString()+Keys.CURRENT_DIALOG, Dialog.ToByteArray());
    }


    public void Retrieve() {

    }
    #endregion
  }
}