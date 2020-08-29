using System;
using System.Collections.Generic;
using UnityEngine;
using Dir = System.IO.Directory;
using Path = System.IO.Path;
using File = System.IO.File;
using System.Xml.Serialization;
using System.Text;

namespace Storm.Subsystems.Saving {

  /// <summary>
  /// Persistent storage for a part of the game.
  /// </summary>
  public class GameFolder {

    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    
    /// <summary>
    /// The name of the folder your game's data should be
    /// stored under. This is not a complete path, just the name of the folder
    /// you want associated with your game in the system's roaming folder.
    /// </summary>
    public string GameName { get { return gamename; } }

    /// <summary>
    /// The name of the save slot.
    /// </summary>
    public string SlotName { get { return slotname; } }

    /// <summary>
    /// The name of the folder.
    /// </summary>
    public string Name { get { return foldername; } }

    /// <summary>
    /// The unqualified path to the directory that stores the data.
    /// </summary>
    public string Directory { get { return directory; } }

    /// <summary>
    /// The number of items stored in this chunk of data.
    /// </summary>
    public int Count {
      get {
        int total = 0;

        foreach (Type type in stores.Keys) {
          total += stores[type].Count;
        }

        return total;
      }
    }

    /// <summary>
    /// The number of different types of data stored.
    /// </summary>
    public int TypeCount { get { return stores.Count; } }
    #endregion

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The name of the folder your game's data should be
    /// stored under. This is not a complete path, just the name of the folder
    /// you want associated with your game in the system's roaming folder.
    /// </summary>
    private string gamename;
    
    /// <summary>
    /// The name of the save slot.
    /// </summary>
    private string slotname;

    /// <summary>
    /// The name of the folder this datastore belongs to.
    /// </summary>
    private string foldername;

    /// <summary>
    /// The unqualified path to the directory that stores the folder data.
    /// </summary>
    private string directory;

    /// <summary>
    /// Each chunk of data saves out by data type in XML files. This is that
    /// representation in memory.
    /// </summary>
    private Dictionary<Type, FileDictionary> stores;

    #endregion

    #region Constructors 
    //-------------------------------------------------------------------------
    // Constructors
    //-------------------------------------------------------------------------

    /// <summary>
    /// Persistent storage for a particular piece of the game. This could be
    /// for a level, for the player or other character, for a Unity scene, etc.
    /// </summary>
    public GameFolder() {

    }

    /// <summary>
    /// Persistent storage for a particular piece of the game. This could be
    /// for a level, for the player or other character, for a Unity scene, etc.
    /// </summary>
    /// <param name="gamename">
    /// The name of the folder your game's data should be
    /// stored under. This is not a complete path, just the name of the folder
    /// you want associated with your game in the system's roaming folder.
    /// </param>
    /// <param name="slotname">The name of the save slot.</param>
    /// <param name="foldername">
    /// The name of the folder this datastore will save to.
    /// </param>
    public GameFolder(string gamename, string slotname, string foldername) {
      stores = new Dictionary<Type, FileDictionary>();
      this.foldername = foldername;
      this.slotname = slotname;
      this.gamename = gamename;
      directory = BuildDirectory(gamename, slotname, foldername);
    }

    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Store a particular value in memory.
    /// </summary>
    /// <param name="key">The name of the value to store.</param>
    /// <param name="value">The value to store.</param>
    /// <typeparam name="T">The value's data type.</typeparam>
    public void Set<T>(string key, T value) {
      TryCreateStore<T>();
      FileDictionary<T> store = GetStore<T>();

      store.Set(key, value);
    }


    /// <summary>
    /// Store a set of values in memory.
    /// </summary>
    /// <param name="keys">The names of the values to set.</param>
    /// <param name="values">The values to set.</param>
    /// <typeparam name="T">The data-type of the values to set.</typeparam>
    public void Set<T>(IList<string> keys, IList<T> values) {
      if (keys.Count != values.Count) {
        throw new ArgumentException("The list of keys & values must be the same length!");
      }

      TryCreateStore<T>();
      FileDictionary<T> store = GetStore<T>();

      for (int i = 0; i < keys.Count; i++) {
        store.Set(keys[i], values[i]);
      }
    }

    /// <summary>
    /// Retrieve a particular value in memory.
    /// </summary>
    /// <param name="key">The name of the value to store.</param>
    /// <typeparam name="T">The value's data type.</typeparam>
    /// <returns>The desired value.</returns>
    public T Get<T>(string key) {
      if (stores.ContainsKey(typeof(T))) {
        return GetStore<T>().Get(key);
      }

      return default(T);
    }

    /// <summary>
    /// Retrieve a particular value in memory.
    /// </summary>
    /// <param name="key">The name of the value to retrieve.</param>
    /// <param name="value">The output value.</param>
    /// <typeparam name="T">The data type of the value to get.</typeparam>
    /// <returns>True if the value was retrieved successfully. False otherwise.</returns>
    public bool Get<T>(string key, out T value) {
      dynamic dVal;
      if (stores.ContainsKey(typeof(T)) && 
          GetStore<T>().Get(key, out dVal)) {

        value = (T)dVal;
        return true;
      } 

      value = default(T);
      return false;
    }

