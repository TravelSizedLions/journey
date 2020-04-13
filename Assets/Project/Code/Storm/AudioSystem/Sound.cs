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

        /** The name of the sound. */
        public string Name;

        /** The sound file. */
        public AudioClip Clip;

        /** How loud the sound will play. */
        [Range(0f, 1f)]
        public float Volume = 1f;

        /** Adjusts the fequency of the sound up or down. */
        [Range(0.1f, 3f)]
        public float Pitch = 1f;

        /** How long to wait before playing the sound. */
        [HideInInspector]
        public float Delay = 0;

        /** The source of the sound. Needed by the AudioManager to play the sound. */
        [ReadOnly]
        public AudioSource source;

        ///<summary>
        /// Makes a copy of the sound, minus a source.
        ///</summary>
        public Sound Copy() {
            Sound copy = new Sound();
            copy.Name = Name;
            copy.Clip = Clip;
            copy.Volume = Volume;
            copy.Pitch = Pitch;
            copy.Delay = Delay;

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
    }
}
