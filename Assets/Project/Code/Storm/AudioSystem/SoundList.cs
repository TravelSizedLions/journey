using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Storm.AudioSystem {

    [Serializable]
    public class SoundList : MonoBehaviour {

        public String Category;

        public List<Sound> sounds;

        public Sound this[int index] {
            get { return sounds[index]; }
        }

        public int Count {
            get { return sounds.Count; }
        }

        public void Start() {
            AudioManager.Instance.RegisterSounds(sounds);
        }
    }
}