    /// <summary>
    /// Save all data for this piece of the game out to disk.
    /// </summary>
    /// <returns>True if all data was saved successfully. False otherwise.</returns>
    public bool Save() {
      foreach (Type type in stores.Keys) {
        if (!((FileDictionary)stores[type]).Save()) {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Load all data for this piece of the game out from disk.
    /// </summary>
    /// <returns>True if all data was loaded successfully. False otherwise.</returns>
    public bool Load() {
      if (stores.Count == 0) { 
        // If the DataStore<T> instances don't exist yet, we infer them from the
        // names of the files in the directory.
        try {
          List<string> paths = GetPaths();

          foreach (string path in paths) {
            FileDictionary store = CreateStoreFromPath(path, out Type typeOfDataStored);
            store.Load();
            stores.Add(typeOfDataStored, store);
          }
        } catch(Exception e) {
          // Something went horribly wrong while trying to magically infer the data store
          // type from the file name and reflection. Go figure.
          Debug.LogWarning(e.Message);
          return false;
        }

      } else {
        foreach (Type type in stores.Keys) {
          if (!stores[type].Load()) {
            return false;
          }
        }

      }

      return true;
    }

    /// <summary>
    /// Clear all data in memory for this piece of the game.
    /// </summary>
    public void Clear() {
      foreach (Type type in stores.Keys) {
        stores[type].Clear();
      }
    }

    /// <summary>
    /// Delete the files for this piece of the game, including the folder their
    /// stored in.
    /// </summary>
    public void DeleteFolder() {
      if (Dir.Exists(directory)) {
        Dir.Delete(directory, true);
      }
    }

    /// <summary>
    /// Gets the file paths that data for this piece of the game are stored in.
    /// </summary>
    /// <returns>The list of fully qualified file paths for this piece of the game.</returns>
    public List<string> GetPaths() {
      if (Dir.Exists(directory)) {
        string[] paths = Dir.GetFiles(directory);

        return new List<string>(paths);
      }

      return new List<string>();
    }

    /// <summary>
    /// String representation of the data for this piece of the game.
    /// </summary>
    public override string ToString() {
      StringBuilder builder = new StringBuilder();
      builder.AppendLine(new String('-', 80));
      builder.AppendLine(string.Format("Data for \"{0}\"", foldername));
      builder.AppendLine(new String('-', 80));

      foreach (Type t in stores.Keys) {
        builder.AppendLine(stores[t].ToString());
      }

      return builder.ToString();
    } 
    #endregion

    #region Helper Methods

    /// <summary>
    /// Gets a file dictionary  of a given type from the list of stores.
    /// </summary>
    /// <typeparam name="T">The data type for the file dictionary.</typeparam>
    /// <returns>The data store for type T.</returns>
    private FileDictionary<T> GetStore<T>() {
      return (FileDictionary<T>)stores[typeof(T)];
    }

    /// <summary>
    /// Constructs a file dictionary from the given path.
    /// </summary>
    /// <param name="path">The path to the data.</param>
    /// <param name="typeOfStore">The type T of <see cref="FileDictionary<T>" /> that was created.</param>
    /// <returns>The constructed <see cref="FileDictionary<T>" /> </returns>
    private FileDictionary CreateStoreFromPath(string path, out Type typeOfStore) {

      // In order to construct a generic type from a Type variable, reflection
      // is needed.
      string unqualifiedTypeName = Path.GetFileNameWithoutExtension(path);

      // Certain Unity data types don't play nice with serialization, so we
      // need to check for them explicitly.
      typeOfStore = DecideType(unqualifiedTypeName);

      // Get the "Base" generic type.
      Type baseType = typeof(FileDictionary<>);

      // Construct the concrete type using the type of data stored and the
      // base generic type.
      Type constructed = baseType.MakeGenericType(new Type[] { typeOfStore });
      
      // Create an instance of the concrete type equivalent to
      // calling new FileDictionary<T>().
      FileDictionary store = (FileDictionary)Activator.CreateInstance(constructed);
      
      // Unfortunately, this method requires an empty constructor, so path
      // needs to be set separately.
      store.FilePath = path;

      return store;
    }

    /// <summary>
    /// Converts an unqualified type name into its corresponding type.
    /// </summary>
    /// <param name="typeName">The unqualified type name.</param>
    /// <returns>The converted type.</returns>
    private Type DecideType(string typeName) {
      switch(typeName) {
        case "UnityEngine.Vector2":
          return typeof(Vector2);
        case "UnityEngine.Vector3":
          return typeof(Vector3);
        default:
          return Type.GetType(typeName);
      }
    }

    /// <summary>
    /// Creates a DataStore<> of type T, if it doesn't already exist for this
    /// piece of the game.
    /// </summary>
    /// <typeparam name="T">The type of data that the DataStore should store.</typeparam>
    private void TryCreateStore<T>() {
      Type t = typeof(T);
      if (!stores.ContainsKey(t)) {
        stores.Add(t, new FileDictionary<T>(gamename, slotname, foldername));
      }
    }

    /// <summary>
    /// Builds the unqualified path to the directory for this data.
    /// </summary>
    /// <param name="gamename">The name of the game this piece of data storage belongs to.</param>
    /// <param name="slotname">The name of the save slot this piece of data storage belongs to.</param>
    /// <param name="foldername">The name of the piece of data storage.</param>
    /// <returns>The unqualified path to the directory for this piece of data storage's data.</returns>
    private string BuildDirectory(string gamename, string slotname, string foldername) {
      return Path.Combine(new string[] {
        Application.persistentDataPath,
        gamename,
        slotname,
        foldername
      });
    }
    #endregion
  }
}