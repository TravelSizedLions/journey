using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace HumanBuilders {

  ///<summary>
  /// A group of related sounds. Whenever you want to make a set of related sounds, create
  /// a Sound Libary asset and attach audio clips to it. 
  ///</summary>
  ///<remarks>
  /// This class is intended to help keep related sounds organized. 
  /// For example, one could add
  /// a collection of explosion sounds that could be played randomly
  /// whenever an enemy dies.
  ///</remarks>
  [Serializable]
  [CreateAssetMenu(fileName = "New Sound Library", menuName = "Sound/Library")]
  public class SoundLibrary : ScriptableObject {

    /// <summary>
    /// The name of the library.
    /// </summary>
    [Tooltip("The type of sounds contained in this library.")]
    public String Category;

    public AudioMixerGroup Mixer;

    /// <summary>
    /// The colection of sounds.
    /// </summary>
    [TableList(AlwaysExpanded=true)]
    public List<Sound> Sounds;

    ///<summary>
    /// Index operator
    ///</summary>
    public Sound this [int index] {
      get { 
        var sound = Sounds[index];
        sound.Mixer = Mixer;
        return sound;
      }
    }

    public Sound this [string name] {
      get {
        foreach (var sound in Sounds) {
          if (sound.Clip.name == name) {
            sound.Mixer = Mixer;
            return sound;
          }
        }

        Debug.LogWarning(String.Format("Could not find sound \"{0}\" in library \"{}\"", name, Category));
        return null;
      }
    }

    ///<summary>
    /// The number of sounds in the collection.
    ///</summary>
    public int Count {
      get { return Sounds.Count; }
    }
  }
}