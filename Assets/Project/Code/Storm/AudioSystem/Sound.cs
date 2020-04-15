using System;
using System.Collections;
using System.Collections.Generic;
using Storm.Attributes;
using UnityEngine;

namespace Storm.AudioSystem {

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
      get { return Clip.name; }
    }

    [Header("Clip", order = 0)]
    [Space(5, order = 1)]

    /// <summary>
    /// The sound file.
    /// </summary>
    [Tooltip("The sound file.")]
    public AudioClip Clip;

    [Space(10, order = 2)]
    #endregion

    #region Audio Settings
    [Header("Audio Settings", order = 3)]

    [Space(5, order = 4)]
    /// <summary>
    /// How loud the sound will play.
    /// </summary>
    [Tooltip("How loud the sound will play.")]
    [Range(0f, 1f)]
    public float Volume = 1.0f;

    /// <summary>
    /// Adjusts the fequency of the sound up or down.
    /// </summary>
    [Tooltip("Adjust the fequency of the sound up or down.")]
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
      Debug.Log(Clip.name);
      return copy;
    }

    ///<summary>
    /// Prepare the sound to be played.
    ///</summary>
    public void Reload(GameObject gameObject) {
      Source = gameObject.AddComponent<AudioSource>();
      Source.clip = Clip;
      Source.volume = Volume;
      Source.pitch = Pitch;
    }
    #endregion
  }
}