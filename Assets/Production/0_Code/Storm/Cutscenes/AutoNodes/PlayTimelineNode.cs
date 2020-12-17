using System;
using System.Collections;
using Sirenix.OdinInspector;
using Storm.Characters.Player;
using Storm.Cutscenes;
using Storm.Subsystems.Dialog;
using Storm.Subsystems.Graph;
using UnityEngine;
using UnityEngine.Playables;
using XNode;

namespace Storm.Cutscenes {
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
    /// Wait this number of seconds before continuing to traverse the graph.
    /// </summary>
    [Tooltip("Wait this number of seconds before playing the timeline clip.")]
    public float WaitBefore = 0.5f;

    /// <summary>
    /// Wait this number of seconds before continuing to traverse the graph.
    /// </summary>
    [Tooltip("Wait this number of seconds after the timeline clip is finished, before continuing to traverse the graph.")]
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
    /// Move the player into a specific position before playing this timeline.
    /// </summary>
    [Tooltip("Move the player into a specific position before playing this timeline.")]
    [ShowIfGroup("TimelineContainsPlayer")]
    [BoxGroup("TimelineContainsPlayer/Player Settings", true, true)]
    [HorizontalGroup("TimelineContainsPlayer/Player Settings/Play In", 50)]
    public bool PlayIn = false;

    /// <summary>
    /// The starting position for the player.
    /// </summary>
    [Tooltip("The starting position for the player.")]
    [ShowIf("PlayIn")]
    [HorizontalGroup("TimelineContainsPlayer/Player Settings/Target")]
    public Transform Target;

    /// <summary>
    /// How fast the player should move into their starting position.
    /// </summary>
    [Tooltip("How fast the player should move into their starting position.")]
    [ShowIf("PlayIn")]
    [HorizontalGroup("TimelineContainsPlayer/Player Settings/Input Speed")]
    [Range(0, 1)]
    public float InputSpeed = 1f;

    /// <summary>
    /// Keep the player in the position the timeline ends them at.
    /// </summary>
    [Tooltip("Keep the player in the position the timeline ends them at.")]
    [HorizontalGroup("TimelineContainsPlayer/Player Settings/Play Out", 50)]
    public bool PlayOut = false;

    /// <summary>
    /// Keep the player's state machine paused. This stops player character
    /// logic and changes in animator state from happening.
    /// </summary>
    [Tooltip("Keep the player's state machine paused. This stops player character logic and changes in animator state from happening.")]
    [HorizontalGroup("TimelineContainsPlayer/Player Settings/Pause FSM")]
    [ShowIf("PlayOut")]
    public bool PauseFSM = false;


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
        new Task(PlayTimeline(graphEngine));
      }
    }
    #endregion

    #region Helper Methods
    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------
    
    private IEnumerator PlayTimeline(GraphEngine graphEngine) {
      bool containsPlayer = TimelineContainsPlayer();
      if (PlayIn && containsPlayer) {
        if (Target != null) {
          Task walk = new Task(WalkToPosition.MoveTo(Target, InputSpeed, graphEngine, true, WaitBefore));
        
          while (walk.Running) {
            yield return null;
          }
        } else {
          Debug.LogWarning(
            string.Format(
              "Set a target for the player to walk to in graph \"{1}.\"",
              graph.name
            )
          );
        }

      }

      Task wait = new Task(WaitUntilFinish(graphEngine, containsPlayer));

      while (wait.Running) {
        yield return null;
      }
      
      graphEngine.UnlockNode();
    }


    private IEnumerator WaitUntilFinish(GraphEngine graphEngine, bool containsPlayer) {
      // Close the dialog box if desired.
      if (CloseDialogBefore) {
        DialogManager.CloseDialogBox();
      }

      yield return new WaitForSeconds(WaitBefore);

      if (containsPlayer) {
        GameManager.Player.FSM.Pause();
      }

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


      if (containsPlayer && !PauseFSM) {
        GameManager.Player.FSM.Resume();
      }

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
    /// <returns></returns>
    private bool TimelineContainsPlayer() {
      if (Director == null) {
        return false;
      }

      foreach (PlayableBinding binding in Director.playableAsset.outputs) {
        if (binding.outputTargetType == typeof(PlayerCharacter)) {
          return true;
        }

        if (binding.streamName.ToLower().Contains("player")) {
          return true;
        }

        if (binding.outputTargetType == typeof(Animator)) {
          Animator anim = (Animator)Director.GetGenericBinding(binding.sourceObject);
          PlayerCharacter player = anim.GetComponentInChildren<PlayerCharacter>(true);
          if (player != null) {
            return true;
          }
        }
      }

      return false;
    }

    private bool IsInDialogGraph() {
      foreach (Node node in graph.nodes) {
        Type nodeType = node.GetType();
        if (nodeType == typeof(StartDialogNode) ||
            nodeType == typeof(SentenceNode) ||
            nodeType == typeof(TextNode)) {

          return true;
        }
      }

      return false;
    }
    #endregion
  }
}