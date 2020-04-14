using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Storm.AudioSystem {

    ///<summary>
    /// A group of related sounds.
    ///</summary>
    ///<remarks>
    /// This class is intended to help keep related sounds organized
    /// and attached to the AudioManager. For example, one could add
    /// a collection of explosion sounds that could be played randomly
    /// whenever an enemy dies.
    ///</remarks>
    [Serializable]
    public class SoundList : MonoBehaviour {

        /// <summary>
        /// The name of the list.
        /// </summary>
        public String Category;

        /// <summary>
        /// The colection of sounds.
        /// </summary>
        public List<Sound> sounds;

        ///<summary>
        /// Index operator
        ///</summary>
        public Sound this[int index] {
            get { return sounds[index]; }
        }

        ///<summary>
        /// The number of sounds in the collection.
        ///</summary>
        public int Count {
            get { return sounds.Count; }
        }

        ///<summary>
        /// Fires before the first frame is rendered.
        ///</summary>
        public void Start() {
            AudioManager.Instance.RegisterSounds(sounds);
        }
    }
}

