using System;
using System.Collections.Generic;
using UnityEngine;
using Dir = System.IO.Directory;
using File = System.IO.File;
using Path = System.IO.Path;

namespace Storm.Subsystems.Saving {

  /// <summary>
  /// Data for a single save file.
  /// </summary>
  public class SaveFile {

    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------   

    /// <summary>
    /// The name of this save file.
    /// </summary>
    public string Name { get { return slotname; } }

    /// <summary>
    /// The name of the game this save file belongs to.
    /// </summary>
    public string GameName { get { return gamename; } }

    /// <summary>
    /// The directory that holds the data for this save file.
    /// </summary>
    public string Directory { get { return directory; } }

    /// <summary>
    /// The number of Folders stored on this save file.
    /// </summary>
    public int FolderCount { get { return levels.Count; }}

    /// <summary>
    /// The total number of values stored in this save file.
    /// </summary>
    public int DataCount { 
      get { 
        int total = 0;
        foreach (string level in levels.Keys) {
          total += levels[level].Count;
        }

        return total;
      }
    }
    #endregion

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The name of this save file.
    /// </summary>
    private string slotname;

    /// <summary>
    /// The name of the game this save slot belongs to.
    /// </summary>
    private string gamename;

    /// <summary>
    /// The directory that holds the data for this save file.
    /// </summary>
    private string directory;
    
    /// <summary>
    /// A map of level names to each level's data.
    /// </summary>
    private Dictionary<string, GameFolder> levels;
    #endregion

    #region Constructors
    //-------------------------------------------------------------------------
    // Constructors
    //-------------------------------------------------------------------------

    /// <summary>
    /// Data for a single save file.
    /// </summary>
    public SaveFile() {
      slotname = "player_1";
      gamename = "game_data";
      levels = new Dictionary<string, GameFolder>();
    }

    /// <summary>
    /// Data for a single save file.
    /// </summary>
    /// <param name="gamename">The name of the game this save slot belongs to.</param>
    /// <param name="slotname">The name of this save file.</param>
    public SaveFile(string gamename, string slotname) {
      this.gamename = gamename;
      this.slotname = slotname;
      directory = BuildDirectory(gamename, slotname);
      levels = new Dictionary<string, GameFolder>();
    }

    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Set a value in a given level.
    /// </summary>
    /// <param name="level">The level to set data to.</param>
    /// <param name="key">The name of the value to set.</param>
    /// <param name="value">The value to set.</param>
    /// <typeparam name="T">The type of the data to set.</typeparam>
    public void Set<T>(string level, string key, T value) {
      levels[level].Set(key, value);
    }


    /// <summary>
    /// Set a list of values in a level.
    /// </summary>
    /// <param name="level">The level to set data for.</param>
    /// <param name="keys">The names of the values to set.</param>
    /// <param name="values">The values to set.</param>
    /// <typeparam name="T">The type of the data to set.</typeparam>
    public void Set<T>(string level, IList<string> keys, IList<T> values) {
      levels[level].Set(keys, values);
    }


    /// <summary>
    /// Get a specific value from a level.
    /// </summary>
    /// <param name="level">The level to get data for.</param>
    /// <param name="key">The name of the value to get.</param>
    /// <param name="value">The output value.</param>
    /// <typeparam name="T">The type of data to get.</typeparam>
    /// <returns>True if the value was retrieved successfully. False otherwise.</returns>
    public bool Get<T>(string level, string key, out T value) {
      return levels[level].Get(key, out value);
    }


    /// <summary>
    /// Get a list of values from a level.
    /// </summary>
    /// <param name="level">The level to get values from.</param>
    /// <param name="keys">The names of the values to get.</param>
    /// <param name="values">The output list of values.</param>
    /// <typeparam name="T">The type of data to get.</typeparam>
    /// <returns>True if all values were retrieved successfully. False otherwise.</returns>
    public bool Get<T>(string level, IList<string> keys, out List<T> values) {
      if (!levels.ContainsKey(level)) {
        values = null;
        return false;
      }

      values = new List<T>();

      foreach (string key in keys) {
        if (levels[level].Get(key, out T value)) {
          values.Add(value);
        } else {
          values = null;
          return false;
        }
      }

      return true;
    }


