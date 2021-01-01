using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Storm.Characters;
using Storm.Characters.Player;
using Storm.Subsystems.FSM;
using UnityEngine;
using UnityEngine.Playables;

namespace Storm.Cutscenes {
  /// <summary>
  /// This class is a track mixer utilized by PlayerTracks to mix together the player's <see cref="FiniteStateMachine" /> and transform information.
  /// </summary>
  /// <seealso cref="PlayerCharacter"/>
  /// <seealso cref="PlayerCharacterTrack"/>
  /// <seealso cref="PoseClip"/>
  /// <seealso cref="AbsolutePoseClip"/>
  /// <seealso cref="RelativePoseClip"/>
  public class PlayerCharacterTrackMixer : PlayableBehaviour {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// A cache of state drivers for the finite state machine.
    /// </summary>
    private Dictionary<Type, StateDriver> stateDrivers;

    /// <summary>
    /// How to handle the end of a track.
    /// 
    /// <list type="bullet">
    /// <item> Resume - Resume normal play from where the track put the player. </item>
    /// <item> Freeze - Freeze the player in place where the track put them.</item>
    /// <item> Revert - Move the player back to their original state prior to the track.</item>
    /// <item> LoadScene - Load a new scene.</item>
    /// </list>
    /// </summary>
    public OutroSetting Outro;

    /// <summary>
    /// A snapshot of the way the player was before the cutscene.
    /// </summary>
    public PlayerSnapshot GraphSnapshot;

    /// <summary>
    /// virtual transform info about the player used to combine both relative
    /// and absolute changes in position.
    /// </summary>
    public PlayerSnapshot VirtualSnapshot;
    #endregion


    #region Playable API
    //-------------------------------------------------------------------------
    // Playable API
    //-------------------------------------------------------------------------
    
    /// <summary>
    /// Process a single frame of animation.
    /// </summary>
    /// <param name="playable">The playable this mixer is for.</param>
    /// <param name="info">Extra information about this frame.</param>
    /// <param name="playerData">Data bound to the track that this mixer is on.</param>
    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
      PlayerCharacter player = playerData != null ? (PlayerCharacter)playerData : GameManager.Player;
      if (player == null) {
        player = GameObject.FindObjectOfType<PlayerCharacter>();
      }

      if (IsSingleClipPlaying(playable, out int clipIndex)) {
        PoseInfo pose = GetPose(playable, clipIndex);
        MixSingleClip(playable, player, pose);

      } else {
        // Mix two clips together based on where we are in the timeline.
        FindClipsToMix(playable, out int clipIndexA, out int clipIndexB);             
        
        PoseInfo poseA = GetPose(playable, clipIndex);
        float weightA = playable.GetInputWeight(clipIndexA);

        PoseInfo poseB = GetPose(playable, clipIndexB);
        float weightB = playable.GetInputWeight(clipIndexB);
        
        MixClips(playable, player, poseA, weightA, poseB, weightB);
      }
    }
  
    /// <summary>
    /// Fires when the graph for the timeline begins playing.
    /// </summary>
    public override void OnGraphStart(Playable playable) {
      stateDrivers = new Dictionary<Type, StateDriver>();

      PlayerCharacter player = GameManager.Player != null ? GameManager.Player : GameObject.FindObjectOfType<PlayerCharacter>();
      if (player == null) {
        return;
      }

      GraphSnapshot = new PlayerSnapshot(player);
      VirtualSnapshot = GraphSnapshot;
    }

    /// <summary>
    /// Fires once the graph for the timeline stops playing.
    /// </summary>
    public override void OnGraphStop(Playable playable) {
      PlayerCharacter player = GameManager.Player != null ? GameManager.Player : GameObject.FindObjectOfType<PlayerCharacter>();
      if (player == null) {
        return;
      }

      Rigidbody2D rb = player.GetComponentInChildren<Rigidbody2D>(true);
      if (rb != null) {
        rb.gravityScale = 1;
        rb.velocity = Vector3.zero;
      }

      #if UNITY_EDITOR
      if (Application.isPlaying) {
      #endif

      switch (Outro) {
        case OutroSetting.Resume: {
          break;
        }
        case OutroSetting.Freeze: {
          player.Physics.GravityScale = 0;
          break;
        }
        case OutroSetting.Revert: {
          GraphSnapshot.RestoreState(player);
          GraphSnapshot.RestoreTransform(player);
          break;
        }
      }

      #if UNITY_EDITOR
      } else {
        GraphSnapshot.RestoreTransform(player);
      } 
      #endif
    }
    #endregion

    #region Clip Mixing
    //-------------------------------------------------------------------------
    // Clip Mixing
    //-------------------------------------------------------------------------

    /// <summary>
    /// Apply pose info for a single fully weighted clip.
    /// </summary>
    /// <param name="playable">The track's playable</param>
    /// <param name="player">The player character</param>
    /// <param name="pose">The pose to apply</param>
    private void MixSingleClip(Playable playable, PlayerCharacter player, PoseInfo pose) {
      if (IsAbsolute(pose)) { // Absolute
        ApplyAbsoluteClip(player, (AbsolutePoseInfo)pose);

      } else { // Relative
        VirtualSnapshot.RestoreTransform(player);
        ApplyRelativeClip(player, (RelativePoseInfo)pose);
      }

      StateDriver driver = GetDriver(pose);
      UpdateFacing(player, pose);
      UpdateState(player, driver, playable);
    }

