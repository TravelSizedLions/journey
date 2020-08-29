using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using System.Text;

namespace Storm.Subsystems.Saving {

  
  public abstract class FileDictionary {
    /// <summary>
    /// The number of items in this store.
    /// </summary>
    public abstract int Count { get; }

    /// <summary>
    /// The directory that this datastore saves out to.
    /// </summary>
    public abstract string FilePath { get; set; }

    /// <summary>
    /// Whether or not the data has already been loaded from disk.
    /// </summary>
    public abstract bool Loaded { get; }

    /// <summary>
    /// Whether or not the data in memory is in sync with the data saved on disk.
    /// </summary>
    public abstract bool Synchronized { get; }

    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Set a value in the store.
    /// </summary>
    /// <param name="key">The name of the value.</param>
    /// <param name="value">The data to store.</param>
    public abstract void Set(string key, dynamic value);

    /// <summary>
    /// Gets a name of the value to get.
    /// </summary>
    /// <param name="key">The name of the value.</param>
    /// <typeparam name="T">The type of the value to get.</typeparam>
    /// <returns>The value.</returns>
    public abstract dynamic Get(string key);

    /// <summary>
    /// Gets a name of the value to get.
    /// </summary>
    /// <param name="key">The name of the value to get.</param>
    /// <param name="value">The output value.</param>
    /// <returns>
    /// True if the value was successfully retrieved. False otherwise.
    /// </returns>
    public abstract bool Get(string key, out dynamic value);

    /// <summary>
    /// Save the data in this store.
    /// </summary>
    /// <returns>True if the data was saved successfully. False otherwise.</returns>
    public abstract bool Save();

    /// <summary>
    /// Load the data for this store.
    /// </summary>
    /// <returns>True if the data was loaded successfully. False otherwise.</returns>
    public abstract bool Load();

    /// <summary>
    /// Clears the datastore in memory.
    /// </summary>
    public abstract void Clear();

    /// <summary>
    /// Deletes the file that this datastore saves information to.
    /// </summary>
    public abstract void DeleteFile();
  }

  /// <summary>
  /// Persistent storage for a particular type of data.
  /// The data in this dictionary 
  /// will be stored under
  /// \<"Application.persistentDataPath"\>/gamename/slotname/folder/{data type name}.xml
  /// </summary>
  /// <typeparam name="T">The type of data to store.</typeparam>
  public class FileDictionary<T> : FileDictionary {

    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The number of items in this store.
    /// </summary>
    public override int Count { get { return store.Count; } }

    /// <summary>
    /// The directory that this datastore saves out to.
    /// </summary>
    public override string FilePath { 
      get { return path; } 
      set { path = value; }
    }

    /// <summary>
    /// Whether or not the data has already been loaded from disk.
    /// </summary>
    public override bool Loaded { get { return loaded; } }

    /// <summary>
    /// Whether or not the data in memory is in sync with the data saved on disk.
    /// </summary>
    public override bool Synchronized { get { return synchronized; } }
    #endregion

    #region Fields 
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The data.
    /// </summary>
    private Dictionary<string, T> store;

    /// <summary>
    /// The directory that this datastore saves out to.
    /// </summary>
    private string path;

    /// <summary>
    /// Whether or not the data has already been loaded from disk.
    /// </summary>
    private bool loaded;


    /// <summary>
    /// Whether or not the data in memory is in sync with the data saved on disk.
    /// </summary>
    private bool synchronized;

    /// <summary>
    /// XML serializer used to load & save data.
    /// </summary>
    private XmlSerializer serializer;
    #endregion

    #region Constructors
    //-------------------------------------------------------------------------
    // Constructors
    //-------------------------------------------------------------------------

    /// <summary>
    /// Persistent storage for a particular type of data.
    /// </summary>
    /// <param name="gamename">The name of the folder your game's data should be
    /// stored under. This is not a complete path, just the name of the folder
    /// you want associated with your game in the system's roaming folder.</param>
    /// <param name="slotname">The name of the save slot.</param>
    /// <param name="levelname">The name of the level this datastore belongs to.</param>
    public FileDictionary(string gamename, string slotname, string levelname) {
      store = new Dictionary<string, T>();
      this.path = BuildPath(gamename, slotname, levelname);
      loaded = false;
      synchronized = false;
      serializer = new XmlSerializer(typeof(List<DictEntry<string, T>>));
    }

