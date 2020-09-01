using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Storm.Subsystems.VSave {

  /// <summary>
  /// A serializable class to help store information.
  /// </summary>
  /// <typeparam name="K">The type of the key.</typeparam>
  /// <typeparam name="V">The type of the value.</typeparam>
  [Serializable]
  public class Entry<K, V> {

    /// <summary>
    /// The key.
    /// </summary>
    public K Key;

    /// <summary>
    /// The value.
    /// </summary>
    public V Value;

    public Entry() {}

    public Entry(K key, V value) {
      this.Key = key;
      this.Value = value;
    }
  }

}