    /// <summary>
    /// Mix together two active clips based on what type of clips they are.
    /// </summary>
    /// <param name="player">The player character</param>
    /// <param name="poseA">The first (e.g. earlier) pose</param>
    /// <param name="poseB">The second (e.g. later) pose</param>
    private void MixClips(Playable playable, PlayerCharacter player, PoseInfo poseA, float weightA, PoseInfo poseB, float weightB) {
      // Mix together clips based on their typing.
      if (IsAbsolute(poseA) && IsAbsolute(poseB)) {
        MixAbsoluteClips(player, (AbsolutePoseInfo)poseA, weightA, (AbsolutePoseInfo)poseB, weightB);
      } else if (IsRelative(poseA) && IsRelative(poseB)) {
        MixRelativeClips(player, (RelativePoseInfo)poseA, weightA, (RelativePoseInfo)poseB, weightB);
      } else {
        MixAbsoluteRelativeClips(player, poseA, weightA, poseB, weightB);
      }


      // Apply player facing and FSM state based on which pose is weighted heavier.
      PoseInfo driverPose = weightA > weightB ? poseA : poseB;
      StateDriver driver = GetDriver(driverPose);
      UpdateFacing(player, driverPose);
      UpdateState(player, driver, playable);
    }

    /// <summary>
    /// Mix together two absolute poses.
    /// </summary>
    /// <param name="player">The player character</param>
    /// <param name="poseA">The first pose</param>
    /// <param name="weightA">Interpolation weight for the first pose</param>
    /// <param name="poseB">The second pose</param>
    /// <param name="weightB">Interpolation weight for the second pose</param>
    private void MixAbsoluteClips(PlayerCharacter player, AbsolutePoseInfo poseA, float weightA, AbsolutePoseInfo poseB, float weightB) {
      ApplyAbsoluteClip(player, poseA, weightA, false);
      ApplyAbsoluteClip(player, poseB, weightB, true);
    }

    /// <summary>
    /// Mix together two relative poses.
    /// </summary>
    /// <param name="player">The player character</param>
    /// <param name="poseA">The first pose</param>
    /// <param name="weightA">Interpolation weight for the first pose</param>
    /// <param name="poseB">The second pose</param>
    /// <param name="weightB">Interpolation weight for the second pose</param>
    private void MixRelativeClips(PlayerCharacter player, RelativePoseInfo poseA, float weightA, RelativePoseInfo poseB, float weightB) {
      VirtualSnapshot.RestoreTransform(player);
      ApplyRelativeClip(player, poseA, weightA);
      ApplyRelativeClip(player, poseB, weightB);
    }

    /// <summary>
    /// Mix together a relative and absolute pose. For inputs, which pose is which doesn't matter.
    /// </summary>
    /// <param name="player">The player character</param>
    /// <param name="poseA">The first pose</param>
    /// <param name="weightA">Interpolation weight for the first pose</param>
    /// <param name="poseB">The second pose</param>
    /// <param name="weightB">Interpolation weight for the second pose</param>
    private void MixAbsoluteRelativeClips(PlayerCharacter player, PoseInfo poseA, float weightA, PoseInfo poseB, float weightB) {
      AbsolutePoseInfo absPose;
      RelativePoseInfo relPose;

      if (IsAbsolute(poseA)) {
        absPose = (AbsolutePoseInfo)poseA;
        relPose = (RelativePoseInfo)poseB;
      } else {
        absPose = (AbsolutePoseInfo)poseB;
        relPose = (RelativePoseInfo)poseA;
      }

      ApplyAbsoluteClip(player, absPose);
      ApplyRelativeClip(player, relPose);
    }


    /// <summary>
    /// Apply the pose of an absolute clip.
    /// </summary>
    /// <param name="player">The player character</param>
    /// <param name="pose">The pose to apply</param>
    /// <param name="weight">(optional) The weighting on the pose, used to
    /// interpolate this pose with another if necessary. Default: 1.</param>
    /// <param name="updateVirtualSnapshot">(optional) Whether or not to update the mixer's virtual snapshot
    /// of the player character. When mixing multiple absolute poses, this
    /// action can be saved for the last pose applied. Default: true.</param>
    private void ApplyAbsoluteClip(PlayerCharacter player, AbsolutePoseInfo pose, float weight = 1f, bool updateVirtualSnapshot = true) {
      Vector3 pos = VirtualSnapshot.Position;
      Vector3 rot = VirtualSnapshot.Rotation;
      Vector3 scl = VirtualSnapshot.Scale;

      player.transform.position = (pose.Position-pos)*weight + pos;
      player.transform.eulerAngles = (pose.Rotation-rot)*weight + rot;
      player.transform.localScale = (pose.Scale-scl)*weight + scl;

      if (updateVirtualSnapshot) {
        VirtualSnapshot = new PlayerSnapshot(player);
      }
    }

