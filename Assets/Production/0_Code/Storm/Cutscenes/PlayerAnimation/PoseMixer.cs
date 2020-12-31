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
  /// A timeline mixer for the player's <see cref="FiniteStateMachine" /> and transform information.
  /// </summary>
  public class PoseMixer : PlayableBehaviour {

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
    public PlayerSnapshot graphSnapshot;

    /// <summary>
    /// A snapshot of the way the player was before this clip.
    /// </summary>
    public PlayerSnapshot poseSnapshot;
    #endregion


    #region Unity API
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

      player.transform.position = Vector3.zero;
      player.transform.eulerAngles = Vector3.zero;
      player.transform.localScale = Vector3.zero;

      for (int i = 0; i < playable.GetInputCount(); i++) {
        ProcessInput(playable, i, player);
      }
    }
  
    /// <summary>
    /// When the graph for the timeline begins playing.
    /// </summary>
    public override void OnGraphStart(Playable playable) {
      stateDrivers = new Dictionary<Type, StateDriver>();

      PlayerCharacter player = GameManager.Player != null ? GameManager.Player : GameObject.FindObjectOfType<PlayerCharacter>();
      if (player == null) {
        return;
      }

      graphSnapshot = new PlayerSnapshot(player);
    }

    /// <summary>
    /// Once the graph for the timeline stops playing.
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
          graphSnapshot.RestoreState(player);
          graphSnapshot.RestoreTransform(player);
          break;
        }
      }

      #if UNITY_EDITOR
      } else {
        graphSnapshot.RestoreTransform(player);
      } 
      #endif
    }
    #endregion

    #region Helper Methods
    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------

    /// <summary>
    /// Mix in a single track input's position, rotation, and scale.
    /// </summary>
    /// <param name="playable">The playable for this mixer.</param>
    /// <param name="i">the playable input index.</param>
    /// <param name="player">The player character.</param>
    private void ProcessInput(Playable playable, int i, PlayerCharacter player) {
      float weight = playable.GetInputWeight(i);

      ScriptPlayable<PoseTemplate> script = (ScriptPlayable<PoseTemplate>)playable.GetInput(i);
      PoseTemplate pose = script.GetBehaviour();

      player.transform.position += pose.Position*weight;
      player.transform.eulerAngles += pose.Rotation*weight;
      player.transform.localScale += pose.Scale*weight;

      if (weight > 0.5f) {
        StateDriver driver = GetDriver(pose);
        UpdateFacing(player, pose);
        UpdateState(player, driver, playable);
      }
    } 

    /// <summary>
    /// Get the state driver for the current state.
    /// </summary>
    /// <param name="pose">The pose clip.</param>
    /// <returns>The state driver for the current state.</returns>
    private StateDriver GetDriver(PoseTemplate pose) {
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
    private void UpdateFacing(PlayerCharacter player, PoseTemplate pose) {
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