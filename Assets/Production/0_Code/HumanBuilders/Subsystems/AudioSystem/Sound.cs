using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace HumanBuilders {

  ///<summary>
  /// An in-game sound effect.
  ///</summary>
  [Serializable]
  public class Sound {

    #region Variables

    #region Identifiers
    /// <summary>
    /// The name of the sound
    /// </summary>
    public string Name {
      get { return (Clip != null ) ? Clip.name : "Sound"; }
    }

    /// <summary>
    /// The sound file.
    /// </summary>
    [FoldoutGroup("$Name")]
    [LabelWidth(50)]
    [Tooltip("The sound file.")]
    public AudioClip Clip;

    #endregion

    #region Audio Settings

    /// <summary>
    /// How loud the sound will play.
    /// </summary>
    [FoldoutGroup("$Name")]
    [Tooltip("How loud the sound will play.")]
    [LabelWidth(50)]
    [Range(0.0f, 1.0f)]
    public float Volume = 1.0f;

    /// <summary>
    /// Adjusts the fequency of the sound up or down.
    /// </summary>
    [FoldoutGroup("$Name")]
    [Tooltip("Adjust the fequency of the sound up or down.")]
    [LabelWidth(50)]
    [Range(0.1f, 3f)]
    public float Pitch = 1.0f;

    #endregion

    #region Hidden from Inspector
    /// <summary>
    /// How long to wait before playing the sound.
    /// </summary>
    [HideInInspector]
    public float Delay = 0;

    /// <summary>
    /// The mixer group for the given sound. Used to independently control
    /// volume for Music vs. SFX.
    /// </summary>
    [HideInInspector]
    public AudioMixerGroup Mixer;

    /// <summary>
    /// The source of the sound. Needed by the AudioManager to play the sound.
    /// </summary>
    [HideInInspector]
    public AudioSource Source;
    #endregion

    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    ///<summary>
    /// Makes a copy of the sound, minus a source.
    ///</summary>
    public Sound Copy() {
      Sound copy = new Sound();
      copy.Clip = Clip;
      copy.Volume = Volume;
      copy.Pitch = Pitch;
      copy.Delay = Delay;
      copy.Mixer = Mixer;
      return copy;
    }

    ///<summary>
    /// Prepare the sound to be played.
    ///</summary>
    public void Reload(GameObject gameObject) {
      Source = gameObject.AddComponent<AudioSource>();
      Source.playOnAwake = false;
      Source.clip = Clip;
      Source.volume = Volume;
      Source.pitch = Pitch;
      Source.outputAudioMixerGroup = Mixer;
    }
    #endregion
  }
}