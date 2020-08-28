using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Storm.Subsystems.Saving {

  /// <summary>
  /// A serializable class to help store dictionary information.
  /// </summary>
  /// <typeparam name="K">The type of the key.</typeparam>
  /// <typeparam name="V">The type of the value.</typeparam>
  [Serializable]
  public class DictEntry<K, V> {

    public K Key;

    public V Value;

    public DictEntry() {}

    public DictEntry(K key, V value) {
      this.Key = key;
      this.Value = value;
    }
  }

}