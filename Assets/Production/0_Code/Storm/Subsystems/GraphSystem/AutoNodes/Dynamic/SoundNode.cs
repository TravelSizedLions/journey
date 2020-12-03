
using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Storm.Subsystems.Dialog;
using UnityEngine;
using XNode;
using Random = UnityEngine.Random;

namespace Storm.Subsystems.Graph {

  /// <summary>
  /// How to play the given sound.
  /// </summary>
  public enum SoundMode {
    Play,
    PlayOneShot,
    PlayClipAtPoint,
    PlayDelayed,
    PlayScheduled
  }

  /// <summary>
  /// A node which causes a delay.
  /// </summary>
  [NodeTint(NodeColors.DYNAMIC_COLOR)]
  [NodeWidth(450)]
  [CreateNodeMenu("Dynamic/Sound")]
  public class SoundNode : AutoNode {

    #region Fields
    //---------------------------------------------------
    // Fields
    //---------------------------------------------------

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(10, order=0)]

    /// <summary>
    /// What mode to use to play the sound.
    /// </summary>
    [Tooltip("What mode to use to play the sound.\nPlay - play the sound normally, and loop if applicable.\nPlay One Shot - play the sound once (good for sound effects).\n")]
    public SoundMode mode = SoundMode.PlayOneShot;

    [Space(20, order=1)]

    /// <summary>
    /// The sound to play (if mode is PlayClipAtPoint)
    /// </summary>
    [Tooltip("The sound to play")]
    [ShowIf("mode", SoundMode.PlayClipAtPoint)]
    public AudioClip soundClip;

    /// <summary>
    /// The sound to play.
    /// </summary>
    [Tooltip("The sound to play.")]
    [HideIf("mode", SoundMode.PlayClipAtPoint)]
    public AudioSource sound;

    [Space(10, order=2)]

    /// <summary>
    /// What pitch to play this sound at.
    /// </summary>
    [Tooltip("What pitch to play this sound at.")]
    [Range(-3, 3)]
    public float pitch = 1;

    /// <summary>
    /// How much to vary the pitch of the sound (+/- this amount).
    /// </summary>
    [Tooltip("How much to vary the pitch of the sound (+/- this amount).")]
    [Range(0, 6)]
    public float pitchVariance = 0;

    [Space(10, order=3)]

    /// <summary>
    /// The place to play this sound from.
    /// </summary>
    [Tooltip("The place to play this sound from.")]
    [ShowIf("mode", SoundMode.PlayClipAtPoint)]
    public Transform point;

    [Space(10, order=3)]

    /// <summary>
    /// The volume to play the sound at.
    /// </summary>
    [Tooltip("The volume to play the sound at.")]
    [Range(0, 1)]
    public float volume = 1;

    /// <summary>
    /// How much to vary the volume of the sound (+/- this amount).
    /// </summary>
    [Tooltip("How much to vary the volume of the sound (+/- this amount).")]
    [Range(0, 1)]
    public float volumeVariance = 0;

    [Space(10, order=4)]

    /// <summary>
    /// How many seconds to wait before playing the sound.
    /// </summary>
    [Tooltip("How many seconds to wait before playing the sound.")]
    [ShowIf("mode", SoundMode.PlayDelayed)]
    public float delay;

    /// <summary>
    /// How much to vary the delay (+/- this number of seconds)
    /// </summary>
    [Tooltip("How much to vary the delay (+/- this number of seconds)")]
    [ShowIf("mode", SoundMode.PlayDelayed)]
    public float delayVariance = 0;

    [Space(10, order=5)]

    /// <summary>
    /// The time the sound should be played (in seconds, since the audio engine
    /// began running).
    /// </summary>
    [Tooltip("The time the sound should be played (in seconds, since the audio engine began running).")]
    [ShowIf("mode", SoundMode.PlayScheduled)]
    public float playAtTime = 0;

    /// <summary>
    /// The output connection for this node.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;
    #endregion
    
    #region XNode API
    //---------------------------------------------------
    // XNode API
    //---------------------------------------------------
    
    /// <summary>
    /// Get the value of a port.
    /// </summary>
    /// <param name="port">The input/output port.</param>
    /// <returns>The value for the port.</returns>
    public override object GetValue(NodePort port) {
      return null;
    }
    #endregion

    #region Dialog Node API
    //---------------------------------------------------
    // Dialog Node API
    //---------------------------------------------------
    
    /// <summary>
    /// Plays a sound.
    /// </summary>
    public override void Handle(GraphEngine graphEngine) {
      switch (mode) {
        case SoundMode.Play: {
          sound.pitch = GetPitch();
          sound.volume = GetVolume();
          sound.Play();
          break;
        }
        case SoundMode.PlayOneShot: {
          Debug.Log("PLAYING!");
          sound.pitch = GetPitch();
          sound.PlayOneShot(sound.clip, GetVolume());
          break;
        }
        case SoundMode.PlayClipAtPoint: {
          AudioSource.PlayClipAtPoint(soundClip, point.position, GetVolume());
          break;
        }
        case SoundMode.PlayDelayed: {
          sound.PlayDelayed(GetDelay());
          break;
        }
        case SoundMode.PlayScheduled: {
          sound.PlayScheduled(playAtTime);
          break;
        }
      }
    }

    /// <summary>
    /// Get a pitch for the sound based on the node's settings.
    /// </summary>
    private float GetPitch() {
      return pitchVariance == 0 ? pitch : Mathf.Clamp(Random.Range(pitch-pitchVariance, pitch+pitchVariance), -3, 3);
    }

    /// <summary>
    /// Get a volume for the sound based on the node's settings.
    /// </summary>
    private float GetVolume() {
      return volumeVariance == 0 ? volume : Mathf.Clamp(Random.Range(volume-volumeVariance,volume+volumeVariance), 0, 1);
    }

    /// <summary>
    /// Get a delay for the sound based on the node's settings.
    /// </summary>
    private float GetDelay() {
      float value = Random.Range(delay-delayVariance, delay+delayVariance);
      return delayVariance == 0 ? delay : Mathf.Clamp(value, 0, value);
    }

    #endregion

    #region Odin Inspector Stuff
    //-------------------------------------------------------------------------
    // Odin Inspector
    //-------------------------------------------------------------------------
    
    /// <summary>
    /// Whether or not to show pitch settings on the node.
    /// </summary>
    private bool ShowPitchSettings() => mode == SoundMode.Play || mode == SoundMode.PlayOneShot;
    
    /// <summary>
    /// Whether or not to show volume settings on the node.
    /// </summary>
    private bool ShowVolumeSettings() => mode == SoundMode.Play || mode == SoundMode.PlayOneShot || mode == SoundMode.PlayClipAtPoint;

    #endregion
  }
}
