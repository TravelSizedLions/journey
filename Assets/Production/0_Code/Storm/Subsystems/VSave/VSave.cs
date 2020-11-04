using System.Collections.Generic;
using System.IO;
using IO = System.IO;
using Sirenix.OdinInspector;
using Storm.Extensions;
using UnityEngine;
using System;

namespace Storm.Subsystems.Save {

  /// <summary>
  /// The save data system!
  /// </summary>
  /// <remarks>
  /// VSave is a dynamic saving system that organizes save data into slots.
  /// Using <see cref="VSave.CreateSlot" /> and <see cref="VSave.ChooseSlot" />, you set the active save
  /// slot for a play session. Then, any data set using <see cref="VSave.Set" /> will get set
  /// within the active save file. 
  /// 
  /// Using <see cref="VSave.Save()" /> will then save the slot out to disk.   
  /// </remarks>
  public static class VSave {

    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------

    /// <summary>
    /// The name of the folder to save game data out to.
    /// </summary>
    public static string FolderName { 
      get { 
        return folderName;
      } 
      set {
        folderName = value;
        directory = System.IO.Path.Combine(new string[] { Application.persistentDataPath, folderName });
      }
    }

    /// <summary>
    /// The unqualified path where the game's save data will be located.
    /// </summary>
    public static string Path { 
      get { return directory; } 
    }

    /// <summary>
    /// The number of save slots.
    /// </summary>
    public static int SlotCount {
      get { return slots.Count; }
    } 

    /// <summary>
    /// The number of physical save folders created.
    /// </summary>
    public static int PhysicalSlotCount {
      get { 
        if (!Directory.Exists(Path)) {
          return 0;
        }

        return Directory.GetDirectories(Path).Length; 
      }
    }

    /// <summary>
    /// The number of pieces of data loaded in the active save slot. This is NOT the
    /// save slot's disk size.
    /// </summary>
    public static int ActiveDataCount {
      get {
        if (activeSlot != null) {
          return activeSlot.DataCount;
        }
        return 0;
      }
    }

