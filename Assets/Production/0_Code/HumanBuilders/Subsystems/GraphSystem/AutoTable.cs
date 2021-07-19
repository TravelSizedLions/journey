using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {

  [Serializable]
  public class AutoTable<T> : IList<T> {
    private List<AutoTableEntry<T>> entries;

    public AutoTable() {
      // Debug.Log("AutoTable");
      entries = new List<AutoTableEntry<T>>();
    }

    public T this [int index] {
      get => entries[index].Value; 
      set { /* Debug.Log("set: " + value); */ entries[index].Value = value; }
    }

    public int Count { get => entries.Count; }
    public bool IsReadOnly => false;

    public void Add(T item) { Debug.Log("add item"); entries.Add(new AutoTableEntry<T>(item)); }
    public void Add(AutoTableEntry entry) { Debug.Log("add entry: " + entry); entries.Add((AutoTableEntry<T>)entry); }
    public void Add(AutoTableEntry<T> entry) { Debug.Log("add gen entry"); entries.Add(entry); }

    public void Clear() => entries.Clear();
    public bool Contains(T item) {
      foreach (var entry in entries) {
        if (entry.Value.Equals(item)) {
          return true;
        }
      }

      return false;
    }
    

    public void CopyTo(T[] array, int arrayIndex) {
      for (int i = 0; i < entries.Count; i++) {
        array[i] = entries[i].Value;
      }
    }

    public IEnumerator<T> GetEnumerator() {
      return new AutoTableEnumerator<T>(entries);
    }

    public int IndexOf(T item) {
      int index = -1;
      for (int i = 0; i < entries.Count; i++) {
        var entry = entries[i];
        if (entry.Value.Equals(item)) {
          index = i;
          break;
        }
      }

      return index;
    }

    public void Insert(int index, T item) { Debug.Log("insert"); entries.Insert(index, new AutoTableEntry<T>(item)); }
    
    public bool Remove(T item) {
      int index = -1;
      for (int i = 0; i < entries.Count; i++) {
        var entry = entries[i];
        if (entry.Value.Equals(item)) {
          index = i;
          break;
        }
      }

      if (index == -1) {
        return false;
      } else {
        entries.RemoveAt(index);
        return true;
      }
    }

    public void RemoveAt(int index) => entries.RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator() {
      return (IEnumerator) GetEnumerator();
    }
  }

  public class AutoTableEnumerator<T> : IEnumerator<T> {

    private int idx;
    private List<AutoTableEntry<T>> entries;
    public T Current { get { return entries[idx].Value; } }


    public AutoTableEnumerator(List<AutoTableEntry<T>> entries) {
      this.entries = entries;
      idx = -1;
    }

    object IEnumerator.Current { get { return Current; } }

    public void Dispose() {}

    public bool MoveNext() {
      idx++;
      return idx < entries.Count;
    }

    public void Reset() {
      idx = -1;
    }
  }
}