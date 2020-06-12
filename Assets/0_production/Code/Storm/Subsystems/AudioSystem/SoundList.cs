using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Subsystems.Audio {

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
  public class SoundList : MonoBehaviour {

    #region Variables
    /// <summary>
    /// The name of the list.
    /// </summary>
    public String Category;

    /// <summary>
    /// The colection of sounds.
    /// </summary>
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
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    ///<summary>
    /// Fires before the first frame is rendered.
    ///</summary>
    private void Start() {
      AudioManager.Instance.RegisterSounds(Sounds);
    }

    #endregion
  }
}