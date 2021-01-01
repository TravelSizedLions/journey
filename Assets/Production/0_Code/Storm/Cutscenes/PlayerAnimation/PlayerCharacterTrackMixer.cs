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

      if (MixerTools.IsSingleClipPlaying(playable, out int clipIndex)) {
        MixSingle(playable, player, clipIndex);

      } else {

        MixerTools.FindClipsToMix(playable, out int clipIndexA, out int clipIndexB);             
        MixMultiple(playable, player, clipIndexA, clipIndexB);
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
    /// Mix a single clip.
    /// </summary>
    /// <param name="playable">The track's playable</param>
    /// <param name="player">The player character</param>
    /// <param name="clipIndex">The index of the clip to mix</param>
    private void MixSingle(Playable playable, PlayerCharacter player, int clipIndex) {
      PoseInfo pose = MixerTools.GetPlayableBehaviour<PoseInfo>(playable, clipIndex);

      if (PoseMixerTools.IsAbsolute(pose)) { // Absolute
        MixAbsolute(player, (AbsolutePoseInfo)pose);

      } else { // Relative
        VirtualSnapshot.RestoreTransform(player);
        MixRelative(player, (RelativePoseInfo)pose);
      }

      StateDriver driver = GetDriver(pose);
      UpdateFacing(player, pose);
      UpdateState(player, driver, playable);
    }

    /// <summary>
    /// Mix together two active clips based on what type of clips they are.
    /// </summary>
    /// <param name="playable">The track's playable</param>
    /// <param name="player">The player character</param>
    /// <param name="clipIndexA">The index of the first clip to mix</param>
    /// <param name="clipIndexB">The index of the second clip to mix</param>
    private void MixMultiple(Playable playable, PlayerCharacter player, int clipIndexA, int clipIndexB) {
      PoseInfo poseA = MixerTools.GetPlayableBehaviour<PoseInfo>(playable, clipIndexA);
      float weightA = playable.GetInputWeight(clipIndexA);

      PoseInfo poseB = MixerTools.GetPlayableBehaviour<PoseInfo>(playable, clipIndexB);
      float weightB = playable.GetInputWeight(clipIndexB);

      // Mix together clips based on their typing.
      if (PoseMixerTools.IsAbsolute(poseA) && PoseMixerTools.IsAbsolute(poseB)) {
        MixAbsolute(player, (AbsolutePoseInfo)poseA, weightA, false);
        MixAbsolute(player, (AbsolutePoseInfo)poseB, weightB, true);

      } else if (PoseMixerTools.IsRelative(poseA) && PoseMixerTools.IsRelative(poseB)) {
        VirtualSnapshot.RestoreTransform(player);
        MixRelative(player, (RelativePoseInfo)poseA, weightA);
        MixRelative(player, (RelativePoseInfo)poseB, weightB);

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

      if (PoseMixerTools.IsAbsolute(poseA)) {
        absPose = (AbsolutePoseInfo)poseA;
        relPose = (RelativePoseInfo)poseB;
      } else {
        absPose = (AbsolutePoseInfo)poseB;
        relPose = (RelativePoseInfo)poseA;
      }

      MixAbsolute(player, absPose);
      MixRelative(player, relPose);
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
    public void MixAbsolute(PlayerCharacter player, AbsolutePoseInfo pose, float weight = 1f, bool updateVirtualSnapshot = true) {
      PoseMixerTools.MixAbsolute(player, VirtualSnapshot, pose, weight);

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
    public void MixRelative(PlayerCharacter player, RelativePoseInfo pose, float weight = 1f) {
      PoseMixerTools.MixRelative(player, VirtualSnapshot, pose, weight);
    }
    #endregion

    #region Helper Methods
    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------

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