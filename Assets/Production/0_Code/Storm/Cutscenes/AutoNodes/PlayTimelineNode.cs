using System;
using System.Collections;
using Sirenix.OdinInspector;
using Storm.Subsystems.Dialog;
using Storm.Subsystems.Graph;
using UnityEngine;
using UnityEngine.Playables;
using XNode;

namespace Storm.Subsystems.Dialog {
  /// <summary>
  /// A dialog node that plays a timeline asset.
  /// </summary>
  [NodeWidth(400)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("Animation/Play Timeline")]
  public class PlayTimelineNode : AutoNode {
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
    /// The playable director for the timeline.
    /// </summary>
    [Tooltip("The playable director for the timeline.")]
    public PlayableDirector Director;

    [Space(8, order=1)]

    /// <summary>
    /// Wait until the timeline clip is finished before continuing to traverse
    /// the graph.
    /// </summary>
    [Tooltip("Pause graph execution until after the timeline clip is finished.")]
    public bool PauseGraph = true;

    /// <summary>
    /// Wait this number of seconds before continuing to traverse the graph.
    /// </summary>
    [BoxGroup("Dialog Settings", true, true)]
    [HorizontalGroup("Dialog Settings/Timer")]
    [VerticalGroup("Dialog Settings/Timer/Timers")]
    [ShowIf("PauseGraph")]
    [Tooltip("Wait this number of seconds before playing the timeline clip.")]
    public float WaitBefore = 0.5f;

    /// <summary>
    /// Wait this number of seconds before continuing to traverse the graph.
    /// </summary>
    [BoxGroup("Dialog Settings", true, true)]
    [VerticalGroup("Dialog Settings/Timer/Timers")]
    [ShowIf("PauseGraph")]
    [Tooltip("Wait this number of seconds after the timeline clip is finished, before continuing to traverse the graph.")]
    public float WaitAfter = 0.5f;

    /// <summary>
    /// Close any open dialog boxes before playing the timeline.
    /// </summary>
    [BoxGroup("Dialog Settings", true, true)]
    [HorizontalGroup("Dialog Settings/Toggles")]
    [ShowIf("PauseGraph")]
    [LabelText("Close Before")]
    [Tooltip("If open, close the dialog box before playing the timeline.")]
    public bool CloseDialogBefore = true;


    /// <summary>
    /// Reopen the default dialog box after the timeline finishes playing.
    /// </summary>
    [BoxGroup("Dialog Settings", true, true)]
    [HorizontalGroup("Dialog Settings/Toggles")]
    [ShowIf("PauseGraph")]
    [LabelText("Open After")]
    [Tooltip("If closed, reopen the default dialog box after the timeline finishes playing.")]
    public bool OpenDialogAfter = true;

    /// <summary>
    /// Output connection for the next node.
    /// </summary>
    [Space(8, order=1)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;
    #endregion

    #region Auto Node API
    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (PauseGraph) {
        graphEngine.StartThread(WaitUntilFinish(graphEngine));
      } else {
        Director.RebuildGraph();
        Director.Play();
      }
    }
    #endregion

    #region Helper Methods
    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------
    private IEnumerator WaitUntilFinish(GraphEngine graphEngine) {
      if (graphEngine.LockNode()) {
        Debug.Log("Waiting");

        // Close the dialog box if desired.
        if (CloseDialogBefore) {
          DialogManager.CloseDialogBox();
        }

        yield return new WaitForSeconds(WaitBefore);

        // Play timeline and wait until finished.
        Director.RebuildGraph();
        Director.Play();
        bool playing = true;

        Action<PlayableDirector> onStop = (PlayableDirector director) => { playing = false; };
        Director.stopped += onStop;

        while (playing && Director.playableGraph.IsPlaying()) {
          yield return null;
        }

        Director.stopped -= onStop;

        // Wait additional time if desired.
        yield return new WaitForSeconds(WaitAfter);

        // Reopen the default dialog box if desired.
        if (CloseDialogBefore && OpenDialogAfter) {
          DialogManager.OpenDialogBox();
        }

        Debug.Log("Continuing");

        graphEngine.UnlockNode();
      }
    }
    #endregion
  }
}