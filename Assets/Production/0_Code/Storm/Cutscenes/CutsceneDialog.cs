using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Storm.Flexible.Interaction;
using Storm.Subsystems.Dialog;
using Storm.Subsystems.Graph;
using UnityEngine;
using UnityEngine.Playables;

namespace Storm.Cutscenes {

  /// <summary>
  /// This component is used during in game cutscene to mimick dialog.
  /// The reason this class exists is to allow more than one dialog to be played
  /// in a cutscene.
  /// <br/>
  /// To use this in the editor, you'll want to familiarize yourself with
  /// Unity's <see cref="https://www.youtube.com/watch?v=G_uBFM3YUF4">Timeline Window</see> 
  /// as well as <see cref="https://www.youtube.com/watch?v=v14PdlcJnnA">Timeline Signals</see>
  /// <br/>
  /// The basic steps are:
  /// <list type="number">
  ///   <item>Create a game object and add an AutoGraph component. Create your dialog.</item>
  ///   <item>Add this component, then a Signal Receiver component.</item>
  ///   <item>Drag the game object over to the timeline for your cutscene,
  ///   adding an animation track.</item>
  ///   <item>At the points where you want dialog to play, add signal a signal
  ///   emitter with the desired signal. This would be things like start_dialog,
  ///   continue_dialog, etc.</item>
  ///   <item>Back on the game object, add reactions each signal and use this
  ///   API to advance the dialog.</item>
  /// </list>
  /// </summary>
  [RequireComponent(typeof(GuidComponent))]
  [RequireComponent(typeof(AutoGraph))]
  public class CutsceneDialog : Interactible {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// Pause the cutscene while this dialog is going on.
    /// </summary>
    [Tooltip("Pause the cutscene while this dialog is going on.")]
    public bool PauseCutscene = true;

    /// <summary>
    /// The cutscene to pause while dialog is going on. If no cutscene is
    /// provided, any cutscene that's playing will continue unhindered.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [Tooltip("The cutscene associated with this dialog.")]
    private PlayableDirector director;

    /// <summary>
    /// The dialog to use in the cutscene!
    /// </summary>
    private AutoGraph dialog = null;

    /// <summary>
    /// A reference to this object's GUID.
    /// </summary>
    private GuidComponent GUID = null;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    protected override void Awake() {
      base.Awake();

      director = GetComponentInParent<PlayableDirector>();
      dialog = GetComponent<AutoGraph>();
      GUID = GetComponent<GuidComponent>();
    }


    #endregion

    /// <summary>
    /// Start a dialog from the middle of a cutscene.
    /// </summary>
    /// <seealso cref="DialogManager.StartDialog" />
    public void StartDialog() {
      GameManager.Player.Interact(this);
    }

    /// <summary>
    /// Continue the dialog (used if PauseCutscene is false to forward the
    /// conversation through timeline signals).
    /// </summary>
    public void ContinueDialog() {
      GameManager.Player.Interact();
    }

    #region Interactible API
    //-------------------------------------------------------------------------
    // Interactible API
    //-------------------------------------------------------------------------

    /// <summary>
    /// Starts or continues a dialog.
    /// </summary>
    public override void OnInteract() {
      if (!interacting) {
        StartInteraction();
      } else {
        ContinueInteraction();
      }
    }

    /// <summary>
    /// Start up the dialog.
    /// </summary>
    private void StartInteraction() {
      if (dialog != null) {
        interacting = true;

        if (director != null && PauseCutscene) {
          if (!director.playableGraph.IsValid()) {
            director.RebuildGraph();
          }

          director.playableGraph.GetRootPlayable(0).Pause();
        }

        DialogManager.StartDialog(GetComponent<AutoGraph>());
      }
    }

    /// <summary>
    /// Continue the dialog.
    /// </summary>
    private void ContinueInteraction() {
      DialogManager.ContinueDialog();
      if (DialogManager.IsDialogFinished()) {
        interacting = false;

        if (director != null && PauseCutscene) {
          if (!director.playableGraph.IsValid()) {
            director.RebuildGraph();
          }
          director.playableGraph.GetRootPlayable(0).Play();
        }

        Input.ResetInputAxes();
      }
    }
    #endregion

  }

}