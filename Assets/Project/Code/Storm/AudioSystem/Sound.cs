using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Storm.Attributes;

namespace Storm.AudioSystem {

    [Serializable]
    public class Sound  {

        public string Name;

        public AudioClip Clip;

        [Range(0f, 1f)]
        public float Volume = 1f;

        [Range(0.1f, 3f)]
        public float Pitch = 1f;

        [HideInInspector]
        public float Delay = 0;

        [ReadOnly]
        public AudioSource source;


        public Sound Copy() {
            Sound copy = new Sound();
            copy.Name = Name;
            copy.Clip = Clip;
            copy.Volume = Volume;
            copy.Pitch = Pitch;
            copy.Delay = Delay;

            return copy;
        }

        public void Reload(GameObject gameObject) {
            source = gameObject.AddComponent<AudioSource>();
            source.clip = Clip;
            source.volume = Volume;
            source.pitch = Pitch;
        }
    }
}