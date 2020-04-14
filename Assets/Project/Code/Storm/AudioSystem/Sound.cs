using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Storm.Attributes;

namespace Storm.AudioSystem {

    ///<summary>
    /// An in-game sound effect.
    ///</summary>
    [Serializable]
    public class Sound  {

        #region Variables
        /// <summary>
        /// The name of the sound
        /// </summary>
        public string Name {
            get { return Clip.name; }
        }

        /// <summary>
        /// The sound file.
        /// </summary>
        [Tooltip("The sound file.")]
        public AudioClip Clip;

        /// <summary>
        /// How loud the sound will play.
        /// </summary>
        [Tooltip("How loud the sound will play.")]
        [Range(0f, 1f)]
        public float Volume = 1f;

        /// <summary>
        /// Adjusts the fequency of the sound up or down.
        /// </summary>
        [Tooltip("Adjust the fequency of the sound up or down.")]
        [Range(0.1f, 3f)]
        public float Pitch = 1f;

        /// <summary>
        /// How long to wait before playing the sound.
        /// </summary>
        [HideInInspector]
        public float Delay = 0;

        /// <summary>
        /// The source of the sound. Needed by the AudioManager to play the sound.
        /// </summary>
        [HideInInspector]
        public AudioSource source;
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
            source = gameObject.AddComponent<AudioSource>();
            source.clip = Clip;
            source.volume = Volume;
            source.pitch = Pitch;
        }
        #endregion
    }
}
