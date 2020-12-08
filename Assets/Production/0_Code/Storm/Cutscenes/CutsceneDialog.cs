using System.Collections;
using System.Collections.Generic;
using Storm.Subsystems.Dialog;
using Storm.Subsystems.Graph;
using UnityEngine;

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
  public class CutsceneDialog : MonoBehaviour {
    
    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Whether or not the player should still be allowed to move around during
    /// the cutscene's dialog.
    /// </summary>
    public bool disablePlayerInput = true;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    /// <summary>
    /// Start a dialog.
    /// </summary>
    /// <seealso cref="DialogManager.StartDialog" />
    public void StartDialog() => DialogManager.StartDialog(GetComponent<AutoGraph>(), disablePlayerInput);

    /// <summary>
    /// Continue the current dialog.
    /// </summary>
    /// <seealso cref="DialogManager.ContinueDialog" />
    public void ContinueDialog() => DialogManager.ContinueDialog();

    /// <summary>
    /// End the current dialog.
    /// </summary>
    /// <seealso cref="DialogManager.EndDialog" />
    public void EndDialog() => DialogManager.EndDialog();

    /// <summary>
    /// Clear the text and speaker from the open dialog box.
    /// </summary>
    /// <seealso cref="DialogManager.ClearText" />
    public void ClearText() => DialogManager.ClearText();
    #endregion
  }

}