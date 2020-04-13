using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Extensions;

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
    ///
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


        private Dictionary<string, Sound> soundTable;

        [SerializeField]
        private Queue<Sound> soundQueue;

        [SerializeField]
        private List<AudioSource> playingSounds;

        [SerializeField]
        private AudioSource currentSource;

        private int sourceCount;


        public override void Awake() {
            base.Awake();


            soundQueue = new Queue<Sound>();
            soundTable = new Dictionary<string, Sound>();
        }

        public void RegisterSounds(List<Sound> sounds) {
            if (sounds == null) {
                return;
            }

            foreach (Sound s in sounds) {
                RegisterSound(s);
            }
        }

        public void RegisterSound(Sound sound) {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.playOnAwake = false;
            
            sound.source.clip = sound.Clip;
            sound.source.volume = sound.Volume;
            sound.source.pitch = sound.Pitch;

            soundTable.Add(sound.Name, sound);
        }

        public void FixedUpdate() {
            if (soundQueue.Count > 0) {
                Sound sound = soundQueue.Dequeue();
                playingSounds.Add(sound.source);
                
                Debug.Log(sound.Delay);
                
                sound.source.PlayDelayed(sound.Delay);
            }

            List<AudioSource> stillPlaying = new List<AudioSource>();


            playingSounds.RemoveAll((AudioSource source) => !source.isPlaying);


        }

        public void Play(string soundName) {
            PlayDelayed(soundName, 0f);
        }


        public void PlayDelayed(string soundName, float delay) {
            Sound sound;
            if (soundTable.TryGetValue(soundName, out sound)) {
                Sound copy = sound.Copy();
                copy.Delay = delay;
                copy.Reload(gameObject);
                soundQueue.Enqueue(copy);
            }
        }
    }
}


