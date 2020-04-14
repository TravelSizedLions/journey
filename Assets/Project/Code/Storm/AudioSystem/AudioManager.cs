using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Extensions;
using Storm.Attributes;

namespace Storm.AudioSystem {

    ///<summary>
    /// This service is meant to play music and sound effects from a predefined list of sounds/music. 
    /// Any consuming code may search for a sound among the SoundList components attached to this class'
    /// GameObject.
    ///</summary>
    ///<remarks>
    /// The following is an example of how a class can use the AudioManager:
    ///
    ///<code>
    /// // Search for the list of explosion sounds.
    /// foreach (SoundList list in FindObjectsOfType<>()) {
    ///     if (list.Category.Contains("Explosion")) {
    ///
    ///         // Play a random sound from the list.
    ///         int explodeNum = Random.Range(0, list.Count);
    ///         Sound sound = list[explodeNum];
    ///         AudioManager.Instance.Play(sound.Name);
    ///     }
    /// }
    ///
    /// ...
    ///
    /// // Play a sound after some time.
    /// AudioManager.Instance.Play("SippingSoda");
    /// AudioManager.Instance.PlayDelayed("Burp", 2.0f);
    ///</code>
    ///</remarks>
    public class AudioManager : Singleton<AudioManager> {

        #region Variables
        /// <summary>
        /// A map of sound names to sounds.
        /// </summary>
        private Dictionary<string, Sound> soundTable;

        /// <summary>
        /// The list of sounds to be played. 
        /// </summary>
        private Queue<Sound> soundQueue;

        /// <summary>
        /// The list of sounds currently being played.
        /// </summary>
        private List<AudioSource> playingSounds;
        #endregion

        #region Unity API
        //---------------------------------------------------------------------
        // Unity API
        //---------------------------------------------------------------------
        
        ///<summary>
        /// Fires before the first call to Start() of any GameObject.
        ///</summary>
        protected override void Awake() {
            base.Awake();

            soundQueue = new Queue<Sound>();
            soundTable = new Dictionary<string, Sound>();
            playingSounds = new List<AudioSource>();
        }

        ///<summary>
        /// Fires every Time.fixedDeltaTime seconds.
        ///</summary>
        private void FixedUpdate() {
            if (soundQueue.Count > 0) {
                Sound sound = soundQueue.Dequeue();
                playingSounds.Add(sound.Source);
                
                Debug.Log(sound.Delay);
                
                sound.Source.PlayDelayed(sound.Delay);
            }

            // Clean up sounds that are finished playing...
            foreach (AudioSource source in playingSounds) {
                if (!source.isPlaying) {
                    Destroy(source);
                }
            }

            // ...then actually remove them from the list.
            playingSounds.RemoveAll((AudioSource source) => !source.isPlaying);
        }
        #endregion

        #region Public Interface
        //---------------------------------------------------------------------
        // Public Interface
        //---------------------------------------------------------------------

        ///<summary>
        /// Add a list of sounds to the manager so they can be played later. 
        ///</summary>
        public void RegisterSounds(List<Sound> sounds) {
            if (sounds == null) {
                return;
            }

            foreach (Sound s in sounds) {
                RegisterSound(s);
            }
        }

        ///<summary>
        /// Add a single sound to the manager so it can be played later.
        ///</summary>
        public void RegisterSound(Sound sound) {
            sound.Source = gameObject.AddComponent<AudioSource>();
            sound.Source.playOnAwake = false;
            
            sound.Source.clip = sound.Clip;
            sound.Source.volume = sound.Volume;
            sound.Source.pitch = sound.Pitch;

            soundTable.Add(sound.Name, sound);
        }

        ///<summary>
        /// Play a sound.
        ///</summary>
        ///<param name="soundName">The name of the sound to play.</param>
        public void Play(string soundName) {
            PlayDelayed(soundName, 0f);
        }

        ///<summary>
        /// Play a sound after a delay.
        ///</summary>
        ///<param name="soundName">The name of the sound to play.</param>
        ///<param name="delay">How long to wait before the sound plays (in seconds).</param>
        public void PlayDelayed(string soundName, float delay) {
            Sound sound;
            if (soundTable.TryGetValue(soundName, out sound)) {
                Sound copy = sound.Copy();
                copy.Delay = delay;
                copy.Reload(gameObject);
                soundQueue.Enqueue(copy);
            }
        }
        #endregion
    }
}