    /// <summary>
    /// The total pieces of data shared accross all files. This is NOT the total
    /// disk size.
    /// </summary>
    public static int TotalDataCount {
      get {
        int total = 0;

        foreach (SaveSlot slot in slots) {
          total += slot.DataCount;
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
    /// The save slots.
    /// </summary>
    private static List<SaveSlot> slots = new List<SaveSlot>();

    /// <summary>
    /// The name of the folder to save game data out to.
    /// </summary>
    private static string folderName;

    /// <summary>
    /// The directory where the game's save data will be located.
    /// </summary>
    private static string directory = System.IO.Path.Combine(new string[] { Application.persistentDataPath, "default" });

    /// <summary>
    /// The currently loaded savefile for the game.
    /// </summary>
    private static SaveSlot activeSlot;

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
      SaveSlot file = slots.Find((SaveSlot slot) => slot.Name == filename);

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
      if (activeSlot != null) {
        return activeSlot.Save();
      }

      return false;
    }

    /// <summary>
    /// Load the list of available save files.
    /// </summary>
    /// <returns>True if the list of save files was loaded successfully.</returns>
    public static bool LoadSlots() {
      string[] paths = Directory.GetDirectories(directory);

      foreach (string path in paths) {
        SaveSlot file = new SaveSlot(path);
        slots.Add(file);
      }

      return true;
    }

    /// <summary>
    /// Delete a save slot.
    /// </summary>
    /// <param name="filename">The name of the save file to delete.</param>
    public static void Delete(string filename) {
      SaveSlot file = slots.Find((SaveSlot slot) => slot.Name == filename);

      if (file != null) {
        file.DeleteFolder();
        slots.Remove(file);
      }
    }

    /// <summary>
    /// Creates a new save slot.
    /// </summary>
    /// <param name="filename">The name of the save file to create.</param>
    public static void CreateSlot(string filename) {
      

      string path = IO.Path.Combine(Path, filename);
      if (!Directory.Exists(path)) {
        Directory.CreateDirectory(path);
      }

      SaveSlot file = new SaveSlot(path);

      slots.Add(file);
    }

    /// <summary>
    /// Choose a save slot. This slot will now act as the active save slot for
    /// any data transactions.
    /// </summary>
    /// <param name="filename">The name of the slot.</param>
    /// <returns>True if the slot exists. False otherwise.</returns>
    public static bool ChooseSlot(string filename) {
      SaveSlot slot = slots.Find((SaveSlot s) => s.Name == filename);

      if (slot != null) {
        activeSlot = slot;
        return true;
      }

      return false;
    }

    /// <summary>
    /// Choose a save slot. This slot will now act as the active save slot for
    /// any data transactions.
    /// </summary>
    /// <param name="index">The index of the save slot.</param>
    /// <returns>True if the slot exists. False otherwise.</returns>
    public static bool ChooseSlot(int index) {
      if (index < 0 || index >= slots.Count) {
        return false;
      }

      activeSlot = slots[index];
      return true;
    }

    /// <summary>
    /// Set a value to a sub folder.
    /// </summary>
    /// <param name="folder">The folder to save data to.</param>
    /// <param name="key">The name of the value to save.</param>
    /// <param name="value">The data to save.</param>
    /// <typeparam name="T">The type of the data to save.</typeparam>
    public static void Set<T>(string folder, string key, T value) {
      if (activeSlot != null) {
        activeSlot.Set(folder, key, value);
      }
    }

    /// <summary>
    /// Set a list of values to a sub folder.
    /// </summary>
    /// <param name="folder">The folder to save data to.</param>
    /// <param name="keys">The names of the values to save.</param>
    /// <param name="values">The data to save.</param>
    /// <typeparam name="T">The type of the data to save.</typeparam>
    public static void Set<T>(string folder, IList<string> keys, IList<T> values) {
      if (activeSlot != null) {
        activeSlot.Set(folder, keys, values);
      }
    }

    /// <summary>
    /// Get a value from a sub folder.
    /// </summary>
    /// <param name="folder">The folder to get data from.</param>
    /// <param name="key">The name of the value to get.</param>
    /// <typeparam name="T">The type of data to get.</typeparam>
    /// <returns>The value.</returns>
    public static T Get<T>(string folder, string key) {
      if (activeSlot != null) {
        return activeSlot.Get<T>(folder, key);
      }

      return default(T);
    }

    /// <summary>
    /// Try to get a value from a sub folder.
    /// </summary>
    /// <param name="folder">The folder to get data from.</param>
    /// <param name="key">The name of the value to get.</param>
    /// <param name="value">The output value.</param>
    /// <typeparam name="T">The type of data to get.</typeparam>
    /// <returns>True if the value was retrieved successfully. False otherwise.</returns>
    public static bool Get<T>(string folder, string key, out T value) {
      if (activeSlot != null) {
        return activeSlot.Get<T>(folder, key, out value);
      }

      value = default(T);
      return false;
    }

    /// <summary>
    /// Get a list of values from a sub folder.
    /// </summary>
    /// <param name="folder">The folder to get data from.</param>
    /// <param name="keys">The list of keys.</param>
    /// <typeparam name="T">The type of data to get.</typeparam>
    /// <returns>The list of values.</returns>
    public static List<T> Get<T>(string folder, IList<string> keys) {
      if (activeSlot != null) {
        return activeSlot.Get<T>(folder, keys);
      }

      return new List<T>();
    }

    /// <summary>
    /// Try to get a list of values from a sub folder.
    /// </summary>
    /// <param name="folder">The folder to get data from.</param>
    /// <param name="keys">The list of keys.</param>
    /// <param name="values">The output list of values.</param>
    /// <typeparam name="T">The type of data to get.</typeparam>
    /// <returns>True if the values were retrieved successfully. False otherwise.</returns>
    public static bool Get<T>(string folder, IList<string> keys, out T[] values) {
      if (activeSlot != null) {
        return activeSlot.Get<T>(folder, keys, out values);
      }

      values = null;
      return false;
    }

    /// <summary>
    /// Try to get a list of values from a sub folder.
    /// </summary>
    /// <param name="folder">The folder to get data from.</param>
    /// <param name="keys">The list of keys.</param>
    /// <param name="values">The output list of values.</param>
    /// <typeparam name="T">The type of data to get.</typeparam>
    /// <returns>True if the values were retrieved successfully. False otherwise.</returns>
    public static bool Get<T>(string folder, IList<string> keys, out List<T> values) {
      if (activeSlot != null) {
        return activeSlot.Get<T>(folder, keys, out values);
      }

      values = null;
      return false;
    }

    /// <summary>
    /// Resets the Save Data system.
    /// </summary>
    /// <param name="ignoreFolders">Do not delete the physical folders.</param>
    /// <returns>True if</returns>
    public static void Reset(bool ignoreFolders = false) {
      activeSlot = null;
      if (!ignoreFolders) {
        foreach (SaveSlot slot in slots) {
          slot.DeleteFolder();
        }
      }

      slots = new List<SaveSlot>();
    }


    public static void ReportContents() {
      Debug.Log(activeSlot.ToString());
    }
    #endregion
  }
}