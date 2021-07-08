using System;
using System.Collections;
using Sirenix.OdinInspector;


using UnityEngine;
using UnityEngine.Playables;
using XNode;

namespace HumanBuilders {

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

    /// <summary>
    /// Wait this number of seconds before continuing to traverse the graph.
    /// </summary>
    [BoxGroup("Timing", true, true)]
    [HorizontalGroup("Timing/1")]
    [Tooltip("Wait this number of seconds before playing the timeline clip.")]
    public float WaitBefore = 0.5f;

    /// <summary>
    /// Wait this number of seconds before continuing to traverse the graph.
    /// </summary>
    [Tooltip("Wait this number of seconds after the timeline clip is finished, before continuing to traverse the graph.")]
    [HorizontalGroup("Timing/2")]
    public float WaitAfter = 0.5f;

    /// <summary>
    /// Close any open dialog boxes before playing the timeline.
    /// </summary>
    [ShowIfGroup("IsInDialogGraph")]
    [BoxGroup("IsInDialogGraph/Dialog Settings", true, true)]
    [HorizontalGroup("IsInDialogGraph/Dialog Settings/Toggles")]
    [LabelText("Close Before")]
    [Tooltip("If open, close the dialog box before playing the timeline.")]
    public bool CloseDialogBefore = true;

    /// <summary>
    /// Reopen the default dialog box after the timeline finishes playing.
    /// </summary>
    [HorizontalGroup("IsInDialogGraph/Dialog Settings/Toggles")]
    [LabelText("Open After")]
    [Tooltip("If closed, reopen the default dialog box after the timeline finishes playing.")]
    public bool OpenDialogAfter = true;

    /// <summary>
    /// The starting position for the player. The player will move to this
    /// position before playing the timeline.
    /// </summary>
    [Tooltip("The starting position for the player. The player will move to this position before beginning the timeline.")]
    [ShowIfGroup("TimelineContainsPlayer")]
    [BoxGroup("TimelineContainsPlayer/Player Settings", true, true)]
    [HorizontalGroup("TimelineContainsPlayer/Player Settings/Target")]
    public Transform StartPosition;

    /// <summary>
    /// How fast the player should move into their starting position.
    /// </summary>
    [Tooltip("How fast the player should move into their starting position.")]
    [HorizontalGroup("TimelineContainsPlayer/Player Settings/Input Speed")]
    [Range(0, 1)]
    public float InputSpeed = 1f;

    [Space(8)]

    /// <summary>
    /// How to handle the end of the timeline.
    /// </summary>
    [Tooltip("How to handle the end of the timeline.\n\nResume: Resume normal play where the timeline ends.\nFreeze: Freeze the player where the timeline ends.\nRevert: Revent the player to their state prior to the timeline.\nLoad Scene: Load a new unity scene.")]
    [HorizontalGroup("TimelineContainsPlayer/Player Settings/Outro")]
    public OutroSetting Outro;

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
      if (graphEngine.LockNode()) {
        new UnityTask(PlayTimeline(graphEngine));
      }
    }
    #endregion

    #region Helper Methods
    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------
    
    /// <summary>
    /// Play a timeline!
    /// </summary>
    /// <param name="graphEngine">The graphing engine that called this node.</param>
    /// <returns></returns>
    private IEnumerator PlayTimeline(GraphEngine graphEngine) {
      bool containsPlayer = TimelineContainsPlayer();
      if (containsPlayer && StartPosition != null) {
        UnityTask walk = WalkToPosition.WalkTo(StartPosition, InputSpeed, graphEngine, true, WaitBefore);
      
        while (walk.Running) {
          yield return null;
        }
      }

      UnityTask play = new UnityTask(Play(graphEngine));

      while (play.Running) {
        yield return null;
      }
      
      graphEngine.UnlockNode();
    }

    /// <summary>
    /// Play a timeline (after dealing with the player character if they're in
    /// the scene).
    /// </summary>
    /// <param name="graphEngine">THe graphing engine that's handling this node.</param>
    private IEnumerator Play(GraphEngine graphEngine) {
      // Close the dialog box if desired.
      if (CloseDialogBefore) {
        DialogManager.CloseDialogBox();
      }

      yield return new WaitForSeconds(WaitBefore);

      UpdateOutro();

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
    }
    #endregion

    #region Odin Inspector Stuff
    //-------------------------------------------------------------------------
    // Odin Inspector
    //-------------------------------------------------------------------------
    
    /// <summary>
    /// Whether or not the director contains a track with a player binding.
    /// </summary>
    /// <returns>True if the timeline has a player track or other track with the
    /// player character bound to it.</returns>
    private bool TimelineContainsPlayer() {
      if (Director == null) {
        return false;
      }

      foreach (PlayableBinding binding in Director.playableAsset.outputs) {
        if (binding.outputTargetType == typeof(PlayerCharacter)) {
          return true;
        }
      }

      return false;
    }

#pragma warning disable 0618
    /// <summary>
    /// Whether or not this node is being used as part of a dialogue sequence.
    /// </summary>
    /// <returns>True if the graph contains</returns>
    private bool IsInDialogGraph() {
      foreach (Node node in graph.nodes) {
        switch(node) {
          case CloseDialogBoxNode node1:
          case DecisionNode node2:
          case EndDialogNode node3:
          case OpenDialogBoxNode node4:
          case SentenceNode node5:
          case StartDialogNode node6:
          case SwitchDialogNode node7:
          case SwitchDialogBoxNode node8:
          case TextNode node9:
          case DialogNode node10:
            return true;
          default:
            break;
        }
      }

      return false;
    }
#pragma warning restore 0618


    /// <summary>
    /// Update any player tracks in the timeline with the new Outro setting.
    /// </summary>
    private void UpdateOutro() {
      if (Director != null) {
        if (!Director.playableGraph.IsValid()) {
          Director.RebuildGraph();
        }

        foreach (PlayableBinding binding in Director.playableAsset.outputs) {
          if (binding.outputTargetType == typeof(PlayerCharacter)) {
            PlayerCharacterTrack track = (PlayerCharacterTrack) binding.sourceObject;
            if (track != null) {
              track.Outro = Outro;
            }
          }
        }
      }
    }
    #endregion
  }
}