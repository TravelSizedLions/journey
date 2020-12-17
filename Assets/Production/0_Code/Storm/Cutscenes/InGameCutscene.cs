using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Storm;
using Storm.Characters.Player;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace Storm.Cutscenes {

  public class InGameCutscene : MonoBehaviour {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The director for the cutscene.
    /// </summary>
    private PlayableDirector director;
    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Start() {
      director = GetComponent<PlayableDirector>();
      
      PopulatePlayerBindings();
      director.RebindPlayableGraphOutputs();
    }
    #endregion


    #region Helper Methods
    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------

    /// <summary>
    /// Repopulate any tracks in the timeline that drive the player.
    /// </summary>
    private void PopulatePlayerBindings() {
      foreach (PlayableBinding binding in director.playableAsset.outputs) {
        if (binding.streamName.ToLower().Contains("player")) {
          if (binding.sourceObject != null) {
            SetBinding(binding, GameManager.Player);
          }
        }
      }
    }

    /// <summary>
    /// Bind the player to the playable binding.
    /// </summary>
    /// <param name="binding">The binding (basically a wrapper around the track
    /// assets you create in timeline).</param>
    /// <param name="player">The player.</param>
    private void SetBinding(PlayableBinding binding, PlayerCharacter player) {
      Object source = binding.sourceObject;
      if (TryBind<AnimationTrack>(source, player.Animator)) return;
      if (TryBind<ActivationTrack>(source, player.gameObject)) return;
      if (TryBind<SignalTrack>(source, player.GetComponent<SignalReceiver>())) return;
      if (TryBind<AudioTrack>(source, player.GetComponent<AudioSource>())) return;
    }

    /// <summary>
    /// Try to bind an object to a specific type of track. If the source is not
    /// the right track type, the binding fails.
    /// </summary>
    /// <param name="source">The playable track.</param>
    /// <param name="obj">The object to bind.</param>
    /// <typeparam name="Track">The expected track type</typeparam>
    /// <returns>True if the binding succeeded. False otherwise.</returns>
    private bool TryBind<Track>(Object source, Object obj) where Track : TrackAsset {
      if (obj != null) {
        Track track = source as Track;

        if (track != null) {
          director.SetGenericBinding(source, obj);
          return true;
        }
      }

      return false;
    }

    [Button("Show Playable Graph")]
    private void ShowGraph() {
      if (director != null && director.playableGraph.IsValid()) {
        GraphVisualizerClient.Show(director.playableGraph);
      }
    }
    #endregion
  }
}
