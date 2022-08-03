using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using D = System.IO.Directory;
using P = System.IO.Path;
using F = System.IO.File;

namespace HumanBuilders {

  /// <summary>
  /// Data for a player's save file.
  /// </summary>
  public class SaveSlot {

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
    public string Path { get { return path; } }

    /// <summary>
    /// The number of Folders stored on this save file.
    /// </summary>
    public int FolderCount { get { return folders.Count; }}

    /// <summary>
    /// The total number of values stored in this save file.
    /// </summary>
    public int DataCount { 
      get { 
        int total = 0;
        foreach (string level in folders.Keys) {
          total += folders[level].Count;
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
    private string path;
    
    /// <summary>
    /// A map of level names to each level's data.
    /// </summary>
    private Dictionary<string, VirtualFolder> folders;
    #endregion

    #region Constructors
    //-------------------------------------------------------------------------
    // Constructors
    //-------------------------------------------------------------------------

    /// <summary>
    /// Data for a single save file.
    /// </summary>
    public SaveSlot() {
      slotname = "player_1";
      gamename = "game_data";
      folders = new Dictionary<string, VirtualFolder>();
    }

    /// <summary>
    /// Data for a single save file.
    /// </summary>
    /// <param name="gamename">The name of the game this save slot belongs to.</param>
    /// <param name="slotname">The name of this save file.</param>
    public SaveSlot(string gamename, string slotname) {
      this.gamename = gamename;
      this.slotname = slotname;
      folders = new Dictionary<string, VirtualFolder>();
      this.path = P.Combine(new string[] {Application.persistentDataPath, gamename, slotname });
      BuildDirectory(this.path);
    }

    public SaveSlot(string path) {
      folders = new Dictionary<string, VirtualFolder>();
      this.path = path;
      BuildDirectory(path);
    }

    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Set a value in a given level.
    /// </summary>
    /// <param name="folder">The folder to set data to.</param>
    /// <param name="key">The name of the value to set.</param>
    /// <param name="value">The value to set.</param>
    /// <typeparam name="T">The type of the data to set.</typeparam>
    public void Set<T>(string folder, string key, T value) {
      if (!folders.ContainsKey(folder)) {
        folders.Add(folder, new VirtualFolder(P.Combine(new string[] {this.path, folder})));
      }

      folders[folder].Set(key, value);

    }


    /// <summary>
    /// Set a list of values in a folder.
    /// </summary>
    /// <param name="folder">The folder to set data for.</param>
    /// <param name="keys">The names of the values to set.</param>
    /// <param name="values">The values to set.</param>
    /// <typeparam name="T">The type of the data to set.</typeparam>
    public void Set<T>(string folder, IList<string> keys, IList<T> values) {
      if (!folders.ContainsKey(folder)) {
        folders.Add(folder, new VirtualFolder(P.Combine(new string[] {this.path, folder})));
      }

      folders[folder].Set(keys, values);
    }


    /// <summary>
    /// Get a specific value from a folder.
    /// </summary>
    /// <param name="folder">The folder to get data for.</param>
    /// <param name="key">The name of the value to get.</param>
    /// <param name="value">The output value.</param>
    /// <typeparam name="T">The type of data to get.</typeparam>
    /// <returns>True if the value was retrieved successfully. False otherwise.</returns>
    public bool Get<T>(string folder, string key, out T value) {
      if (!folders.ContainsKey(folder)) {
        value = default(T);
        return false;
      }

      return folders[folder].Get(key, out value);
    }


    /// <summary>
    /// Get a list of values from a folder.
    /// </summary>
    /// <param name="folder">The folder to get values from.</param>
    /// <param name="keys">The names of the values to get.</param>
    /// <param name="values">The output list of values.</param>
    /// <typeparam name="T">The type of data to get.</typeparam>
    /// <returns>True if all values were retrieved successfully. False otherwise.</returns>
    public bool Get<T>(string folder, IList<string> keys, out List<T> values) {
      if (!folders.ContainsKey(folder)) {
        values = null;
        return false;
      }

      values = new List<T>();

      foreach (string key in keys) {
        if (folders[folder].Get(key, out T value)) {
          values.Add(value);
        } else {
          values = null;
          return false;
        }
      }

      return true;
    }


    /// <summary>
    /// Get a list of values from a folder.
    /// </summary>
    /// <param name="folder">The folder to get values from.</param>
    /// <param name="keys">The names of the values to get.</param>
    /// <param name="values">The output list of values.</param>
    /// <typeparam name="T">The type of data to get.</typeparam>
    /// <returns>True if all values were retrieved successfully. False otherwise.</returns>
    public bool Get<T>(string folder, IList<string> keys, out T[] values) {
      if (!folders.ContainsKey(folder)) {
        values = null;
        return false;
      }

      List<T> v = new List<T>();

      foreach (string key in keys) {
        if (folders[folder].Get(key, out T value)) {
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
    /// Get a value from a particular folder.
    /// </summary>
    /// <param name="folder">The folder to get data for.</param>
    /// <param name="key">The name of the value to get.</param>
    /// <typeparam name="T">The data type of the value to get.</typeparam>
    /// <returns>The value.</returns>
    public T Get<T>(string folder, string key) {
      if (folders.ContainsKey(folder)) {
        return folders[folder].Get<T>(key);
      }
      
      return default(T);
    }


    /// <summary>
    /// Get a list of values from a folder.
    /// </summary>
    /// <param name="folder">The folder to get values from.</param>
    /// <param name="keys">The names of the values to get.</param>
    /// <typeparam name="T">The type of data to get.</typeparam>
    /// <returns>The list of values.</returns>
    public List<T> Get<T>(string folder, IList<string> keys) {
      List<T> values = new List<T>();

      if (folders.ContainsKey(folder)) {
        foreach (string key in keys) {
          values.Add(folders[folder].Get<T>(key));
        }

        if (values.Count != keys.Count) {
          values.Clear();
          throw new UnityException("Not all keys existed in the list of keys!");
        }
      }


      return values;
    }

    public bool IsSet<T>(string folder, string key) {
      if (folders.ContainsKey(folder)) {
        return folders[folder].Contains<T>(key);
      }

      return false;
    }

    /// <summary>
    /// Removes a key-value pair from a specific folder
    /// </summary>
    /// <param name="folder">The folder to remove the value from.</param>
    /// <param name="key">The names of the value to remove.</param>
    /// <typeparam name="T">The type of data to remove.</typeparam>
    /// <returns>True if the value was removed successfully.</returns>
    public bool Clear<T>(string folder, string key) {
      if (folders.ContainsKey(folder)) {
        return folders[folder].Clear<T>(key);
      }
      return false;
    }

    /// <summary>
    /// Removes a list of key-value pairs from a specific folder.
    /// </summary>
    /// <param name="folder">The folder to remove values from.</param>
    /// <param name="keys">The names of the values to remove.</param>
    /// <typeparam name="T">The type of the date to remove.</typeparam>
    public void Clear<T>(string folder, IList<string> keys) {
      if (folders.ContainsKey(folder)) {
        folders[folder].Clear<T>(keys);
      }
    }

    /// <summary>
    /// Saves all game data out to disk.
    /// </summary>
    /// <returns>True if all folder saved successfully. False otherwise.</returns>
    public bool Save() {
      foreach (string folder in folders.Keys) {
        if (!Save(folder)) {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Saves data for a single level out to disk.
    /// </summary>
    /// <param name="folder">The level to save.</param>
    /// <returns>True if the level saved successfully. False otherwise.</returns>
    public bool Save(string folder) {
      if (folders.ContainsKey(folder)) {
        return folders[folder].Save();
      }

      return false;
    }

    /// <summary>
    /// Register a new level in the save file.
    /// </summary>
    /// <param name="foldername">The name of the level to register.</param>
    /// <returns>True if the level was added successfully. False otherwise.</returns>
    public bool RegisterLevel(string foldername) {
      if (!folders.ContainsKey(foldername)) {
        folders.Add(foldername, new VirtualFolder(P.Combine(new string[] {this.path, foldername})));
        return true;
      }

      return false;
    }


    /// <summary>
    /// Clear all data in memory for this save file.
    /// </summary>
    public void Clear() {
      foreach (string name in folders.Keys) {
        ClearLevel(name);
      }
    }

    /// <summary>
    /// Clear all data in memory for a specific level.
    /// </summary>
    /// <param name="levelname">The name of the level to clear.</param>
    public void ClearLevel(string levelname) {
      folders[levelname].ClearAll();
    }

    /// <summary>
    /// Delete the save file.
    /// </summary>
    public void DeleteFolder() {
      if (D.Exists(path)) {
        D.Delete(path, true);
      }
    }

    /// <summary>
    /// Delete all save data for a specific level.
    /// </summary>
    /// <param name="levelname">The name of the level to delete.</param>
    public void DeleteLevelFolder(string levelname) {
      folders[levelname].DeleteFolder();
    }

    public override string ToString() {
      StringBuilder builder = new StringBuilder();

      builder.AppendLine(new String('-', 80));
      builder.AppendLine("Data for save slot: " + Name);
      builder.AppendLine(new String('-', 80));

      foreach (string name in folders.Keys) {
        builder.AppendLine(folders[name].ToString());
      }

      return builder.ToString();
    }
    #endregion


    #region Helper Methods

    /// <summary>
    /// Builds out the subdirectories for this path.
    /// </summary>
    /// <param name="gamename">the path to this directory</param>
    /// <returns>The unqualified path to the directory for this save file's data.</returns>
    private void BuildDirectory(string path) {
      if (D.Exists(path)) {
        
        string[] pathPieces = path.Split(P.DirectorySeparatorChar);

        //<persistentDataPath>/gamename/slotname/
        slotname = pathPieces[pathPieces.Length-1];
        gamename = pathPieces[pathPieces.Length-2];

        string[] paths = D.GetDirectories(path);

        foreach(string subfolder in paths) {
          VirtualFolder folder = new VirtualFolder(subfolder);
          folders.Add(folder.Name, folder);
        }
      } 
    }
    #endregion
  }
}