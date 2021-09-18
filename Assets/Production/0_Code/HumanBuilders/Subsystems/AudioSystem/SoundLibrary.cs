using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {

  ///<summary>
  /// A group of related sounds. Whenever you want to make a set of related sounds,
  /// add a SoundList component onto the AudioManager in your scene and add your sounds there.
  ///</summary>
  ///<remarks>
  /// This class is intended to help keep related sounds organized
  /// and attached to the AudioManager. For example, one could add
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

    /// <summary>
    /// The colection of sounds.
    /// </summary>
    [TableList(AlwaysExpanded=true)]
    public List<Sound> Sounds;

    ///<summary>
    /// Index operator
    ///</summary>
    public Sound this [int index] {
      get { return Sounds[index]; }
    }

    ///<summary>
    /// The number of sounds in the collection.
    ///</summary>
    public int Count {
      get { return Sounds.Count; }
    }

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    ///<summary>
    /// Fires before the first frame is rendered.
    ///</summary>
    private void OnEnable() {
      if (Application.isPlaying || Application.isEditor) {
        AudioManager.RegisterSounds(Sounds);
      }
    }
  }
}