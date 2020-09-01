using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using System.Text;

namespace Storm.Subsystems.Save {

  
  /// <summary>
  /// A virtual file for storing information. This file is meant to represent a
  /// physical file on disk. However, no data is loaded into memory for the file
  /// until first access. The data in this folder will be stored under
  /// \<"Application.persistentDataPath"\>/gamename/slotname/folder/{data type name}.xml
  /// </summary>
  public abstract class VirtualFile {
    /// <summary>
    /// The number of items in this store.
    /// </summary>
    public abstract int Count { get; }

    /// <summary>
    /// The directory that this datastore saves out to.
    /// </summary>
    public abstract string Path { get; set; }

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
  /// A virtual file for storing information. This file is meant to represent a
  /// physical file on disk. However, no data is loaded into memory for the file
  /// until first access. The data in this folder will be stored under
  /// \<"Application.persistentDataPath"\>/gamename/slotname/folder/{data type name}.xml
  /// </summary>
  /// <typeparam name="T">The type of data to store.</typeparam>
  public class VirtualFile<T> : VirtualFile {

    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The number of items in this store.
    /// </summary>
    public override int Count { get { return entries.Count; } }

    /// <summary>
    /// The directory that this datastore saves out to.
    /// </summary>
    public override string Path { 
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
    private Dictionary<string, T> entries;

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
    public VirtualFile(string gamename, string slotname, string levelname) {
      entries = new Dictionary<string, T>();
      this.path = BuildPath(gamename, slotname, levelname);
      loaded = false;
      synchronized = false;
      serializer = new XmlSerializer(typeof(List<Entry<string, T>>));
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public VirtualFile() {
      entries = new Dictionary<string, T>();
      loaded = false;
      synchronized = false;
      serializer = new XmlSerializer(typeof(List<Entry<string, T>>));
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

      if (entries.ContainsKey(key)) {
        entries[key] = (T)value;
        synchronized = false;
      } else {
        entries.Add(key, value);
        synchronized = false;
      }
    }



    /// <summary>
    /// Gets a value from the file.
    /// </summary>
    /// <param name="key">The name of the value to get.</param>
    /// <returns>
    /// The value.
    /// </returns>
    public override dynamic Get(string key) {
      if (!loaded) {
        Load();
      }

      if (entries.ContainsKey(key)) {
          return entries[key];
      }

      throw new UnityException("The datastore for " + path + " does not contain a value for \"" + key + ".\"");
    }


    /// <summary>
    /// Gets a value from the file.
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

      if (entries.ContainsKey(key)) {
        value = entries[key];
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
      List<Entry<string, T>> entries = new List<Entry<string, T>>();

      foreach (var key in this.entries.Keys) {
        entries.Add(new Entry<string, T>(key, this.entries[key]));
      }

      if (!Directory.Exists(System.IO.Path.GetDirectoryName(path))) {
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
      }

      using (StreamWriter writer = System.IO.File.CreateText(path)) {
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
      if (System.IO.File.Exists(path)) {
        entries.Clear();

        using (StreamReader reader = System.IO.File.OpenText(path)) {
          List<Entry<string, T>> entries = (List<Entry<string, T>>)serializer.Deserialize(reader);
        
          foreach (Entry<string, T> entry in entries) {
            this.entries.Add(entry.Key, entry.Value);
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
      entries.Clear();

      loaded = false;
      synchronized = false;
    }


    /// <summary>
    /// Deletes the physical file that this virtual file saves information to.
    /// </summary>
    public override void DeleteFile() {
      if (System.IO.File.Exists(path)) {
        System.IO.File.Delete(path);
      }
    }


    public override string ToString() {
      StringBuilder builder = new StringBuilder();

      builder.AppendLine(string.Format("  - FileDictionary<{0}>:", typeof(T)));
      builder.AppendLine(string.Format("    path: {0}\n", path));

      foreach(string key in entries.Keys) {
        T value = entries[key];
        builder.AppendLine(string.Format("    key: {0}, value: {1}", key, value));
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
      path = System.IO.Path.Combine(new string [] { path, root, slotname, levelname, filename});
      path += ".xml";
      return new Uri(path).LocalPath;
    }
    #endregion
  }

}