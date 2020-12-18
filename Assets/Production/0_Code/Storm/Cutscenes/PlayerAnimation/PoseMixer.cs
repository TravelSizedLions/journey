using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Storm.Characters;
using Storm.Characters.Player;
using Storm.Subsystems.FSM;
using UnityEngine;
using UnityEngine.Playables;

namespace Storm.Cutscenes {
  public class PoseMixer : PlayableBehaviour {

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
    public PlayerSnapshot snapshot;


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

#if UNITY_EDITOR
        if (Application.isPlaying) {
#endif
          if (!stateDrivers.ContainsKey(pose.State)) {
            stateDrivers.Add(pose.State, StateDriver.For(pose.State));
          }

          StateDriver driver = stateDrivers[pose.State];
          if (!driver.IsInState(player.FSM)) {
            driver.ForceStateChangeOn(player.FSM);
          }

          if (pose.Flipped) {
            player.SetFacing(Facing.Left);
          } else {
            player.SetFacing(Facing.Right);
          }

#if UNITY_EDITOR
        }
#endif
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

      snapshot = new PlayerSnapshot(player);
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
            snapshot.RestoreState(player);
            snapshot.RestoreTransform(player);
            break;
          }
        }

#if UNITY_EDITOR
      } else {
        snapshot.RestoreTransform(player);
      } 
#endif

    }
  }


  /// <summary>
  /// A snapshot of important visual infomation for the player character.
  /// </summary>
  public class PlayerSnapshot {
    /// <summary>
    /// A driver for the state that the player's finite state machine was
    /// previously in. This allows the finite state machine to roll back to
    /// its previous state.
    /// </summary>
    private StateDriver driver;

    /// <summary>
    /// The player's position.
    /// </summary>
    private Vector3 position;

    /// <summary>
    /// The player's euler rotation (x, y, z).
    /// </summary>
    private Vector3 rotation;

    /// <summary>
    /// The player's local scale.
    /// </summary>
    private Vector3 scale;

    /// <summary>
    /// Whether or not the player is facing left or right.
    /// </summary>
    private Facing facing;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="player">The player character.</param>
    public  PlayerSnapshot(PlayerCharacter player) {
      driver = StateDriver.For(player.FSM.CurrentState);
      position = player.transform.position;
      rotation = player.transform.eulerAngles;
      scale = player.transform.localScale;
      facing = player.Facing;
    }

    /// <summary>
    /// Restore the transform the player was in.
    /// </summary>
    /// <param name="player">The player character.</param>
    public void RestoreTransform(PlayerCharacter player) {
      player.transform.position = position;
      player.transform.eulerAngles = rotation;
      player.transform.localScale = scale;
      player.SetFacing(facing);
    }

    /// <summary>
    /// Restore the state the player's state machine was in.
    /// </summary>
    /// <param name="player">The player character.</param>
    public void RestoreState(PlayerCharacter player) {
      if (driver != null && !driver.IsInState(player.FSM)) {
        driver.ForceStateChangeOn(player.FSM);
      }
    }
  }
}