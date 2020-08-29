using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Storm.Extensions;
using UnityEngine;

namespace Storm.Subsystems.Saving {

  /// <summary>
  /// The top level Facade for data management.
  /// </summary>
  public static class Data {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The save files.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private static List<SaveFile> slots = new List<SaveFile>();

    /// <summary>
    /// The name of the folder to save game data out to.
    /// </summary>
    public static string FolderName = "journey_data";


    private static string directory = Path.Combine(new string[] {Application.persistentDataPath, FolderName});

    /// <summary>
    /// The currently loaded savefile for the game.
    /// </summary>
    private static SaveFile activeFile;

    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Saves game data for a certain save file out to disk.
    /// </summary>
    /// <param name="filename">The name of the save file to save.</param>
    public static bool Save(string filename) {
      SaveFile file = slots.Find((SaveFile slot) => slot.Name == filename);

      if (file != null) {
        return file.Save();
      }

      return false;
    }

    /// <summary>
    /// Saves game data for the active save file.
    /// </summary>
    /// <returns>True if the save was successful. False otherwise.</returns>
    public static bool Save() {
      if (activeFile != null) {
        return activeFile.Save();
      }

      return false;
    }

    /// <summary>
    /// Load the list of available save files.
    /// </summary>
    /// <returns></returns>
    public static bool Preload() {
      string[] paths = Directory.GetDirectories(directory);

      foreach (string path in paths) {
        string saveFileName = path.Remove(0, directory.Length);
        SaveFile file = new SaveFile(FolderName, saveFileName);
        slots.Add(file);
      }

      return true;
    }


    /// <summary>
    /// Loads all game data for a certain save file from disk.
    /// </summary>
    /// <param name="filename">The name of the save file to load.</param>
    public static bool Load(string filename) {
      SaveFile file = slots.Find((SaveFile slot) => slot.Name == filename);

      if (file != null) {
        activeFile = file;
        return file.Load();
      }

      return false;
    }

    /// <summary>
    /// Delete a slot with a given name.
    /// </summary>
    /// <param name="filename">The name of the save file to delete.</param>
    public static void Delete(string filename) {
      SaveFile file = slots.Find((SaveFile slot) => slot.Name == filename);

      if (file != null) {
        file.DeleteFolder();
        slots.Remove(file);
      }
    }

    /// <summary>
    /// Create a save slot.
    /// </summary>
    /// <param name="filename">The name of the save file to create.</param>
    public static void CreateSaveFile(string filename) {
      SaveFile file = new SaveFile(FolderName, filename);
      slots.Add(file);
    }


    public static void Set<T>(string file, string key, T value) {
      if (activeFile != null) {
        activeFile.Set(file, key, value);
      }
    }

    public static void Set<T>(string file, IList<string> keys, IList<T> values) {
      if (activeFile != null) {
        activeFile.Set(file, keys, values);
      }
    }


    public static T Get<T>(string file, string key) {
      if (activeFile != null) {
        return activeFile.Get<T>(file, key);
      }

      return default(T);
    }

    public static bool Get<T>(string file, string key, out T value) {
      if (activeFile != null) {
        return activeFile.Get<T>(file, key, out value);
      }

      value = default(T);
      return false;
    }


    public static List<T> Get<T>(string file, IEnumerable<string> keys) {
      if (activeFile != null) {
        return activeFile.Get<T>(file, keys);
      }

      return new List<T>();
    }

    public static bool Get<T>(string file, IList<string> keys, out T[] values) {
      if (activeFile != null) {
        return activeFile.Get<T>(file, keys, out values);
      }

      values = null;
      return false;
    }

    public static bool Get<T>(string file, IList<string> keys, out List<T> values) {
      if (activeFile != null) {
        return activeFile.Get<T>(file, keys, out values);
      }

      values = null;
      return false;
    }
    #endregion
  }
}