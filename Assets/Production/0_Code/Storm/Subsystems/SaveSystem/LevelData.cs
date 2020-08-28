using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using System.Text;

namespace Storm.Subsystems.Saving {

  /// <summary>
  /// Persistent storage for a particular level of the game.
  /// </summary>
  public class LevelData {

    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------

    /// <summary>
    /// The name of the level.
    /// </summary>
    public string LevelName { get { return levelname; } }


    /// <summary>
    /// The number of items stored in this level.
    /// </summary>
    public int Count {
      get {
        int total = 0;

        foreach (Type type in stores.Keys) {
          total += counts[type];
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
    /// The name of the level this datastore belongs to.
    /// </summary>
    private string levelname;

    /// <summary>
    /// Each level saves its data by data type in XML files. This is that
    /// representation in memory.
    /// </summary>
    private Dictionary<Type, DataStore> stores;


    /// <summary>
    /// Tracks the number of items of each type in the level.
    /// </summary>
    private Dictionary<Type, int> counts;

    #endregion

    #region Constructors 
    //-------------------------------------------------------------------------
    // Constructors
    //-------------------------------------------------------------------------

    /// <summary>
    /// Persistent storage for a particular level of the game.
    /// </summary>
    public LevelData() {

    }

    /// <summary>
    /// Persistent storage for a particular level of the game.
    /// </summary>
    /// <param name="gamename">
    /// The name of the folder your game's data should be
    /// stored under. This is not a complete path, just the name of the folder
    /// you want associated with your game in the system's roaming folder.
    /// </param>
    /// <param name="slotname">The name of the save slot.</param>
    /// <param name="levelname">The name of the level this datastore belongs to.</param>
    public LevelData(string gamename, string slotname, string levelname) {
      stores = new Dictionary<Type, DataStore>();
      counts = new Dictionary<Type, int>();
      this.levelname = levelname;
      this.slotname = slotname;
      this.gamename = gamename;
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
      Type t = typeof(T);
      if (!stores.ContainsKey(t)) {
        stores.Add(t, new DataStore<T>(gamename, slotname, levelname));
        counts.Add(t, 0);
      }

      DataStore<T> store = GetStore<T>();

      store.Set(key, value);
      counts[t] = store.Count;
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
    /// Save all level data out to disk.
    /// </summary>
    /// <returns>True if all data was saved successfully. False otherwise.</returns>
    public bool Save() {
      foreach (Type type in stores.Keys) {
        if (!((DataStore)stores[type]).Save()) {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Load all level data out from disk.
    /// </summary>
    /// <returns>True if all data was loaded successfully. False otherwise.</returns>
    public bool Load() {
      if (stores.Count == 0) { 
        // If the DataStore<T> instances don't exist yet, we infer them from the
        // names of the files in the level directory.
        try {
          List<string> paths = GetPaths();

          foreach (string path in paths) {
            DataStore store = CreateStoreFromPath(path, out Type typeOfDataStored);
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
    /// Clear all level data in memory.
    /// </summary>
    public void Clear() {
      foreach (Type type in stores.Keys) {
        stores[type].Clear();
      }
    }

    /// <summary>
    /// Delete the files for this level.
    /// </summary>
    public void DeleteFiles() {
      if (stores.Count > 0) {
        foreach (Type type in stores.Keys) {
          stores[type].DeleteFile();
        }
      } else {
        List<string> paths = GetPaths();
        foreach(string path in paths) {
          File.Delete(path);
        }
      }
    }

    /// <summary>
    /// Gets the file paths that data for this level are stored in.
    /// </summary>
    /// <returns>The list of fully qualified file paths for the level.</returns>
    public List<string> GetPaths() {
      string directory = Path.Combine(new string[] {
        Application.persistentDataPath,
        gamename,
        slotname,
        levelname
      });

      string[] paths = Directory.GetFiles(directory);

      return new List<string>(paths);
    }

    /// <summary>
    /// String representation of the data for the level.
    /// </summary>
    /// <returns></returns>
    public override string ToString() {
      StringBuilder builder = new StringBuilder();
      builder.AppendLine(new String('-', 80));
      builder.AppendLine(string.Format("Level Data for \"{0}\"", levelname));
      builder.AppendLine(new String('-', 80));

      foreach (Type t in stores.Keys) {
        builder.AppendLine(stores[t].ToString());
      }

      return builder.ToString();
    } 
    #endregion

    #region Helper Methods

    /// <summary>
    /// Gets a datastore of a given type from the list of stores.
    /// </summary>
    /// <typeparam name="T">The data type for the store.</typeparam>
    /// <returns>The data store for type T.</returns>
    private DataStore<T> GetStore<T>() {
      return (DataStore<T>)stores[typeof(T)];
    }

    /// <summary>
    /// Constructs a datastore from the given path.
    /// </summary>
    /// <param name="path">The path to the data.</param>
    /// <param name="typeOfStore">The type T of <see cref="DataStore<T>" /> that was created.</param>
    /// <returns>The constructed <see cref="DataStore<T>" /> </returns>
    private DataStore CreateStoreFromPath(string path, out Type typeOfStore) {

      // In order to construct a generic type from a Type variable, reflection
      // is needed.
      string unqualifiedTypeName = Path.GetFileNameWithoutExtension(path);

      // Certain Unity data types don't play nice with serialization, so we
      // need to check for them explicitly.
      typeOfStore = DecideType(unqualifiedTypeName);

      // Get the "Base" generic type.
      Type baseType = typeof(DataStore<>);

      // Construct the concrete type using the type of data stored and the
      // base generic type.
      Type constructed = baseType.MakeGenericType(new Type[] { typeOfStore });
      
      // Create an instance of the concrete type equivalent to
      // calling new DataStore<T>().
      DataStore store = (DataStore)Activator.CreateInstance(constructed);
      
      // Unfortunately, this method requires an empty constructor, so path
      // needs to be set separately.
      store.FilePath = path;

      return store;
    }

    private Type DecideType(string unqualifiedTypeName) {
      switch(unqualifiedTypeName) {
        case "UnityEngine.Vector2":
          return typeof(Vector2);
        case "UnityEngine.Vector3":
          return typeof(Vector3);
        default:
          return Type.GetType(unqualifiedTypeName);
      }
    }
    #endregion
  }
}