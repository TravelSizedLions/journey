using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace HumanBuilders {

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
    /// Default dropdown value for sound selection.
    /// </summary>
    public const string DEFAULT_SOUND = "None";

    /// <summary>
    /// A map of sound names to sounds.
    /// </summary>
    private static Dictionary<string, Sound> soundTable;

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
    private void Update() {
      if (soundQueue.Count > 0) {
        Sound sound = soundQueue.Dequeue();
        playingSounds.Add(sound.Source);

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
    public static void RegisterSounds(List<Sound> sounds) => Instance.RegisterSounds_Inner(sounds);
    private void RegisterSounds_Inner(List<Sound> sounds) {
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
    public static void RegisterSound(Sound sound) => Instance.RegisterSound_Inner(sound);
    private void RegisterSound_Inner(Sound sound) {
      sound.Source = Instance.gameObject.AddComponent<AudioSource>();
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
    public static void Play(string soundName) => Instance.PlayDelayed_Inner(soundName, 0);

    ///<summary>
    /// Play a sound after a delay.
    ///</summary>
    ///<param name="soundName">The name of the sound to play.</param>
    ///<param name="delay">How long to wait before the sound plays (in seconds).</param>
    public static void PlayDelayed(string soundName, float delay) => Instance.PlayDelayed_Inner(soundName, delay);
    private void PlayDelayed_Inner(string soundName, float delay) {
      if (soundName.Contains("/")) {
        string[] pieces = soundName.Split('/');
        soundName = pieces[pieces.Length-1];
      }

      Sound sound;
      if (soundTable.TryGetValue(soundName, out sound)) {
        Sound copy = sound.Copy();
        copy.Delay = delay;
        copy.Reload(gameObject);
        soundQueue.Enqueue(copy);
      } else {
        Debug.LogWarning("Can't find sound \"" + soundName + ".\" Double check that there's SoundLibrary added to your scene, and that the desired sound clip is added to it.");
      }
    }
    #endregion


    /// <summary>
    /// Finds all sounds in a scene, and lists them by library.
    /// </summary>
    public static List<string> FindSoundsInScene() {
      List<string> names = new List<string>();
      names.Add(DEFAULT_SOUND);

      SoundLibrary[] sounds = GameObject.FindObjectsOfType<SoundLibrary>();
      foreach (SoundLibrary list in sounds) {
        string category = !string.IsNullOrWhiteSpace(list.Category) ? list.Category + "/" : "";
        
        foreach (Sound sound in list.Sounds) {
          string name = sound.Name;

          names.Add(category + name);
        }
      }

      return names;
    }
  }
}