    /// <summary>
    /// Apply the pose of a relative clip.
    /// </summary>
    /// <param name="player">The player character</param>
    /// <param name="pose">The pose to apply</param>
    /// <param name="weight">(optional) The weighting on the pose, used to
    /// interpolate this pose with another if necessary. Default: 1.</param>
    private void ApplyRelativeClip(PlayerCharacter player, RelativePoseInfo pose, float weight = 1f) {
      player.transform.position += pose.Position*weight;
      player.transform.eulerAngles += pose.Rotation*weight;
      player.transform.localScale = (pose.Scale-VirtualSnapshot.Scale)*weight + VirtualSnapshot.Scale;
    }

    /// <summary>
    /// Determines whether or not this track is playing a single clip at full weight.
    /// </summary>
    /// <param name="playable">The track playable.</param>
    /// <param name="clipIndex">The output index of the clip</param>
    /// <returns>True if only one clip is active on the track at the current frame.</returns>
    private bool IsSingleClipPlaying(Playable playable, out int clipIndex) {
      for (int i = 0; i < playable.GetInputCount(); i++) {
        if (playable.GetInputWeight(i) == 1f) {
          clipIndex = i;
          return true;
        }
      }

      clipIndex = -1;
      return false;
    }

    /// <summary>
    /// Get the two clips that need to be mixed together for the current frame.
    /// </summary>
    /// <param name="playable">The track's playable.</param>
    /// <param name="clipIndexA">The output index for the first clip.</param>
    /// <param name="clipIndexB">The output index for the second clip.</param>
    private void FindClipsToMix(Playable playable, out int clipIndexA, out int clipIndexB) {
      for (int i = 0; i < playable.GetInputCount(); i++) {
        float weight = playable.GetInputWeight(i);
        if (weight > 0 && weight < 1f) {
          clipIndexA = i;
          clipIndexB = i+1;

          break;
        }
      }

      clipIndexA = -1;
      clipIndexB = -1;
    }
    #endregion

    #region Helper Methods
    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------

    /// <summary>
    /// Get information for a pose clip from the track's playable.
    /// </summary>
    /// <param name="playable">The track's playable</param>
    /// <param name="clipIndex">The index of the pose clip to get</param>
    /// <returns>Info about the pose</returns>
    private PoseInfo GetPose(Playable playable, int clipIndex) => ((ScriptPlayable<PoseInfo>)playable.GetInput(clipIndex)).GetBehaviour();

    /// <summary>
    /// Get the state driver for the current state.
    /// </summary>
    /// <param name="pose">The pose clip.</param>
    /// <returns>The state driver for the current state.</returns>
    private StateDriver GetDriver(PoseInfo pose) {
      if (pose == null || pose.State == null) {
        return null;
      }

      if (!stateDrivers.ContainsKey(pose.State)) {
        stateDrivers.Add(pose.State, StateDriver.For(pose.State));
      }
      return stateDrivers[pose.State];
    }

    /// <summary>
    /// Whether or not a pose clip is meant for absolute positioning.
    /// </summary>
    /// <param name="template">The pose information</param>
    /// <returns>True if the pose information is for an AbsolutePose clip</returns>
    private bool IsAbsolute(PoseInfo template) => template.GetType() == typeof(AbsolutePoseInfo);
    
    /// <summary>
    /// Whether or not a pose clip is meant for relative positioning.
    /// </summary>
    /// <param name="template">The pose information</param>
    /// <returns>True if the pose information is for a RelativePose clip</returns>
    private bool IsRelative(PoseInfo template) => template.GetType() == typeof(RelativePoseInfo);

    /// <summary>
    /// Update which way the player is facing if necessary.
    /// </summary>
    /// <param name="player">The player character.</param>
    /// <param name="pose">The target pose for the player.</param>
    private void UpdateFacing(PlayerCharacter player, PoseInfo pose) {
      if (pose.Flipped && !(player.Facing != Facing.Left)) {
        player.SetFacing(Facing.Left);
      } else if (!pose.Flipped && !(player.Facing != Facing.Right)) {
        player.SetFacing(Facing.Right);
      }
    }

    /// <summary>
    /// Update the animation/FSM state for the player.
    /// </summary>
    /// <param name="player">The player character.</param>
    /// <param name="driver">The state driver for the target state.</param>
    /// <param name="playable">The playable associated with this clip.</param>
    private void UpdateState(PlayerCharacter player, StateDriver driver, Playable playable) {
      if (driver == null) {
        return;
      }

      #if UNITY_EDITOR
      if (Application.isPlaying) {
      #endif

        if (!driver.IsInState(player.FSM)) {
          driver.ForceStateChangeOn(player.FSM);
        }

      #if UNITY_EDITOR
      } else {
        // In-Editor we don't want to actually change the Finite State Machine,
        // so just play the appropriate animation.
        driver.SampleClip(player.FSM, (float)playable.GetTime());
      }
      #endif
    }
    #endregion
  }
}