    /// <summary>
    /// Get a list of values from a level.
    /// </summary>
    /// <param name="level">The level to get values from.</param>
    /// <param name="keys">The names of the values to get.</param>
    /// <param name="values">The output list of values.</param>
    /// <typeparam name="T">The type of data to get.</typeparam>
    /// <returns>True if all values were retrieved successfully. False otherwise.</returns>
    public bool Get<T>(string level, IList<string> keys, out T[] values) {
      if (!levels.ContainsKey(level)) {
        values = null;
        return false;
      }

      List<T> v = new List<T>();

      foreach (string key in keys) {
        if (levels[level].Get(key, out T value)) {
          v.Add(value);
        } else {
          values = null;
          return false;
        }
      }

      values = v.ToArray();
      return true;
    }


    /// <summary>
    /// Get a value from a particular level.
    /// </summary>
    /// <param name="level">The level to get data for.</param>
    /// <param name="key">The name of the value to get.</param>
    /// <typeparam name="T">The data type of the value to get.</typeparam>
    /// <returns>The value.</returns>
    public T Get<T>(string level, string key) {
      return levels[level].Get<T>(key);
    }


    /// <summary>
    /// Get a list of values from a level.
    /// </summary>
    /// <param name="level">The level to get values from.</param>
    /// <param name="keys">The names of the values to get.</param>
    /// <typeparam name="T">The type of data to get.</typeparam>
    /// <returns>The list of values.</returns>
    public List<T> Get<T>(string level, IEnumerable<string> keys) {
      List<T> values = new List<T>();

      foreach (string key in keys) {
        values.Add(levels[level].Get<T>(key));
      }

      return values;
    }

    /// <summary>
    /// Saves all game data out to disk.
    /// </summary>
    /// <returns>True if all levels saved successfully. False otherwise.</returns>
    public bool Save() {
      foreach (string levelname in levels.Keys) {
        if (!SaveLevel(levelname)) {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Saves data for a single level out to disk.
    /// </summary>
    /// <param name="levelname">The level to save.</param>
    /// <returns>True if the level saved successfully. False otherwise.</returns>
    public bool SaveLevel(string levelname) {
      if (levels.ContainsKey(levelname)) {
        return levels[levelname].Save();
      }

      return false;
    }

    /// <summary>
    /// Loads all game data from disk.
    /// </summary>
    public bool Load() {
      foreach(string levelname in levels.Keys) {
        if (!LoadLevel(levelname)) {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Loads data for a single level of the game.
    /// </summary>
    /// <param name="levelname">The name of the level to load.</param>
    public bool LoadLevel(string levelname) {
      if (levels.ContainsKey(levelname)) {
        return levels[levelname].Load();
      }

      return false;
    }

    /// <summary>
    /// Register a new level in the save file.
    /// </summary>
    /// <param name="levelname">The name of the level to register.</param>
    /// <returns>True if the level was added successfully. False otherwise.</returns>
    public bool RegisterLevel(string levelname) {
      if (!levels.ContainsKey(levelname)) {
        levels.Add(levelname, new GameFolder(gamename, slotname, levelname));
        return true;
      }

      return false;
    }


    /// <summary>
    /// Clear all data in memory for this save file.
    /// </summary>
    public void Clear() {
      foreach (string name in levels.Keys) {
        ClearLevel(name);
      }
    }

    /// <summary>
    /// Clear all data in memory for a specific level.
    /// </summary>
    /// <param name="levelname">The name of the level to clear.</param>
    public void ClearLevel(string levelname) {
      levels[levelname].Clear();
    }

    /// <summary>
    /// Delete the save file.
    /// </summary>
    public void DeleteFolder() {
      if (Dir.Exists(directory)) {
        Dir.Delete(directory, true);
      }
    }

    /// <summary>
    /// Delete all save data for a specific level.
    /// </summary>
    /// <param name="levelname">The name of the level to delete.</param>
    public void DeleteLevelFolder(string levelname) {
      levels[levelname].DeleteFolder();
    }
    #endregion


    #region Helper Methods

    /// <summary>
    /// Builds the unqualified path to the directory for this save file's data.
    /// </summary>
    /// <param name="gamename">The name of the game this level belongs to.</param>
    /// <param name="slotname">The name of the save slot this level belongs to.</param>
    /// <returns>The unqualified path to the directory for this save file's data.</returns>
    private string BuildDirectory(string gamename, string slotname) {
      return Path.Combine(new string[] {
        Application.persistentDataPath,
        gamename,
        slotname
      });
    }
    #endregion
  }
}