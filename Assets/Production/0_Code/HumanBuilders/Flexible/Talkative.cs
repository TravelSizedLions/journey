using System;




using UnityEngine;
using UnityEngine.Events;

namespace HumanBuilders {

  [RequireComponent(typeof(GuidComponent))]
  public class Talkative : PhysicalInteractible {
    
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    [Header("Dialog Setup", order=0)]
    [Space(5, order=1)]

    /// <summary>
    /// The dialog graph in the scene that will be used in the conversation.
    /// </summary>
    [Tooltip("The dialog graph in the scene that will be used in the conversation.")]
    public AutoGraph Dialog;

    /// <summary>
    /// A reference to this object's GUID.
    /// </summary>
    private GuidComponent GUID;


    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected void Start() {
      GUID = GetComponent<GuidComponent>();      
      LoadDialog();
    }

    protected new void OnDestroy() {
      base.OnDestroy();
      if (DialogManager.Instance != null) {
        DialogManager.EndDialog();
      }
    }


    //-------------------------------------------------------------------------
    // Interactible API
    //-------------------------------------------------------------------------

    /// <summary>
    /// What the object should do when interacted with.
    /// </summary>
    public override void OnInteract() {
      if (!interacting) {
        LoadDialog();
        if (Dialog != null) {
          interacting = true;
          DialogManager.StartDialog(Dialog.graph);
        }
      } else {
        DialogManager.ContinueDialog();
        

        if (DialogManager.IsDialogFinished()) {
          interacting = false;

          // This stops the player from hopping/twitching after the conversation
          // ends.
          Input.ResetInputAxes();
        }
      }
    }

    /// <summary>
    /// Whether or not the indicator for this interactible should be shown.
    /// </summary>
    /// <remarks>
    /// This is used when this particular interactive object is the closest to the player. If the indicator can be shown
    /// that usually means it can be interacted with.
    /// </remarks>
    public override bool ShouldShowIndicator() {
      return true;
    }

    /// <summary>
    /// Try to load the current dialog from the save file.
    /// </summary>
    private void LoadDialog() {
      if (GUID != null) {
        string key = GUID.ToString()+Keys.CURRENT_DIALOG;
        // Debug.Log("Trying to load: " + key);

        if (VSave.Get(StaticFolders.DIALOGS, key, out byte[] bytes)) {
          Guid guid = new Guid(bytes);
          GameObject go = GuidManager.ResolveGuid(guid);

          if (go != null) {
            Dialog = go.GetComponent<AutoGraph>();
          } else {
            Debug.LogWarning("Could not find Game Object with GUID " + guid.ToString());
          }
        }
      } else {
        Debug.LogWarning("Talkative object \"" + name + "\" needs a GuidComponent!");
      }
    }
  }
}