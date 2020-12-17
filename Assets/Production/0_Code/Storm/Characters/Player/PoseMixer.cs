using System;
using System.Collections.Generic;
using Storm.Characters.Player;
using Storm.Subsystems.FSM;
using UnityEngine;
using UnityEngine.Playables;

namespace Storm.Cutscenes {
  public class PoseMixer : PlayableBehaviour {


    private Dictionary<Type, StateDriver> stateDrivers;

    public bool KeepStateAfterPlay;

    private Animation previousAnimation;

    private Vector3 previousPosition;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
      PlayerCharacter player = playerData != null ? (PlayerCharacter)playerData : GameManager.Player;
      if (player == null) {
        player = GameObject.FindObjectOfType<PlayerCharacter>();
      }

      player.transform.position = Vector3.zero;

      for (int i = 0; i < playable.GetInputCount(); i++) {
        ProcessInput(playable, i, player);
      }

    }
  
    /// <summary>
    /// Process a single input.
    /// </summary>
    /// <param name="playable">The playable.</param>
    /// <param name="i">the playable input index.</param>
    /// <param name="player">The player character.</param>
    private void ProcessInput(Playable playable, int i, PlayerCharacter player) {
      float weight = playable.GetInputWeight(i);

      if (weight > 0.5f) {
        ScriptPlayable<PoseTemplate> script = (ScriptPlayable<PoseTemplate>)playable.GetInput(i);
        PoseTemplate pose = script.GetBehaviour();

        player.transform.position += pose.Position*weight;

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

#if UNITY_EDITOR
        }
#endif
      }
    } 
  


    public override void OnGraphStart(Playable playable) {
      stateDrivers = new Dictionary<Type, StateDriver>();

      PlayerCharacter player = GameManager.Player != null ? GameManager.Player : GameObject.FindObjectOfType<PlayerCharacter>();
      if (player == null) {
        return;
      }

      previousPosition = player.transform.position;
    }

    public override void OnGraphStop(Playable playable) {
      PlayerCharacter player = GameManager.Player != null ? GameManager.Player : GameObject.FindObjectOfType<PlayerCharacter>();
      if (player == null) {
        return;
      }
      
      if (!KeepStateAfterPlay) {
        player.transform.position = previousPosition;
      }

#if UNITY_EDITOR
      if (!Application.isPlaying) {
        player.transform.position = previousPosition;
      } 
#endif

    }


  }
}