    /// <summary>
    /// Persistent storage for a particular type of data.
    /// </summary>
    public FileDictionary() {
      store = new Dictionary<string, T>();
      path = "";
      synchronized = false;
      serializer = new XmlSerializer(typeof(List<DictEntry<string, T>>));
    }

    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Set a value in the store.
    /// </summary>
    /// <param name="key">The name of the value.</param>
    /// <param name="value">The data to store.</param>
    public override void Set(string key, dynamic value) {
      if (!loaded) {
        Load();
      }

      if (store.ContainsKey(key)) {
        store[key] = (T)value;
        synchronized = false;
      } else {
        store.Add(key, value);
        synchronized = false;
      }
    }



    /// <summary>
    /// Gets a name of the value to get.
    /// </summary>
    /// <param name="key">The name of the value to get.</param>
    /// <returns>
    /// The value.
    /// </returns>
    public override dynamic Get(string key) {
      if (!loaded) {
        Load();
      }

      if (store.ContainsKey(key)) {
          return store[key];
      }

      throw new UnityException("The datastore for " + path + " does not contain a value for \"" + key + ".\"");
    }


    /// <summary>
    /// Gets a name of the value to get.
    /// </summary>
    /// <param name="key">The name of the value to get.</param>
    /// <param name="value">The output value.</param>
    /// <returns>
    /// True if the value was successfully retrieved. False otherwise.
    /// </returns>
    public override bool Get(string key, out dynamic value) {
      if (!loaded) {
        Load();
      }

      if (store.ContainsKey(key)) {
        value = store[key];
        return true;
      }

      value = default(T);
      return false;
    }


    /// <summary>
    /// Save the data in this store.
    /// </summary>
    /// <returns>True if the data was saved successfully. False otherwise.</returns>
    public override bool Save() {
      List<DictEntry<string, T>> entries = new List<DictEntry<string, T>>();

      foreach (var key in store.Keys) {
        entries.Add(new DictEntry<string, T>(key, store[key]));
      }

      if (!Directory.Exists(Path.GetDirectoryName(path))) {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
      }

      using (StreamWriter writer = File.CreateText(path)) {
        serializer.Serialize(writer, entries);
      }

      synchronized = true;
      return true;
    }

    /// <summary>
    /// Load the data for this store.
    /// </summary>
    /// <returns>True if the data was loaded successfully. False otherwise.</returns>
    public override bool Load() {
      if (File.Exists(path)) {
        store.Clear();

        using (StreamReader reader = File.OpenText(path)) {
          List<DictEntry<string, T>> entries = (List<DictEntry<string, T>>)serializer.Deserialize(reader);
        
          foreach (DictEntry<string, T> entry in entries) {
            store.Add(entry.Key, entry.Value);
          }

          loaded = true;
          synchronized = true;
          return true;
        }
      } else {
        return false;
      }
    }


    /// <summary>
    /// Clears the datastore in memory.
    /// </summary>
    public override void Clear() {
      store.Clear();

      loaded = false;
      synchronized = false;
    }


    /// <summary>
    /// Deletes the file that this datastore saves information to.
    /// </summary>
    public override void DeleteFile() {
      if (File.Exists(path)) {
        File.Delete(path);
      }
    }


    public override string ToString() {
      StringBuilder builder = new StringBuilder();

      builder.AppendLine(string.Format("DataStore<{0}>:", typeof(T)));
      builder.AppendLine(string.Format("\tpath: {0}\n", path));

      foreach(string key in store.Keys) {
        T value = store[key];
        builder.AppendLine(string.Format("\tkey: {0}, value: {1}", key, value));
      }

      return builder.ToString();
    }

    #endregion

    #region Helper Methods
    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------

    /// <summary>
    /// Constructs the path used to save this datastore.
    /// </summary>
    /// <param name="gamename">
    /// The name of the folder your game's data should be
    /// stored under. This is not a complete path, just the name of the folder
    /// you want associated with your game in the system's roaming folder.
    /// </param>
    /// <param name="slotname">The name of the save slot.</param>
    /// <param name="levelname">The name of the level this datastore belongs to.</param>
    /// <returns>The unqualified path to save this datastore.</returns>
    private string BuildPath(string root, string slotname, string levelname) {
      string path = Application.persistentDataPath;

      string filename = typeof(T).ToString();
      path = Path.Combine(new string [] {path, root, slotname, levelname, filename});
      path += ".xml";
      return new Uri(path).LocalPath;
    }
    #endregion
  }

}